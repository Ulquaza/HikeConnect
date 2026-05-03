using HikeConnect.Core.Dtos;

namespace HikeConnect.Core.Interfaces
{
    /// <summary>
    /// Application-facing CRM sync after domain events (e.g. behavioral profile saved).
    /// </summary>
    public interface ICrmSyncService
    {
        Task<CrmOperationResult> SyncBehavioralProfileAsync(
            CrmBehavioralLeadSyncRequest request,
            CancellationToken cancellationToken = default);
    }
}
