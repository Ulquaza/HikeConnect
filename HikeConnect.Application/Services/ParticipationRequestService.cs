using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class ParticipationRequestService : IParticipationRequestService
    {
        private readonly IParticipationRequestRepository _participationRequestRepository;
        private readonly ITripRepository _tripRepository;

        public ParticipationRequestService(
            IParticipationRequestRepository participationRequestRepository,
            ITripRepository tripRepository)
        {
            _participationRequestRepository = participationRequestRepository;
            _tripRepository = tripRepository;
        }

        public async Task<ParticipationRequest?> ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.Status != ParticipationRequestStatus.Pending) return null;

            var trip = await _tripRepository.GetByIdAsync(request.TripId, ct);
            if (trip is null || trip.AuthorId != organizerId) return null;

            request.Status = ParticipationRequestStatus.Approved;
            return await _participationRequestRepository.UpdateAsync(request, ct);
        }

        public async Task<ParticipationRequest?> CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.UserId != userId) return null;

            if (request.Status == ParticipationRequestStatus.Rejected ||
                request.Status == ParticipationRequestStatus.Canceled)
            {
                return request;
            }

            request.Status = ParticipationRequestStatus.Canceled;
            return await _participationRequestRepository.UpdateAsync(request, ct);
        }

        public async Task<ParticipationRequest?> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty || userId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null || trip.AuthorId == userId) return null;

            var existingRequest = await _participationRequestRepository.GetByTripAndUserAsync(tripId, userId, ct);
            if (existingRequest is not null) return existingRequest;

            var request = new ParticipationRequest
            {
                TripId = tripId,
                UserId = userId,
                Status = ParticipationRequestStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            return await _participationRequestRepository.AddAsync(request, ct);
        }

        public async Task DeleteAsync(Guid requestId, CancellationToken ct = default)
        {
            if (requestId == Guid.Empty) return;

            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null) return;

            await _participationRequestRepository.DeleteAsync(request, ct);
        }

        public Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty) return Task.FromResult<ParticipationRequest?>(null);

            return _participationRequestRepository.GetByIdAsync(id, ct);
        }

        public async Task<IReadOnlyList<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return [];

            return await _participationRequestRepository.GetByTripIdAsync(tripId, ct);
        }

        public async Task<IReadOnlyList<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty) return [];

            return await _participationRequestRepository.GetByUserIdAsync(userId, ct);
        }

        public async Task<ParticipationRequest?> RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.Status != ParticipationRequestStatus.Pending) return null;

            var trip = await _tripRepository.GetByIdAsync(request.TripId, ct);
            if (trip is null || trip.AuthorId != organizerId) return null;

            request.Status = ParticipationRequestStatus.Rejected;
            return await _participationRequestRepository.UpdateAsync(request, ct);
        }

        public async Task<ParticipationRequest?> UpdateAsync(Guid requestId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null) return null;

            if (request.AppliedAt == default)
            {
                request.AppliedAt = DateTime.UtcNow;
            }

            return await _participationRequestRepository.UpdateAsync(request, ct);
        }
    }
}
