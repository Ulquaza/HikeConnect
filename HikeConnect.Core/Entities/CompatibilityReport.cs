using HikeConnect.Core.Dtos;

namespace HikeConnect.Core.Entities
{
    public class CompatibilityReport
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public Guid TargetId { get; set; }
        public ICollection<CompatibilityPoint> RiskPoints { get; set; }   // jsonb
        public ICollection<CompatibilityPoint> MatchPoints { get; set; }  // jsonb
        public string SummaryText { get; set; }

        public virtual User Author { get; set; }
        public virtual User Target { get; set; }
    }
}
