using HikeConnect.Core.Dtos;
using HikeConnect.Core.Interfaces;

namespace HikeConnect.Infrastructure.Crm
{
    public sealed class CrmSyncService : ICrmSyncService
    {
        private readonly ICrmClient _crmClient;

        public CrmSyncService(ICrmClient crmClient)
        {
            _crmClient = crmClient;
        }

        public Task<CrmOperationResult> SyncBehavioralProfileAsync(
            CrmBehavioralLeadSyncRequest request,
            CancellationToken cancellationToken = default) =>
            _crmClient.UpsertBehavioralLeadAsync(request, cancellationToken);
    }
}
