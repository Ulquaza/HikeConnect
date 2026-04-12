using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IParticipationRequestRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(User request, CancellationToken ct = default);
        Task UpdateAsync(User request, CancellationToken ct = default);
        Task DeleteAsync(User request, CancellationToken ct = default);

        Task<IEnumerable<User>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default);
        Task<IEnumerable<User>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<User?> GetByTripAndUserAsync(Guid tripId, Guid userId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid tripId, Guid userId, CancellationToken ct = default);
    }
}
