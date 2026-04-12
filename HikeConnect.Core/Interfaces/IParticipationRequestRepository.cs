using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestRepository
    {
        Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<ParticipationRequest>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(ParticipationRequest request, CancellationToken ct = default);
        Task UpdateAsync(ParticipationRequest request, CancellationToken ct = default);
        Task DeleteAsync(ParticipationRequest request, CancellationToken ct = default);

        Task<IEnumerable<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IEnumerable<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<ParticipationRequest?> GetByTripAndUserAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid tripId, Guid userId, CancellationToken ct = default);
    }
}
