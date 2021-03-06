﻿using System.Collections.Generic;

namespace SaltyRPC.Protocol.RepositoryPacket
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class RepositoryGetRequest<TKey> : BaseRequest
    {
        /// <summary>
        /// if this property is null, means that you want to request every objects
        /// </summary>
        public IEnumerable<TKey> ObjectIds { get; set; }
    }
}