﻿namespace SaltyRPC.Configs
{
    public class RabbitMqConfiguration
    {
        public string Address { get; set; }
        public short Port { get; set; }
        public string Password { get; set; }
        public string RequestQueueName { get; set; }
        public string ResponseQueueName { get; set; }
        public string BroadcastQueueName { get; set; }
    }
}