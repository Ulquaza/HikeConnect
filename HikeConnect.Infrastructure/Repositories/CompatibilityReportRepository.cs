using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class CompatibilityReportRepository : ICompatibilityReportRepository
    {
        public Task AddAsync(CompatibilityReport report, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompatibilityReport>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompatibilityReport>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CompatibilityReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CompatibilityReport?> GetByUsersIdAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
