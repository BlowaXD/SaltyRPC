using System.Threading.Tasks;

namespace SaltyRPC.Interfaces
{
    public interface IRoutingInformationFactory
    {
        Task<IRoutingInformation> Create(string topic, string responseTopic);
    }
}