namespace HikeConnect.Core.Dtos
{
    /// <summary>
    /// Payload mapped to the Twenty custom object (e.g. behavioral leads) — see Context/stage1-crm-research.md.
    /// </summary>
    public sealed class CrmBehavioralLeadSyncRequest
    {
        public required Guid SourceUserId { get; init; }
        public required Guid SourceProfileId { get; init; }
        public required string Email { get; init; }
        public required string FullName { get; init; }
        public required int RiskTolerance { get; init; }
        public required string PacingStyle { get; init; }
        public required int DisciplineLevel { get; init; }
        public required string ConflictStrategy { get; init; }
        public required DateTime ProfileUpdatedAt { get; init; }
    }
}
