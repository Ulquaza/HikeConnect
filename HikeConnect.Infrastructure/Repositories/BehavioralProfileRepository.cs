using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class BehavioralProfileRepository : IBehavioralProfileRepository
    {
        private readonly HikeConnectContext _context;

        public BehavioralProfileRepository(HikeConnectContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            if (profile is null) return;

            _context.BehavioralProfiles.Add(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var profile = await _context.BehavioralProfiles
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (profile is null) return;

            _context.BehavioralProfiles.Remove(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<BehavioralProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.BehavioralProfiles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<BehavioralProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.BehavioralProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<BehavioralProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.BehavioralProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }

        public async Task UpdateAsync(BehavioralProfile profile, CancellationToken cancellationToken = default)
        {
            if (profile is null) return;

            _context.BehavioralProfiles.Update(profile);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
