using System.Net;

namespace HTTPBackend
{
    public interface IMiddleware
    {
        IMiddleware Next { set; }
        void ResolveRequest(HttpListenerContext context);
    }
}
