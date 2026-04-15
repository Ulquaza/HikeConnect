using HikeConnect.Core.Dtos;
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

        public async Task<ParticipationRequestDto?> ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.Status != ParticipationRequestStatus.Pending) return null;

            var trip = await _tripRepository.GetByIdAsync(request.TripId, ct);
            if (trip is null || trip.AuthorId != organizerId) return null;

            request.Status = ParticipationRequestStatus.Approved;
            var updated = await _participationRequestRepository.UpdateAsync(request, ct);
            return MapToDto(updated);
        }

        public async Task<ParticipationRequestDto?> CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.UserId != userId) return null;

            if (request.Status == ParticipationRequestStatus.Rejected ||
                request.Status == ParticipationRequestStatus.Canceled)
            {
                return MapToDto(request);
            }

            request.Status = ParticipationRequestStatus.Canceled;
            var updated = await _participationRequestRepository.UpdateAsync(request, ct);
            return MapToDto(updated);
        }

        public async Task<ParticipationRequestDto?> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty || userId == Guid.Empty) return null;

            var trip = await _tripRepository.GetByIdAsync(tripId, ct);
            if (trip is null || trip.AuthorId == userId) return null;

            var existingRequest = await _participationRequestRepository.GetByTripAndUserAsync(tripId, userId, ct);
            if (existingRequest is not null) return MapToDto(existingRequest);

            var request = new ParticipationRequest
            {
                TripId = tripId,
                UserId = userId,
                Status = ParticipationRequestStatus.Pending,
                AppliedAt = DateTime.UtcNow
            };

            var created = await _participationRequestRepository.AddAsync(request, ct);
            return MapToDto(created);
        }

        public async Task DeleteAsync(Guid requestId, CancellationToken ct = default)
        {
            if (requestId == Guid.Empty) return;

            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null) return;

            await _participationRequestRepository.DeleteAsync(request, ct);
        }

        public async Task<ParticipationRequestDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty) return null;

            var request = await _participationRequestRepository.GetByIdAsync(id, ct);
            return MapToDto(request);
        }

        public async Task<IReadOnlyList<ParticipationRequestDto>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        {
            if (tripId == Guid.Empty) return [];

            var requests = await _participationRequestRepository.GetByTripIdAsync(tripId, ct);
            return requests.Select(MapToDto).Where(dto => dto is not null).Cast<ParticipationRequestDto>().ToList();
        }

        public async Task<IReadOnlyList<ParticipationRequestDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            if (userId == Guid.Empty) return [];

            var requests = await _participationRequestRepository.GetByUserIdAsync(userId, ct);
            return requests.Select(MapToDto).Where(dto => dto is not null).Cast<ParticipationRequestDto>().ToList();
        }

        public async Task<ParticipationRequestDto?> RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null || request.Status != ParticipationRequestStatus.Pending) return null;

            var trip = await _tripRepository.GetByIdAsync(request.TripId, ct);
            if (trip is null || trip.AuthorId != organizerId) return null;

            request.Status = ParticipationRequestStatus.Rejected;
            var updated = await _participationRequestRepository.UpdateAsync(request, ct);
            return MapToDto(updated);
        }

        public async Task<ParticipationRequestDto?> UpdateAsync(Guid requestId, CancellationToken ct = default)
        {
            var request = await _participationRequestRepository.GetByIdAsync(requestId, ct);
            if (request is null) return null;

            if (request.AppliedAt == default)
            {
                request.AppliedAt = DateTime.UtcNow;
            }

            var updated = await _participationRequestRepository.UpdateAsync(request, ct);
            return MapToDto(updated);
        }

        private static ParticipationRequestDto? MapToDto(ParticipationRequest? request)
        {
            if (request is null)
            {
                return null;
            }

            return new ParticipationRequestDto
            {
                Id = request.Id,
                TripId = request.TripId,
                UserId = request.UserId,
                Status = request.Status,
                AppliedAt = request.AppliedAt,
                UserFullName = request.User?.FullName,
                UserName = request.User?.UserName
            };
        }
    }
}
