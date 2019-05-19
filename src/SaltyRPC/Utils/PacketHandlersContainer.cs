using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Utils
{
    public sealed class PacketHandlersContainer : IIpcPacketHandlersContainer
    {
        private readonly Dictionary<Type, IIpcPacketHandler> _packetHandlers = new Dictionary<Type, IIpcPacketHandler>();

        public event EventHandler<Type> Registered;
        public event EventHandler<Type> Unregistered;

        public Task RegisterAsync(IIpcPacketHandler handler, Type type)
        {
            if (_packetHandlers.ContainsKey(type))
            {
                return Task.CompletedTask;
            }

            _packetHandlers.Add(type, handler);
            OnRegistered(type);
            return Task.CompletedTask;
        }

        public async Task HandleAsync(IAsyncRpcRequest request, Type type)
        {
            if (!_packetHandlers.TryGetValue(type, out IIpcPacketHandler handler))
            {
                return;
            }

            await handler.Handle(request);
        }

        public Task UnregisterAsync(Type type)
        {
            if (_packetHandlers.ContainsKey(type))
            {
                OnUnregistered(type);
                _packetHandlers.Remove(type);
            }

            return Task.CompletedTask;
        }

        private void OnRegistered(Type e)
        {
            Registered?.Invoke(this, e);
        }

        private void OnUnregistered(Type e)
        {
            Unregistered?.Invoke(this, e);
        }
    }
}