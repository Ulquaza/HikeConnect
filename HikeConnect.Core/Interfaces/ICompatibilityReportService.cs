using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ICompatibilityReportService
    {
        Task<CompatibilityReport?> CreateAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default);

        Task<CompatibilityReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<CompatibilityReport?> GetByUsersIdAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompatibilityReport>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CompatibilityReport>> GetAllAsync(CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
