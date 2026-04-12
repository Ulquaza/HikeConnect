namespace HikeConnect.Core.Entities
{
    public enum TripStatus
    {
        Planned,     // набор
        Ongoing,     // в процессе
        Completed    // завершён
    }

    public class Trip
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public TripStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User Author { get; set; }

        public virtual ICollection<ParticipationRequest> ParticipationRequests { get; set; } = new List<ParticipationRequest>();
    }
}
