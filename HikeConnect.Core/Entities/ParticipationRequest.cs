namespace HikeConnect.Core.Entities
{
    public enum ParticipationRequestStatus
    {
        Pending,    // в ожидании
        Approved,   // принят
        Rejected    // отклонён
    }

    public class ParticipationRequest
    {
        public string Id { get; set; }
        public string TripId { get; set; }
        public string UserId { get; set; }
        public ParticipationRequestStatus Status { get; set; }
        public int CompatibilityPercentage { get; set; }
        public DateTime AppliedAt { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual User User { get; set; }
    }
}
