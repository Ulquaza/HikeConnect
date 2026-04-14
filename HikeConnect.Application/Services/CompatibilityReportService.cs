using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class CompatibilityReportService : ICompatibilityReportService
    {
        private readonly ICompatibilityReportRepository _compatibilityReportRepository;
        private readonly IBehavioralProfileRepository _behavioralProfileRepository;
        private readonly IMatchingService _matchingService;

        public CompatibilityReportService(
            ICompatibilityReportRepository compatibilityReportRepository,
            IBehavioralProfileRepository behavioralProfileRepository,
            IMatchingService matchingService)
        {
            _compatibilityReportRepository = compatibilityReportRepository;
            _behavioralProfileRepository = behavioralProfileRepository;
            _matchingService = matchingService;
        }

        public async Task<CompatibilityReport?> CreateAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default)
        {
            if (authorId == Guid.Empty || targetId == Guid.Empty || authorId == targetId)
            {
                return null;
            }

            var authorProfile = await _behavioralProfileRepository.GetByUserIdAsync(authorId, cancellationToken);
            var targetProfile = await _behavioralProfileRepository.GetByUserIdAsync(targetId, cancellationToken);
            if (authorProfile is null || targetProfile is null)
            {
                return null;
            }

            var existingReport = await _compatibilityReportRepository.GetByUsersIdAsync(authorId, targetId, cancellationToken);
            if (existingReport is not null)
            {
                await _compatibilityReportRepository.DeleteAsync(existingReport.Id, cancellationToken);
            }

            var report = _matchingService.BuildCompatibilityReport(authorProfile, targetProfile);
            report.CreatedAt = DateTime.UtcNow;

            return await _compatibilityReportRepository.AddAsync(report, cancellationToken);
        }

        public async Task<CompatibilityReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) return null;

            return await _compatibilityReportRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<CompatibilityReport?> GetByUsersIdAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default)
        {
            if (authorId == Guid.Empty || targetId == Guid.Empty) return null;

            var report = await _compatibilityReportRepository.GetByUsersIdAsync(authorId, targetId, cancellationToken);
            if (report is null) return null;

            if (DateTime.UtcNow - report.CreatedAt >= TimeSpan.FromDays(7))
            {
                return await CreateAsync(authorId, targetId, cancellationToken);
            }

            return report;
        }

        public async Task<IReadOnlyList<CompatibilityReport>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            if (authorId == Guid.Empty) return [];

            return await _compatibilityReportRepository.GetByAuthorIdAsync(authorId, cancellationToken);
        }

        public async Task<IReadOnlyList<CompatibilityReport>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _compatibilityReportRepository.GetAllAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty) return;

            await _compatibilityReportRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
