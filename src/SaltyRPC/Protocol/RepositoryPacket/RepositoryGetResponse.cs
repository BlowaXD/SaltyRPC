using System.Collections.Generic;

namespace SaltyRPC.Protocol.RepositoryPacket
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public sealed class RepositoryGetResponse<TObject> : BaseResponse
    {
        public IEnumerable<TObject> Objects { get; set; }
    }
}