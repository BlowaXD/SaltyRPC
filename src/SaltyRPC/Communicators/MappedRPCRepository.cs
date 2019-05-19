using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaltyRPC.Interfaces;
using SaltyRPC.Protocol.RepositoryPacket;

namespace SaltyRPC.Communicators
{
    public abstract class MappedRpcRepository<T> : IMappedRepository<T> where T : class, IMappedDto
    {
        protected readonly IRpcClient Client;
        protected MappedRpcRepository(IRpcClient client)
        {
            Client = client;
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            return (await Client.RequestAsync<RepositoryGetResponse<T>>(new RepositoryGetRequest<long> { ObjectIds = null })).Objects;
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return (await Client.RequestAsync<RepositoryGetResponse<T>>(new RepositoryGetRequest<long> { ObjectIds = new[] { id } })).Objects.ElementAt(0);
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<long> ids)
        {
            return (await Client.RequestAsync<RepositoryGetResponse<T>>(new RepositoryGetRequest<long> { ObjectIds = ids })).Objects;
        }

        public async Task<T> SaveAsync(T obj)
        {
            return (await Client.RequestAsync<RepositorySaveRequest<T>.Response>(new RepositorySaveRequest<T> { Objects = new[] { obj } })).Objects.ElementAt(0);
        }

        public async Task SaveAsync(IEnumerable<T> objs)
        {
            await Client.RequestAsync<RepositorySaveRequest<T>.Response>(new RepositorySaveRequest<T> { Objects = objs });
        }

        public async Task DeleteByIdAsync(long id)
        {
            await Client.BroadcastAsync(new RepositoryDeleteRequest<long> { ObjectIds = new[] { id } });
        }

        public async Task DeleteByIdsAsync(IEnumerable<long> ids)
        {
            await Client.BroadcastAsync(new RepositoryDeleteRequest<long> { ObjectIds = ids });
        }
    }
}