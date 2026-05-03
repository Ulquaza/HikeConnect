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
    /// Twenty Core API (<c>/graphql</c>) — upsert via <c>createMany*</c> + <c>upsert: true</c> (Twenty 2.x).
    /// </summary>
    public sealed class TwentyCrmClient : ICrmClient
    {
        private readonly HttpClient _http;
        private readonly IOptions<CrmTwentySettings> _options;
        private readonly ILogger<TwentyCrmClient> _logger;

        public TwentyCrmClient(
            HttpClient http,
            IOptions<CrmTwentySettings> options,
            ILogger<TwentyCrmClient> logger)
        {
            _http = http;
            _options = options;
            _logger = logger;
        }

        public async Task<CrmOperationResult> UpsertBehavioralLeadAsync(
            CrmBehavioralLeadSyncRequest request,
            CancellationToken cancellationToken = default)
        {
            var settings = _options.Value;
            if (!settings.IsConfigured)
            {
                _logger.LogInformation("Twenty CRM sync skipped (Crm:Twenty not fully configured).");
                return CrmOperationResult.Skipped();
            }

            var mutationField = settings.GetCreateManyMutationFieldName();
            var inputType = string.IsNullOrWhiteSpace(settings.CreateInputTypeName)
                ? "BehavioralLeadCreateInput"
                : settings.CreateInputTypeName.Trim();

            var mutation =
                $"mutation UpsertBehavioralLead($data: [{inputType}!]!, $upsert: Boolean!) {{ "
                + $"{mutationField}(data: $data, upsert: $upsert) {{ id }} }}";

            var record = new Dictionary<string, object?>
            {
                ["sourceUserId"] = request.SourceUserId.ToString("D"),
                ["sourceProfileId"] = request.SourceProfileId.ToString("D"),
                ["email"] = request.Email,
                ["fullName"] = request.FullName,
                ["riskTolerance"] = request.RiskTolerance,
                ["pacingStyle"] = request.PacingStyle,
                ["disciplineLevel"] = request.DisciplineLevel,
                ["conflictStrategy"] = request.ConflictStrategy,
                ["profileUpdatedAt"] = request.ProfileUpdatedAt.ToUniversalTime().ToString("o"),
            };

            var variables = new Dictionary<string, object?>
            {
                ["data"] = new[] { record },
                ["upsert"] = true,
            };

            var body = new Dictionary<string, object?>
            {
                ["query"] = mutation,
                ["variables"] = variables,
            };

            var path = string.IsNullOrWhiteSpace(settings.GraphQlPath)
                ? "graphql"
                : settings.GraphQlPath.TrimStart('/');

            try
            {
                using var response = await _http.PostAsJsonAsync(path, body, cancellationToken).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Twenty CRM HTTP {StatusCode}: {Body}",
                        (int)response.StatusCode,
                        json);
                    return CrmOperationResult.Fail($"Twenty CRM HTTP {(int)response.StatusCode}.");
                }

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
                {
                    var message = errors.GetArrayLength() > 0 && errors[0].TryGetProperty("message", out var msg)
                        ? msg.GetString()
                        : "GraphQL error.";
                    _logger.LogWarning(
                        "Twenty CRM GraphQL error: {Message}. If unknown field/type: introspect mutations and align Crm:Twenty ObjectPluralName, CreateManyMutationName, CreateInputTypeName and payload keys with workspace schema. Body: {Body}",
                        message,
                        json);
                    return CrmOperationResult.Fail(message ?? "GraphQL error.");
                }

                if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
                    return CrmOperationResult.Fail("Twenty CRM: missing data in GraphQL response.");

                if (!data.TryGetProperty(mutationField, out var payload))
                    return CrmOperationResult.Fail(
                        $"Twenty CRM: missing field '{mutationField}' in response — проверьте CreateManyMutationName / ObjectPluralName по схеме GraphQL workspace.");

                var id = TryReadFirstRecordId(payload);
                return CrmOperationResult.Ok(id);
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

        private static string? TryReadFirstRecordId(JsonElement payload)
        {
            if (payload.ValueKind == JsonValueKind.Array)
            {
                if (payload.GetArrayLength() == 0)
                    return null;
                return ReadId(payload[0]);
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
