using System;
using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface IRpcServer
    {
        Task ResponseAsync<T>(T response, Type requestType) where T : ISyncRpcResponse;
    }
}