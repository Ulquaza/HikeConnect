using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class TripRepository : ITripRepository
    {
        public Task AddAsync(Trip trip, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trip>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Trip trip, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
