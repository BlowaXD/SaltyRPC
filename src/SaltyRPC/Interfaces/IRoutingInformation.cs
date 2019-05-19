namespace SaltyRPC.Interfaces
{
    public interface IRoutingInformation
    {
        string IncomingTopic { get; }
        string OutgoingTopic { get; }
    }
}