using SaltyRPC.Interfaces;

namespace SaltyRPC.Routing
{
    public class RoutingInformation : IRoutingInformation
    {
        public string IncomingTopic { get; set; }
        public string OutgoingTopic { get; set; }
    }
}