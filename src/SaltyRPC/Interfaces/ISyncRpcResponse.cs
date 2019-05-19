using System;

namespace SaltyRPC.Interfaces
{
    public interface ISyncRpcResponse : IAsyncRpcRequest
    {
        Guid RequestId { get; set; }
    }
}