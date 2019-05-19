using System.Collections.Generic;
using SaltyRPC.Protocol.Attributes;

namespace SaltyRPC.Protocol.RepositoryPacket
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    [PacketRequest(typeof(RepositorySaveRequest<>))]
    public sealed class RepositorySaveRequest<TObject> : BaseRequest
    {
        public sealed class Response : BaseResponse
        {
            public IEnumerable<TObject> Objects { get; set; }
        }

        /// <summary>
        /// if this property is null, means that you want to request every objects
        /// </summary>
        public IEnumerable<TObject> Objects { get; set; }
    }
}