namespace HikeConnect.Core.Entities
{
    public enum ParticipationRequestStatus
    {
        Pending,    // в ожидании
        Approved,   // принят
        Rejected,   // отклонён
        Canceled    // отменён
    }

    public class ParticipationRequest
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid UserId { get; set; }
        public ParticipationRequestStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }

        public virtual Trip Trip { get; set; }
        public virtual User User { get; set; }
    }
}
