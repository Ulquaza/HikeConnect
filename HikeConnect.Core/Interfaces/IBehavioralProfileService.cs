using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IBehavioralProfileService
    {
        Task<BehavioralProfile?> CreateAsync(BehavioralSurveySubmissionRequest request, CancellationToken cancellationToken = default);

        Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<BehavioralProfile?> UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
