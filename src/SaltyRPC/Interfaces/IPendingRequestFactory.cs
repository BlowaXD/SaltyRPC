using SaltyRPC.Utils;

namespace SaltyRPC.Interfaces
{
    public interface IPendingRequestFactory
    {
        PendingRequest Create(ISyncRpcRequest request);
    }
}