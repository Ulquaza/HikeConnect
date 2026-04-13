namespace HikeConnect.Core.Entities
{
    public class BehavioralProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int RiskTolerance { get; set; }
        public string PacingStyle { get; set; }
        public int DisciplineLevel { get; set; }
        public string ConflictStrategy { get; set; }
        public DateTime LastUpdatedAt { get; set; } // в будущем возможно версионирование

        public virtual User User { get; set; }
    }
}
