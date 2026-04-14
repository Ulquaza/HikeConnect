using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class BehavioralProfileService : IBehavioralProfileService
    {
        private readonly IBehavioralProfileRepository _behavioralProfileRepository;
        private readonly IMatchingService _matchingService;

        public BehavioralProfileService(
            IBehavioralProfileRepository behavioralProfileRepository,
            IMatchingService matchingService)
        {
            _behavioralProfileRepository = behavioralProfileRepository;
            _matchingService = matchingService;
        }

        public async Task<BehavioralProfile?> CreateAsync(BehavioralSurveySubmissionRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null || request.UserId == Guid.Empty) return null;

            var normalizedProfile = _matchingService.BuildBehavioralProfile(request);
            return await _behavioralProfileRepository.AddAsync(normalizedProfile, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) return;

            await _behavioralProfileRepository.DeleteAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _behavioralProfileRepository.GetAllAsync(cancellationToken);
        }

        public async Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) return null;

            return await _behavioralProfileRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty) return null;

            return await _behavioralProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        }

        public async Task<BehavioralProfile?> UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            if (profile is null || profile.Id == Guid.Empty || profile.UserId == Guid.Empty)
            {
                return null;
            }

            profile.LastUpdatedAt = DateTime.UtcNow;
            return await _behavioralProfileRepository.UpdateAsync(profile, cancellationToken);
        }

    }
}
