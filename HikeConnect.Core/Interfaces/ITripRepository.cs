using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface ITripRepository
    {
        Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Trip>> GetAllAsync(CancellationToken ct = default);

        Task<Trip?> AddAsync(Trip trip, CancellationToken ct = default);
        Task<Trip?> UpdateAsync(Trip trip, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
