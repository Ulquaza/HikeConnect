namespace HikeConnect.Core.Entities
{
    public class BehavioralProfile
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public int RiskTolerance { get; set; }
        public string PacingStyle { get; set; }
        public int DisciplineLevel { get; set; }
        public string ConflictStrategy { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public virtual User User { get; set; }
    }
}
