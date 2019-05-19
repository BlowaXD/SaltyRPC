using System;
using SaltyRPC.Protocol;

namespace SaltyRPC.Interfaces
{
    public interface IPacketContainerFactory
    {
        PacketContainer Create<T>(string content);
        PacketContainer Create(Type type, string content);

        PacketContainer ToPacket<T>(T packet) where T : IAsyncRpcRequest;
        PacketContainer ToPacket(Type type, IAsyncRpcRequest packet);
    }
}