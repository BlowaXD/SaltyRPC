namespace SaltyRPC.Interfaces
{
    public interface IMappedRepository<T> : IAsyncRepository<T, long> where T : class, IMappedDto
    {
    }
}