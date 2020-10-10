using System;
using System.Net;

namespace HTTPBackend
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class AttributeMiddleware : Attribute, IMiddleware
    {
        public IMiddleware Next { private get; set; }

        public abstract void ResolveRequest(HttpListenerContext context);
    }
}
