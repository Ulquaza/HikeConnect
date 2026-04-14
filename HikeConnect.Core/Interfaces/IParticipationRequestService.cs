using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestService
    {
        Task<ParticipationRequest?> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<ParticipationRequest?> UpdateAsync(Guid requestId, CancellationToken ct = default);

        Task DeleteAsync(Guid requestId, CancellationToken ct = default);


        Task<ParticipationRequest?> ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        Task<ParticipationRequest?> RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        
        Task<ParticipationRequest?> CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default);
    }
}
