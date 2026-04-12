using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestService
    {
        Task<Guid> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<User>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IEnumerable<User>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task UpdateAsync(Guid requestId, CancellationToken ct = default);

        Task DeleteAsync(Guid requestId, CancellationToken ct = default);


        Task ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        Task RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default);
        
        Task CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default);
    }
}
