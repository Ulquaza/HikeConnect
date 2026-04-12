using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ITripRepository
    {
        Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Trip>> GetAllAsync(CancellationToken ct = default);

        Task AddAsync(Trip trip, CancellationToken ct = default);
        Task UpdateAsync(Trip trip, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
