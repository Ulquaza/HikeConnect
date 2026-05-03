using System.Net.Http.Json;
using System.Text.Json;
using HikeConnect.Core.Dtos;
using HikeConnect.Core.Interfaces;
using HikeConnect.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HikeConnect.Infrastructure.Crm
{
    /// <summary>
    /// Twenty Core API (/graphql) sync via find/update-or-create.
    /// Uses operations observed in Twenty UI: findManyBehavioralleads,
    /// updateBehaviorallead, createBehaviorallead.
    /// </summary>
    public sealed class TwentyCrmClient : ICrmClient
    {
        private readonly HttpClient _http;
        private readonly CrmTwentySettings _twentySettings;
        private readonly ILogger<TwentyCrmClient> _logger;

        public TwentyCrmClient(HttpClient http, IOptions<CrmTwentySettings> options, ILogger<TwentyCrmClient> logger)
        {
            _http = http;
            _twentySettings = options.Value;
            _logger = logger;
        }

        public async Task<CrmOperationResult> UpsertBehavioralLeadAsync(CrmBehavioralLeadSyncRequest request, CancellationToken cancellationToken = default)
        {
            if (!_twentySettings.IsConfigured)
            {
                _logger.LogWarning(
                    "Twenty CRM sync skipped: Crm:Twenty is not fully configured. Enabled: {Enabled}, BaseUrl configured: {HasBaseUrl}, ApiKey configured: {HasApiKey}.",
                    _twentySettings.Enabled,
                    !string.IsNullOrWhiteSpace(_twentySettings.BaseUrl),
                    !string.IsNullOrWhiteSpace(_twentySettings.ApiKey));
                return CrmOperationResult.Skipped();
            }

            var record = new Dictionary<string, object?>
            {
                // Field names are lowercase in this Twenty workspace schema.
                ["sourceuserid"] = request.SourceUserId.ToString("D"),
                ["sourceprofileid"] = request.SourceProfileId.ToString("D"),
                ["email"] = request.Email,
                ["fullname"] = request.FullName,
                ["risktolerance"] = request.RiskTolerance,
                ["pacingstyle"] = request.PacingStyle,
                ["disciplinelevel"] = request.DisciplineLevel,
                ["conflictstrategy"] = request.ConflictStrategy,
                ["profileupdatedat"] = request.ProfileUpdatedAt.ToUniversalTime().ToString("o"),
            };

            var path = string.IsNullOrWhiteSpace(_twentySettings.GraphQlPath)
                ? "graphql"
                : _twentySettings.GraphQlPath.TrimStart('/');

            try
            {
                var existingId = await FindExistingBehavioralLeadIdAsync(
                    path,
                    request.SourceUserId.ToString("D"),
                    cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(existingId))
                {
                    var updateQuery =
                        "mutation UpdateOneBehaviorallead($idToUpdate: UUID!, $input: BehavioralleadUpdateInput!) { " +
                        "updateBehaviorallead(id: $idToUpdate, data: $input) { id } }";
                    var updateVariables = new Dictionary<string, object?>
                    {
                        ["idToUpdate"] = existingId,
                        ["input"] = record,
                    };

                    var updateResult = await SendGraphQlAsync(path, updateQuery, updateVariables, cancellationToken).ConfigureAwait(false);
                    if (!updateResult.Success)
                        return updateResult.Error!;

                    var updatedId = TryReadFieldId(updateResult.Data!.Value, "updateBehaviorallead");
                    return CrmOperationResult.Ok(updatedId ?? existingId);
                }

                var createQuery =
                    "mutation CreateOneBehaviorallead($input: BehavioralleadCreateInput!) { " +
                    "createBehaviorallead(data: $input) { id } }";
                var createVariables = new Dictionary<string, object?>
                {
                    ["input"] = record,
                };

                var createResult = await SendGraphQlAsync(path, createQuery, createVariables, cancellationToken).ConfigureAwait(false);
                if (!createResult.Success)
                    return createResult.Error!;

                var createdId = TryReadFieldId(createResult.Data!.Value, "createBehaviorallead");
                return CrmOperationResult.Ok(createdId);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Twenty CRM sync failed.");
                return CrmOperationResult.Fail(ex.Message);
            }
        }

        private async Task<string?> FindExistingBehavioralLeadIdAsync(string path, string sourceUserId, CancellationToken cancellationToken)
        {
            var findQuery =
                "query FindManyBehavioralleads($sourceuserid: String!) { " +
                "findManyBehavioralleads(filter: { sourceuserid: { eq: $sourceuserid } }, first: 1) { " +
                "edges { node { id } } } }";

            var variables = new Dictionary<string, object?>
            {
                ["sourceuserid"] = sourceUserId,
            };

            var result = await SendGraphQlAsync(path, findQuery, variables, cancellationToken).ConfigureAwait(false);
            if (!result.Success || result.Data is null)
                return null;

            if (!result.Data.Value.TryGetProperty("findManyBehavioralleads", out var payload))
                return null;

            return TryReadFirstIdFromCollection(payload);
        }

        private async Task<(bool Success, JsonElement? Data, CrmOperationResult? Error)> SendGraphQlAsync(
            string path,
            string query,
            Dictionary<string, object?> variables,
            CancellationToken cancellationToken)
        {
            var body = new Dictionary<string, object?>
            {
                ["query"] = query,
                ["variables"] = variables,
            };

            using var response = await _http.PostAsJsonAsync(path, body, cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Twenty CRM HTTP {StatusCode}: {Body}", (int)response.StatusCode, json);
                return (false, null, CrmOperationResult.Fail($"Twenty CRM HTTP {(int)response.StatusCode}."));
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
            {
                var message = errors.GetArrayLength() > 0 && errors[0].TryGetProperty("message", out var msg)
                    ? msg.GetString()
                    : "GraphQL error.";
                _logger.LogWarning("Twenty CRM GraphQL error: {Message}. Body: {Body}", message, json);
                return (false, null, CrmOperationResult.Fail(message ?? "GraphQL error."));
            }

            if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
                return (false, null, CrmOperationResult.Fail("Twenty CRM: missing data in GraphQL response."));

            return (true, data.Clone(), null);
        }

        private static string? TryReadFieldId(JsonElement data, string fieldName)
        {
            if (!data.TryGetProperty(fieldName, out var payload))
                return null;

            return ReadId(payload);
        }

        private static string? TryReadFirstIdFromCollection(JsonElement payload)
        {
            if (payload.ValueKind == JsonValueKind.Array)
            {
                if (payload.GetArrayLength() == 0)
                    return null;
                return ReadId(payload[0]);
            }

            if (payload.ValueKind == JsonValueKind.Object
                && payload.TryGetProperty("edges", out var edges)
                && edges.ValueKind == JsonValueKind.Array)
            {
                foreach (var edge in edges.EnumerateArray())
                {
                    if (edge.ValueKind != JsonValueKind.Object)
                        continue;
                    if (!edge.TryGetProperty("node", out var node))
                        continue;

                    var id = ReadId(node);
                    if (!string.IsNullOrWhiteSpace(id))
                        return id;
                }
            }

            return ReadId(payload);
        }

        private static string? ReadId(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            return element.TryGetProperty("id", out var id) ? id.GetString() : null;
        }
    }
}
