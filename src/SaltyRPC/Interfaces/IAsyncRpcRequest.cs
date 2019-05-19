using System;

namespace SaltyRPC.Interfaces
{
    public interface IAsyncRpcRequest
    {
        Guid Id { get; set; }
    }
}