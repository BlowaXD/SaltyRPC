using System.Threading.Tasks;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Utils
{
    public class PendingRequest
    {
        public ISyncRpcRequest Request { get; set; }
        public TaskCompletionSource<ISyncRpcResponse> Response { get; set; }
    }
}