using System.Threading.Tasks;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Utils
{
    public class PendingRequestFactory : IPendingRequestFactory
    {
        public PendingRequest Create(ISyncRpcRequest request) => new PendingRequest
        {
            Request = request,
            Response = new TaskCompletionSource<ISyncRpcResponse>()
        };
    }
}