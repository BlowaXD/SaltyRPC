namespace SaltyRPC.Serializers
{
    public interface IIpcSerializer
    {
        byte[] Serialize<T>(T packet);

        T Deserialize<T>(byte[] buffer);
    }
}