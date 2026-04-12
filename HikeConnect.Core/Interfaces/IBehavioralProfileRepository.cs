using HikeConnect.Core.Entities;

namespace HikeConnect.Core.Interfaces
{
    public interface IBehavioralProfileRepository
    {
        Task AddAsync(BehavioralProfile profile, CancellationToken cancellationToken = default);

        Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
