using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class BehavioralProfileRepository : IBehavioralProfileRepository
    {
        public Task AddAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
