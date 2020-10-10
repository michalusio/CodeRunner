using System.Net;

namespace HTTPBackend
{
    public abstract class BaseMiddleware : IMiddleware
    {
        public IMiddleware Next { protected get; set; }

        public abstract void ResolveRequest(HttpListenerContext context);
    }
}
