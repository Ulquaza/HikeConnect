using HikeConnect.Core.Dtos;
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

        public async Task<Trip?> CreateAsync(CreateTripRequest request, Guid userId, CancellationToken ct = default)
        {
            if (request is null) return null;

            var trip = new Trip
            {
                Name = request.Name,
                Description = request.Description,
                StartAt = ToUtcTimestamp(request.StartAt)
            };

            trip.AuthorId = userId;
            trip.Status = TripStatus.Planned;
            trip.CreatedAt = DateTime.UtcNow;

            return await _tripRepository.AddAsync(trip, ct);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            if (id == Guid.Empty) return;

            var trip = await _tripRepository.GetByIdAsync(id);
            if (trip is null || trip.AuthorId != userId) return;

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

        public async Task<Trip?> PublishAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null || trip.AuthorId != userId) return null;

            trip.Status = TripStatus.Planned;
            return await _tripRepository.UpdateAsync(trip, ct);
        }

        public async Task<Trip?> UnpublishAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null || trip.AuthorId != userId) return null;

            trip.Status = TripStatus.Ongoing;
            return await _tripRepository.UpdateAsync(trip, ct);
        }

        public async Task<Trip?> UpdateAsync(Trip trip, Guid userId, CancellationToken ct = default)
        {
            if (trip is null || trip.Id == Guid.Empty || trip.AuthorId != userId) return null;

            trip.StartAt = ToUtcTimestamp(trip.StartAt);

            return await _tripRepository.UpdateAsync(trip, ct);
        }

        /// <summary>
        /// Npgsql (PostgreSQL timestamptz) принимает только UTC; Blazor InputDate даёт Local.
        /// </summary>
        private static DateTime ToUtcTimestamp(DateTime value) =>
            value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                _ => value.ToUniversalTime()
            };
    }
}
