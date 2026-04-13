using HikeConnect.Core.Entities;
using HikeConnect.Core.Interfaces;
using HikeConnect.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HikeConnect.Infrastructure.Repositories
{
    internal class CompatibilityReportRepository : ICompatibilityReportRepository
    {
        private readonly HikeConnectContext _context;

        public CompatibilityReportRepository(HikeConnectContext context)
        {
            _context = context;
        }

        public async Task<CompatibilityReport?> AddAsync(CompatibilityReport report, CancellationToken cancellationToken = default)
        {
            if (report is null) return null;

            _context.CompatibilityReports.Add(report);
            await _context.SaveChangesAsync(cancellationToken);

            return report;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var report = await _context.CompatibilityReports
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (report is null) return;

            _context.CompatibilityReports.Remove(report);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompatibilityReport>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.CompatibilityReports
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CompatibilityReport>> GetByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
        {
            return await _context.CompatibilityReports
                .AsNoTracking()
                .Where(x => x.AuthorId == authorId)
                .ToListAsync(cancellationToken);
        }

        public async Task<CompatibilityReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.CompatibilityReports
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<CompatibilityReport?> GetByUsersIdAsync(Guid authorId, Guid targetId, CancellationToken cancellationToken = default)
        {
            return await _context.CompatibilityReports
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AuthorId == authorId && x.TargetId == targetId, cancellationToken);
        }
    }
}
