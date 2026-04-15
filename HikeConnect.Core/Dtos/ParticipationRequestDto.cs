using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Dtos
{
    public class ParticipationRequestDto
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }
        public Guid UserId { get; set; }
        public ParticipationRequestStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public string? UserFullName { get; set; }
        public string? UserName { get; set; }
    }
}
