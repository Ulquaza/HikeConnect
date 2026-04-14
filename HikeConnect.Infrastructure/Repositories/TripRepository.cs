using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class TripRepository : ITripRepository
    {
        private readonly HikeConnectContext _context;

        public TripRepository(HikeConnectContext context)
        {
            _context = context;
        }

        public async Task<Trip?> AddAsync(Trip trip, CancellationToken ct = default)
        {
            if (trip is null) return null;

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync(ct);

            return trip;
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var trip = await _context.Trips.FirstOrDefaultAsync(x => x.Id == id, ct);

            if (trip is null) return;

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Trip>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Trips.ToListAsync(ct);
        }

        public async Task<Trip?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Trips.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<IReadOnlyList<Trip>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Trips
                .Where(x => x.AuthorId == userId)
                .ToListAsync(ct);
        }

        public async Task<Trip?> UpdateAsync(Trip trip, CancellationToken ct = default)
        {
            if (trip is null) return null;

            _context.Trips.Update(trip);
            await _context.SaveChangesAsync(ct);

            return trip;
        }
    }
}
