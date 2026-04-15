using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class ParticipationRequestRepository : IParticipationRequestRepository
    {
        private readonly HikeConnectContext _context;

        public ParticipationRequestRepository(HikeConnectContext context)
        {
            _context = context;
        }

        public async Task<ParticipationRequest?> AddAsync(ParticipationRequest request, CancellationToken ct = default)
        {
            if (request is null) return null;

            _context.ParticipationRequests.Add(request);
            await _context.SaveChangesAsync(ct);

            return request;
        }

        public async Task DeleteAsync(ParticipationRequest request, CancellationToken ct = default)
        {
            if (request is null) return;

            _context.ParticipationRequests.Remove(request);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .AnyAsync(x => x.TripId == tripId && x.UserId == userId, ct);
        }

        public async Task<IReadOnlyList<ParticipationRequest>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<ParticipationRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<ParticipationRequest?> GetByTripAndUserAsync(Guid tripId, Guid userId, CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.TripId == tripId && x.UserId == userId)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<ParticipationRequest>> GetByTripIdAsync(Guid tripId, CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.TripId == tripId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ParticipationRequest>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.ParticipationRequests
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .ToListAsync(ct);
        }

        public async Task<ParticipationRequest?> UpdateAsync(ParticipationRequest request, CancellationToken ct = default)
        {
            if (request is null) return null;

            _context.ParticipationRequests.Update(request);
            await _context.SaveChangesAsync(ct);

            return request;
        }
    }
}
