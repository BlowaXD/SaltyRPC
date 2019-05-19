﻿namespace SaltyRPC.Configs
{
    public class MqttClientConfigurationBuilder
    {
        private string _endpoint;
        private string _clientName;

        public MqttClientConfigurationBuilder WithName(string name)
        {
            _clientName = name;
            return this;
        }

        public MqttClientConfigurationBuilder ConnectTo(string endpoint)
        {
            _endpoint = endpoint;
            return this;
        }

        public MqttIpcClientConfiguration Build()
        {
            return new MqttIpcClientConfiguration
            {
                ClientName = _clientName,
                EndPoint = _endpoint,
            };
        }

        public static implicit operator MqttIpcClientConfiguration(MqttClientConfigurationBuilder builder)
        {
            return builder.Build();
        }
    }
}