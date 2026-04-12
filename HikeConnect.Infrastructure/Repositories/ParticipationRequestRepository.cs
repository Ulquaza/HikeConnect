using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class ParticipationRequestRepository : IParticipationRequestRepository
    {
        public Task AddAsync(User request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(User request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByTripAndUserAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User request, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
