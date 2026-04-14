using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;

        public TripService(ITripRepository tripRepository)
        {
            _tripRepository = tripRepository;
        }

        public async Task<Trip?> CreateAsync(Trip trip, CancellationToken ct = default)
        {
            if (trip is null) return null;

            trip.CreatedAt = trip.CreatedAt == default ? DateTime.UtcNow : trip.CreatedAt;

            return await _tripRepository.AddAsync(trip, ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty) return;

            await _tripRepository.DeleteAsync(id, ct);
        }

        public async Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken ct = default)
        {
            return await _tripRepository.GetAllAsync(ct);
        }

        public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty) return null;

            return await _tripRepository.GetByIdAsync(id, ct);
        }

        public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty) return [];

            return await _tripRepository.GetByUserIdAsync(userId, ct);
        }

        public async Task<Trip?> PublishAsync(Guid tripId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null) return null;

            trip.Status = TripStatus.Ongoing;
            return await _tripRepository.UpdateAsync(trip, ct);
        }

        public async Task<Trip?> UnpublishAsync(Guid tripId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null) return null;

            trip.Status = TripStatus.Planned;
            return await _tripRepository.UpdateAsync(trip, ct);
        }

        public async Task<Trip?> UpdateAsync(Trip trip, CancellationToken ct = default)
        {
            if (trip is null || trip.Id == Guid.Empty) return null;

            return await _tripRepository.UpdateAsync(trip, ct);
        }
    }
}
