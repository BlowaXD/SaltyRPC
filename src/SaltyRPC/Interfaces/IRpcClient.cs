using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface IRpcClient
    {
        Task<T> RequestAsync<T>(ISyncRpcRequest request) where T : class, ISyncRpcResponse;
        Task BroadcastAsync<T>(T packet) where T : IAsyncRpcRequest;
    }
}