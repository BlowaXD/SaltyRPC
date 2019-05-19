using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface IIpcPacketHandler
    {
        Task Handle(IAsyncRpcRequest packet);
    }
}