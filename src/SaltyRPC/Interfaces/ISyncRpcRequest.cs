using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface ISyncRpcRequest : IAsyncRpcRequest
    {
        Task ReplyAsync<T, TRequest>(T response) where T : ISyncRpcResponse;
    }
}