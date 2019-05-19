﻿using System;
using Newtonsoft.Json;

namespace SaltyRPC.Protocol
{
    public class PacketContainer
    {
        public Type Type { get; set; }
        public string Content { get; set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}