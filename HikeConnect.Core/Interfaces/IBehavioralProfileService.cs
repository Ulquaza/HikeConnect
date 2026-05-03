using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IBehavioralProfileService
    {
        Task<BehavioralProfileDto?> CreateAsync(BehavioralSurveySubmissionRequest request, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Пересчёт профиля из ответов опроса и обновление существующей записи пользователя.</summary>
        Task<BehavioralProfileDto?> UpdateFromSurveyAsync(BehavioralSurveySubmissionRequest request, Guid userId, CancellationToken cancellationToken = default);

        Task<BehavioralProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<BehavioralProfileDto?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BehavioralProfileDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<BehavioralProfileDto?> UpdateAsync(BehavioralProfile profile, Guid userId, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
