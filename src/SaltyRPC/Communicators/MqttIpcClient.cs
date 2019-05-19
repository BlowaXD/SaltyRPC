using System;
using System.Collections.Concurrent;
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
using SaltyRPC.Utils;

namespace SaltyRPC.Communicators
{
    public sealed class MqttIpcClient : IRpcClient
    {
        private readonly IIpcPacketRouter _router;
        private readonly IIpcSerializer _serializer;
        private readonly IPacketContainerFactory _packetFactory;
        private readonly IPendingRequestFactory _requestFactory;
        private readonly ConcurrentDictionary<Guid, PendingRequest> _pendingRequests;
        private readonly HashSet<string> _responsesQueues;
        private readonly IManagedMqttClient _client;
        private readonly string _endPoint;
        private readonly string _name;

        public MqttIpcClient(MqttClientConfigurationBuilder builder, IIpcPacketRouter router, IIpcSerializer serializer) : this(builder.Build(), router, serializer)
        {
        }

        private MqttIpcClient(MqttIpcClientConfiguration conf, IIpcPacketRouter router, IIpcSerializer serializer)
        {
            _router = router;
            _serializer = serializer;
            _endPoint = conf.EndPoint;
            _name = conf.ClientName;
            _responsesQueues = new HashSet<string>();
            _pendingRequests = new ConcurrentDictionary<Guid, PendingRequest>();
            _packetFactory = new PacketContainerFactory();
            _requestFactory = new PendingRequestFactory();
            _client = new MqttFactory().CreateManagedMqttClient(new MqttNetLogger(conf.ClientName));
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(_name)
                    .WithTcpServer(_endPoint)
                    .Build())
                .Build();
            _client.ApplicationMessageReceived += (sender, args) => OnMessage(args.ClientId, args.ApplicationMessage);
            _client.StartAsync(options).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        private void OnMessage(string clientId, MqttApplicationMessage message)
        {
            var container = _serializer.Deserialize<PacketContainer>(message.Payload);
            object packet = JsonConvert.DeserializeObject(container.Content, container.Type);

            if (!(packet is BaseResponse response))
            {
                return;
            }

            if (!_pendingRequests.TryGetValue(response.RequestId, out PendingRequest request))
            {
                return;
            }

            request.Response.SetResult(response);
        }

        private async Task<IRoutingInformation> CheckRouting(Type type)
        {
            IRoutingInformation routingInfos = await _router.GetRoutingInformationsAsync(type);
            if (string.IsNullOrEmpty(routingInfos.OutgoingTopic) || _responsesQueues.Contains(routingInfos.OutgoingTopic))
            {
                return routingInfos;
            }

            await _client.SubscribeAsync(routingInfos.OutgoingTopic);
            _responsesQueues.Add(routingInfos.OutgoingTopic);
            return routingInfos;
        }

        public async Task<TResponse> RequestAsync<TResponse>(ISyncRpcRequest packet) where TResponse : class, ISyncRpcResponse
        {
            // add packet to requests
            PendingRequest request = _requestFactory.Create(packet);
            if (!_pendingRequests.TryAdd(packet.Id, request))
            {
                return null;
            }

            // create the packet container
            PacketContainer container = _packetFactory.ToPacket(packet.GetType(), packet);
            await SendAsync(container);

            ISyncRpcResponse tmp = await request.Response.Task;
            return tmp as TResponse;
        }

        private async Task SendAsync(PacketContainer container)
        {
            IRoutingInformation infos = await CheckRouting(container.Type);
            await _client.PublishAsync(builder =>
                builder
                    .WithPayload(_serializer.Serialize(container))
                    .WithTopic(infos.IncomingTopic)
                    .WithExactlyOnceQoS()
            );
        }

        public Task BroadcastAsync<T>(T packet) where T : IAsyncRpcRequest
        {
            PacketContainer container = _packetFactory.ToPacket(typeof(T), packet);
            return SendAsync(container);
        }
    }
}