using System.Text;
using Newtonsoft.Json;

namespace SaltyRPC.Serializers
{
    public class JsonSerializer : IIpcSerializer
    {
        public byte[] Serialize<T>(T packet)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet));
        }

        public T Deserialize<T>(byte[] buffer)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(buffer));
        }
    }
}