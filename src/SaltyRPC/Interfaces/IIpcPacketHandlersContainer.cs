using System;
using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface IIpcPacketHandlersContainer
    {
        event EventHandler<Type> Registered;
        event EventHandler<Type> Unregistered;
        Task RegisterAsync(IIpcPacketHandler handler, Type type);

        Task UnregisterAsync(Type type);
        Task HandleAsync(IAsyncRpcRequest request, Type type);
    }
}