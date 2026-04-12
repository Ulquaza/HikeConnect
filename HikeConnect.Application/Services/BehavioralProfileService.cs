using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class BehavioralProfileService : IBehavioralProfileService
    {
        public Task<BehavioralProfile?> CreateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BehavioralProfile?> UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
