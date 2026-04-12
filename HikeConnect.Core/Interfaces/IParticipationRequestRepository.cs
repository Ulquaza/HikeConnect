using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestRepository
    {
        Task<ParticipationRequest?> AddAsync(ParticipationRequest request, CancellationToken ct = default);

        Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequest>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IReadOnlyList<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<ParticipationRequest?> GetByTripAndUserAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<ParticipationRequest?> UpdateAsync(ParticipationRequest request, CancellationToken ct = default);
        Task DeleteAsync(ParticipationRequest request, CancellationToken ct = default);
    }
}
