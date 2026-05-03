using HikeConnect.Core.Dtos;

namespace HikeConnect.Core.Interfaces
{
    /// <summary>
    /// Outbound port to Twenty Core GraphQL (upsert behavioral lead / custom object record).
    /// </summary>
    public interface ICrmClient
    {
        Task<CrmOperationResult> UpsertBehavioralLeadAsync(
            CrmBehavioralLeadSyncRequest request,
            CancellationToken cancellationToken = default);
    }
}
