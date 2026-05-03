using HikeConnect.Core.Dtos;
using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace HikeConnect.Application.Services
{
    public class BehavioralProfileService : IBehavioralProfileService
    {
        private readonly IBehavioralProfileRepository _behavioralProfileRepository;
        private readonly IMatchingService _matchingService;
        private readonly IAuthService _authService;
        private readonly ICrmSyncService _crmSyncService;
        private readonly ILogger<BehavioralProfileService> _logger;

        public BehavioralProfileService(
            IBehavioralProfileRepository behavioralProfileRepository,
            IMatchingService matchingService,
            IAuthService authService,
            ICrmSyncService crmSyncService,
            ILogger<BehavioralProfileService> logger)
        {
            _behavioralProfileRepository = behavioralProfileRepository;
            _matchingService = matchingService;
            _authService = authService;
            _crmSyncService = crmSyncService;
            _logger = logger;
        }

        public async Task<BehavioralProfile?> CreateAsync(BehavioralSurveySubmissionRequest request, Guid userId, CancellationToken cancellationToken = default)
        {
            if (request is null || userId == Guid.Empty) return null;
            request.UserId = userId;

            var normalizedProfile = _matchingService.BuildBehavioralProfile(request);
            var saved = await _behavioralProfileRepository.AddAsync(normalizedProfile, cancellationToken);
            if (saved is not null)
                await TrySyncBehavioralProfileToCrmAsync(saved, userId, cancellationToken).ConfigureAwait(false);

            return saved;
        }

        public async Task<BehavioralProfile?> UpdateFromSurveyAsync(
            BehavioralSurveySubmissionRequest request,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (request is null || userId == Guid.Empty) return null;

            var existing = await _behavioralProfileRepository.GetByUserIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (existing is null) return null;

            request.UserId = userId;
            var normalizedProfile = _matchingService.BuildBehavioralProfile(request);
            normalizedProfile.Id = existing.Id;

            return await UpdateAsync(normalizedProfile, userId, cancellationToken).ConfigureAwait(false);
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

        public async Task<BehavioralProfile?> UpdateAsync(BehavioralProfile profile, Guid userId, CancellationToken cancellationToken = default)
        {
            if (profile is null || profile.Id == Guid.Empty || userId == Guid.Empty)
            {
                return null;
            }

            profile.UserId = userId;
            profile.LastUpdatedAt = DateTime.UtcNow;
            var updated = await _behavioralProfileRepository.UpdateAsync(profile, cancellationToken);
            if (updated is not null)
                await TrySyncBehavioralProfileToCrmAsync(updated, userId, cancellationToken).ConfigureAwait(false);

            return updated;
        }

        private async Task TrySyncBehavioralProfileToCrmAsync(
            BehavioralProfile profile,
            Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(userId).ConfigureAwait(false);
                if (user is null)
                {
                    _logger.LogWarning("CRM sync skipped for behavioral profile: user {UserId} not found.", userId);
                    return;
                }

                var email = user.Email?.Trim();
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("CRM sync skipped for behavioral profile: user {UserId} has no email.", userId);
                    return;
                }

                var fullName = string.IsNullOrWhiteSpace(user.FullName)
                    ? user.UserName ?? string.Empty
                    : user.FullName.Trim();

                var syncRequest = new CrmBehavioralLeadSyncRequest
                {
                    SourceUserId = userId,
                    SourceProfileId = profile.Id,
                    Email = email,
                    FullName = fullName,
                    RiskTolerance = profile.RiskTolerance,
                    PacingStyle = profile.PacingStyle,
                    DisciplineLevel = profile.DisciplineLevel,
                    ConflictStrategy = profile.ConflictStrategy,
                    ProfileUpdatedAt = profile.LastUpdatedAt,
                };

                var result = await _crmSyncService.SyncBehavioralProfileAsync(syncRequest, cancellationToken).ConfigureAwait(false);
                if (!result.Success)
                    _logger.LogWarning("CRM sync failed for behavioral profile: {Message}", result.ErrorMessage);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRM sync threw for behavioral profile (user {UserId}).", userId);
            }
        }

    }
}
