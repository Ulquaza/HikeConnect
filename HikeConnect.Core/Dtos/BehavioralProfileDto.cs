namespace HikeConnect.Core.Dtos
{
    public class BehavioralProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int RiskTolerance { get; set; }
        public string PacingStyle { get; set; }
        public int DisciplineLevel { get; set; }
        public string ConflictStrategy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
