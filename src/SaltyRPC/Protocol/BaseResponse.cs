﻿using System;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Protocol
{
    public class BaseResponse : ISyncRpcResponse
    {
        private Guid _id;

        public Guid Id
        {
            get => _id == Guid.Empty ? _id = Guid.NewGuid() : _id;
            set => _id = value;
        }

        public string Topic { get; set; }
        public Guid RequestId { get; set; }
    }
}