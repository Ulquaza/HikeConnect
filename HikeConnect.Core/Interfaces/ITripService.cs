using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ITripService
    {
        Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken ct = default);
        Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Trip>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task<Trip?> CreateAsync(Trip trip, CancellationToken ct = default);
        Task<Trip?> UpdateAsync(Trip trip, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<Trip?> PublishAsync(Guid tripId, CancellationToken ct = default);
        Task<Trip?> UnpublishAsync(Guid tripId, CancellationToken ct = default);
    }
}
