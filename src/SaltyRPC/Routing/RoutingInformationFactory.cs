using System.Threading.Tasks;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Routing
{
    public class RoutingInformationFactory : IRoutingInformationFactory
    {
        public Task<IRoutingInformation> Create(string topic, string responseTopic)
        {
            return Task.FromResult((IRoutingInformation)new RoutingInformation
            {
                IncomingTopic = topic,
                OutgoingTopic = responseTopic
            });
        }
    }
}