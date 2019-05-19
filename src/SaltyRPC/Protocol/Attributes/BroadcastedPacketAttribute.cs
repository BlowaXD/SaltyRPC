using System;

namespace SaltyRPC.Protocol.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BroadcastedPacketAttribute : Attribute
    {
        public BroadcastedPacketAttribute(string topic)
        {
            Topic = topic;
        }

        public string Topic { get; }
    }
}