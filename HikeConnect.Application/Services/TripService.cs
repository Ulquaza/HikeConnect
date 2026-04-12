using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class TripService : ITripService
    {
        public Task<Trip?> CreateAsync(Trip trip, CancellationToken ct = default)
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

        public Task<Trip?> PublishAsync(Guid tripId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Trip?> UnpublishAsync(Guid tripId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Trip?> UpdateAsync(Trip trip, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
