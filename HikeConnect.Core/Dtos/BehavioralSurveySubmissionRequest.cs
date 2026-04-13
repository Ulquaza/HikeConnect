namespace HikeConnect.Core.Dtos
{
    public class BehavioralSurveySubmissionRequest
    {
        public Guid UserId { get; set; }
        public ICollection<BehavioralSurveyAnswerDto> Answers { get; set; } = new List<BehavioralSurveyAnswerDto>();
    }
}
