using System;
using Newtonsoft.Json;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Protocol
{
    public class PacketContainerFactory : IPacketContainerFactory
    {
        public PacketContainer Create<T>(string content) => Create(typeof(T), content);

        public PacketContainer Create(Type type, string content) =>
            new PacketContainer
            {
                Type = type,
                Content = content
            };

        public PacketContainer ToPacket<T>(T packet) where T : IAsyncRpcRequest => ToPacket(typeof(T), packet);

        public PacketContainer ToPacket(Type type, IAsyncRpcRequest packet) => Create(type, JsonConvert.SerializeObject(packet));
    }
}