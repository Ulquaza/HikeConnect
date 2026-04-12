using Microsoft.AspNetCore.Identity;

namespace HikeConnect.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string? Bio { get; set; }
        public DateTime RegisteredAt { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }

        public virtual BehavioralProfile BehavioralProfile { get; set; }

        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<ParticipationRequest> ParticipationRequests { get; set; } = new List<ParticipationRequest>();
        public virtual ICollection<CompatibilityReport> CompatibilityReportsWhereAuthor { get; set; } = new List<CompatibilityReport>();
        public virtual ICollection<CompatibilityReport> CompatibilityReportsWhereTarget { get; set; } = new List<CompatibilityReport>();
    }
}
