using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Application.Services
{
    public class ParticipationRequestService : IParticipationRequestService
    {
        public Task<ParticipationRequest?> ApproveAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipationRequest?> CancelAsync(Guid requestId, Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipationRequest?> CreateAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid requestId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipationRequest?> RejectAsync(Guid requestId, Guid organizerId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ParticipationRequest?> UpdateAsync(Guid requestId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
