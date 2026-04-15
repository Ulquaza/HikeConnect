using HikeConnect.Core.Dtos;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestService
    {
        Task<ParticipationRequestDto?> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<ParticipationRequestDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequestDto>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequestDto>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<ParticipationRequestDto?> UpdateAsync(Guid requestId, CancellationToken ct = default);

        Task DeleteAsync(Guid requestId, CancellationToken ct = default);


        Task<ParticipationRequestDto?> ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        Task<ParticipationRequestDto?> RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        
        Task<ParticipationRequestDto?> CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default);
    }
}
