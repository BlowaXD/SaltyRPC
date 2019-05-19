using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using SaltyRPC.Configs;
using SaltyRPC.Interfaces;
using SaltyRPC.Protocol;
using SaltyRPC.Serializers;

namespace SaltyRPC.Communicators
{
    public class MqttIpcServer : IRpcServer
    {
        private readonly HashSet<string> _queues;
        private readonly IManagedMqttClient _client;
        private readonly IIpcSerializer _serializer;
        private readonly IPacketContainerFactory _packetFactory;

        // handling
        private readonly IIpcPacketHandlersContainer _packetHandlersContainer;
        private readonly IIpcPacketRouter _router;

        public MqttIpcServer(MqttServerConfigurationBuilder builder, IIpcSerializer serializer, IIpcPacketRouter router, IIpcPacketHandlersContainer packetHandlersContainer) : this(
            builder.Build(),
            serializer, router, packetHandlersContainer)
        {
        }

        private MqttIpcServer(MqttIpcServerConfiguration configuration, IIpcSerializer serializer, IIpcPacketRouter router, IIpcPacketHandlersContainer packetHandlersContainer)
        {
            _serializer = serializer;
            _router = router;
            _packetHandlersContainer = packetHandlersContainer;
            string clientName = configuration.ClientName;
            string endPoint = configuration.EndPoint;


            _client = new MqttFactory().CreateManagedMqttClient(new MqttNetLogger(clientName));
            _packetFactory = new PacketContainerFactory();
            _queues = new HashSet<string>();
            _packetHandlersContainer.Registered += (sender, type) => CheckRouting(type).ConfigureAwait(false).GetAwaiter().GetResult();
            _packetHandlersContainer.Unregistered += (sender, type) =>
            {
                IRoutingInformation infos = _router.GetRoutingInformationsAsync(type).ConfigureAwait(false).GetAwaiter().GetResult();
                _client.UnsubscribeAsync(infos.IncomingTopic).ConfigureAwait(false).GetAwaiter().GetResult();
            };
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(clientName)
                    .WithTcpServer(endPoint)
                    .Build())
                .Build();
            _client.ApplicationMessageReceived += (sender, args) => OnMessage(args.ClientId, args.ApplicationMessage);
            _client.StartAsync(options).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void OnMessage(string clientId, MqttApplicationMessage message)
        {
            var container = _serializer.Deserialize<PacketContainer>(message.Payload);
            object packet = JsonConvert.DeserializeObject(container.Content, container.Type);

            switch (packet)
            {
                case BaseRequest request:
                    OnRequest(request, container.Type);
                    break;
            }
        }

        public void OnRequest(BaseRequest request, Type type)
        {
            request.Server = this;
            _packetHandlersContainer.HandleAsync(request, type).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task ResponseAsync<T>(T response, Type packetType) where T : ISyncRpcResponse
        {
            PacketContainer container = _packetFactory.ToPacket(response);
            IRoutingInformation routingInfos = await CheckRouting(packetType);
            await SendAsync(container, routingInfos);
        }


        private async Task<IRoutingInformation> CheckRouting(Type type)
        {
            IRoutingInformation routingInfos = await _router.GetRoutingInformationsAsync(type);
            if (string.IsNullOrEmpty(routingInfos.IncomingTopic) || _queues.Contains(routingInfos.IncomingTopic))
            {
                return routingInfos;
            }

            await _client.SubscribeAsync(routingInfos.IncomingTopic);
            _queues.Add(routingInfos.IncomingTopic);
            return routingInfos;
        }

        private async Task SendAsync(PacketContainer container, IRoutingInformation routingInfos)
        {
            await _client.PublishAsync(builder => builder
                .WithPayload(_serializer.Serialize(container))
                .WithTopic(routingInfos.OutgoingTopic)
                .WithExactlyOnceQoS());
        }
    }
}