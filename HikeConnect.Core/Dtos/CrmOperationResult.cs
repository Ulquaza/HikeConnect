namespace HikeConnect.Core.Dtos
{
    public sealed class CrmOperationResult
    {
        public bool Success { get; init; }
        public bool WasSkipped { get; init; }
        public string? ErrorMessage { get; init; }
        public string? CrmRecordId { get; init; }

        public static CrmOperationResult Ok(string? crmRecordId = null) =>
            new() { Success = true, CrmRecordId = crmRecordId };

        public static CrmOperationResult Skipped() =>
            new() { Success = true, WasSkipped = true };

        public static CrmOperationResult Fail(string message) =>
            new() { Success = false, ErrorMessage = message };
    }
}
