using System.Threading.Tasks;
using SaltyRPC.Interfaces;

namespace SaltyRPC.Handlers
{
    public abstract class GenericIpcRequestHandler<TRequest, TResponse> : IIpcPacketHandler where TRequest : ISyncRpcRequest
                                                                                            where TResponse : ISyncRpcResponse
    {
        public async Task Handle(IAsyncRpcRequest packet)
        {
            if (packet is TRequest request)
            {
                await request.ReplyAsync<TResponse, TRequest>(await Handle(request));
                return;
            }
        }

        protected abstract Task<TResponse> Handle(TRequest request);
    }
}