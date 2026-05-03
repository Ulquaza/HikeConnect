using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using HikeConnect.Core.Dtos;
using HikeConnect.Core.Interfaces;
using HikeConnect.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HikeConnect.Infrastructure.Crm
{
    /// <summary>
    /// Twenty REST Core API sync for behavioralleads via find/update-or-create.
    /// </summary>
    public sealed class TwentyCrmClient : ICrmClient
    {
        private const string BehavioralleadsPath = "rest/behavioralleads";

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
                ["profileupdatedat"] = request.ProfileUpdatedAt.ToUniversalTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            };

            try
            {
                _logger.LogInformation(
                    "Twenty CRM sync started for behavioral lead. SourceUserId: {SourceUserId}, SourceProfileId: {SourceProfileId}, Email: {Email}.",
                    request.SourceUserId,
                    request.SourceProfileId,
                    request.Email);

                var existingId = await FindExistingBehavioralLeadIdAsync(
                    request.SourceUserId.ToString("D"),
                    cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(existingId))
                {
                    _logger.LogInformation(
                        "Twenty CRM behavioral lead found by sourceuserid. SourceUserId: {SourceUserId}, ExistingId: {ExistingId}. Updating.",
                        request.SourceUserId,
                        existingId);

                    var patchPath = $"{BehavioralleadsPath}/{Uri.EscapeDataString(existingId)}";
                    var updateResult = await SendRestAsync(
                        new HttpMethod("PATCH"),
                        patchPath,
                        record,
                        cancellationToken).ConfigureAwait(false);
                    if (!updateResult.Success)
                        return updateResult.Error!;

                    var updatedId = TryReadIdFromRestPayload(updateResult.Data!.Value);
                    _logger.LogInformation(
                        "Twenty CRM behavioral lead updated. SourceUserId: {SourceUserId}, ExistingId: {ExistingId}, ReturnedId: {ReturnedId}.",
                        request.SourceUserId,
                        existingId,
                        updatedId);
                    return CrmOperationResult.Ok(updatedId ?? existingId);
                }

                _logger.LogInformation(
                    "Twenty CRM behavioral lead was not found by sourceuserid. SourceUserId: {SourceUserId}. Creating.",
                    request.SourceUserId);

                var createResult = await SendRestAsync(
                    HttpMethod.Post,
                    BehavioralleadsPath,
                    record,
                    cancellationToken).ConfigureAwait(false);
                if (!createResult.Success)
                    return createResult.Error!;

                var createdId = TryReadIdFromRestPayload(createResult.Data!.Value);
                _logger.LogInformation(
                    "Twenty CRM behavioral lead created. SourceUserId: {SourceUserId}, CreatedId: {CreatedId}.",
                    request.SourceUserId,
                    createdId);
                return CrmOperationResult.Ok(createdId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Twenty CRM sync cancelled for behavioral lead. SourceUserId: {SourceUserId}, SourceProfileId: {SourceProfileId}.",
                    request.SourceUserId,
                    request.SourceProfileId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Twenty CRM sync failed for behavioral lead. SourceUserId: {SourceUserId}, SourceProfileId: {SourceProfileId}, Email: {Email}.",
                    request.SourceUserId,
                    request.SourceProfileId,
                    request.Email);
                return CrmOperationResult.Fail(ex.Message);
            }
        }

        private async Task<string?> FindExistingBehavioralLeadIdAsync(string sourceUserId, CancellationToken cancellationToken)
        {
            var filter = $"sourceuserid[eq]:\"{sourceUserId}\"";
            var path = $"{BehavioralleadsPath}?limit=1&filter={Uri.EscapeDataString(filter)}";
            _logger.LogDebug(
                "Twenty CRM find behavioral lead request. SourceUserId: {SourceUserId}, Path: {Path}, Filter: {Filter}.",
                sourceUserId,
                path,
                filter);
            var result = await SendRestAsync(HttpMethod.Get, path, body: null, cancellationToken).ConfigureAwait(false);
            if (!result.Success || result.Data is null)
            {
                _logger.LogWarning(
                    "Twenty CRM find behavioral lead failed or returned empty data. SourceUserId: {SourceUserId}, Success: {Success}.",
                    sourceUserId,
                    result.Success);
                return null;
            }

            var existingId = TryReadFirstIdFromRestList(result.Data.Value);
            _logger.LogDebug(
                "Twenty CRM find behavioral lead completed. SourceUserId: {SourceUserId}, ExistingId: {ExistingId}.",
                sourceUserId,
                existingId);
            return existingId;
        }

        private async Task<(bool Success, JsonElement? Data, CrmOperationResult? Error)> SendRestAsync(
            HttpMethod method,
            string path,
            Dictionary<string, object?>? body,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(method, path);
            if (body is not null)
                request.Content = JsonContent.Create(body);

            _logger.LogDebug(
                "Twenty CRM REST request started. Method: {Method}, Path: {Path}, HasBody: {HasBody}.",
                method.Method,
                path,
                body is not null);

            using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Twenty CRM HTTP {StatusCode}: {Body}", (int)response.StatusCode, json);
                return (false, null, CrmOperationResult.Fail($"Twenty CRM HTTP {(int)response.StatusCode}."));
            }

            _logger.LogDebug(
                "Twenty CRM REST request completed. Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, ResponseLength: {ResponseLength}.",
                method.Method,
                path,
                (int)response.StatusCode,
                json.Length);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            return (true, root.Clone(), null);
        }

        private static string? TryReadIdFromRestPayload(JsonElement payload)
        {
            var id = ReadId(payload);
            if (!string.IsNullOrWhiteSpace(id))
                return id;

            if (payload.ValueKind == JsonValueKind.Object
                && payload.TryGetProperty("data", out var dataElement))
            {
                id = ReadId(dataElement);
                if (!string.IsNullOrWhiteSpace(id))
                    return id;
            }

            return null;
        }

        private static string? TryReadFirstIdFromRestList(JsonElement payload)
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

            if (payload.ValueKind == JsonValueKind.Object
                && payload.TryGetProperty("data", out var dataElement))
            {
                if (dataElement.ValueKind == JsonValueKind.Array)
                {
                    if (dataElement.GetArrayLength() == 0)
                        return null;
                    return ReadId(dataElement[0]);
                }

                if (dataElement.ValueKind == JsonValueKind.Object
                    && dataElement.TryGetProperty("data", out var innerData)
                    && innerData.ValueKind == JsonValueKind.Array)
                {
                    if (innerData.GetArrayLength() == 0)
                        return null;
                    return ReadId(innerData[0]);
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
