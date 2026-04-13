using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ICompatibilityReportRepository
    {
        Task<CompatibilityReport?> AddAsync(CompatibilityReport report, CancellationToken cancellationToken = default);

        Task<CompatibilityReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CompatibilityReport?> GetByUsersIdAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompatibilityReport>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompatibilityReport>> GetAllAsync(CancellationToken cancellationToken = default);
        
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
