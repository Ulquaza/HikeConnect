using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IMatchingService
    {
        BehavioralProfile BuildBehavioralProfile(BehavioralSurveySubmissionRequest request);
        CompatibilityReport BuildCompatibilityReport(BehavioralProfile authorProfile, BehavioralProfile targetProfile);
    }
}
