using System;

namespace SaltyRPC.Interfaces
{
    public interface ISynchronizedRepository<T> : IAsyncRepository<T, Guid> where T : class, ISynchronizedDto
    {
    }
}