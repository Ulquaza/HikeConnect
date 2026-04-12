using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ITripService
    {
        Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Trip>> GetAllAsync(CancellationToken ct = default);

        Task<Guid> CreateAsync(Trip trip, CancellationToken ct = default);
        Task UpdateAsync(Trip trip, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task PublishAsync(Guid tripId, CancellationToken ct = default);
        Task UnpublishAsync(Guid tripId, CancellationToken ct = default);
    }
}
