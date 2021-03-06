﻿using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SaltyRPC.Configs;
using SaltyRPC.Interfaces;
using SaltyRPC.Protocol;

namespace SaltyRPC.Communicators
{
    public abstract class RabbitMqServer : IRpcServer
    {
        private readonly RabbitMqConfiguration _configuration;
        private readonly string _requestQueueName;
        private readonly string _responseQueueName;
        private readonly string _broadcastQueueName;
        private const string ExchangeName = ""; // default exchange
        private readonly IModel _channel;

        private readonly IConnection _connection;
        private readonly IPacketContainerFactory _packetContainerFactory;

        private readonly IIpcPacketHandlersContainer _packetHandlersContainer;

        protected RabbitMqServer(RabbitMqConfiguration config, IIpcPacketHandlersContainer packetHandlersContainer)
        {
            _configuration = config;
            _packetHandlersContainer = packetHandlersContainer;
            var factory = new ConnectionFactory { HostName = _configuration.Address, Password = _configuration.Password, Port = _configuration.Port };

            _requestQueueName = _configuration.RequestQueueName;
            _responseQueueName = _configuration.ResponseQueueName;
            _broadcastQueueName = _configuration.BroadcastQueueName;

            _packetContainerFactory = new PacketContainerFactory();

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(_requestQueueName, true, false, false, null);
            _channel.QueueDeclare(_responseQueueName, true, false, false, null);
            _channel.QueueDeclare(_broadcastQueueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnMessage;
            _channel.BasicConsume(_requestQueueName, true, consumer);
            _channel.BasicConsume(_broadcastQueueName, true, consumer);
        }

        public Task ResponseAsync<T>(T response) where T : ISyncRpcResponse
        {
            return Task.Run(() => { Publish(_packetContainerFactory.ToPacket<T>(response), _responseQueueName); });
        }

        private void OnMessage(object sender, BasicDeliverEventArgs e)
        {
            string requestMessage = Encoding.UTF8.GetString(e.Body);
            var packet = JsonConvert.DeserializeObject<PacketContainer>(requestMessage);

            if (e.BasicProperties.ReplyTo == _requestQueueName)
            {
                object ipc = JsonConvert.DeserializeObject(packet.Content, packet.Type);
                OnRequest(ipc as BaseRequest, packet.Type);
            }
            else if (e.BasicProperties.ReplyTo == _broadcastQueueName)
            {
                return;
            }
        }

        public void OnRequest(BaseRequest request, Type type)
        {
            request.Server = this;
            _packetHandlersContainer.HandleAsync(request, type).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }

        private void Publish(PacketContainer container, string queueName)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(container.ToString());
            IBasicProperties props = _channel.CreateBasicProperties();
            props.ReplyTo = queueName;
            _channel.BasicPublish(ExchangeName, queueName, props, messageBytes);
        }

        public Task ResponseAsync<T>(T response, Type requestType) where T : ISyncRpcResponse
        {
            return Task.Run(() => { Publish(_packetContainerFactory.ToPacket<T>(response), _responseQueueName); });
        }
    }
}