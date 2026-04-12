using HikeConnect.Core.Dtos;

namespace HikeConnect.Core.Entities
{
    public class CompatibilityReport
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public string TargetId { get; set; }
        public ICollection<CompatibilityPoint> RiskPoints { get; set; }   // jsonb
        public ICollection<CompatibilityPoint> MatchPoints { get; set; }  // jsonb
        public string SummaryText { get; set; }

        public virtual User Author { get; set; }
        public virtual User Target { get; set; }
    }
}
