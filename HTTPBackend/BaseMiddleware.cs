using System;
using System.Collections.Generic;
using System.Net;

namespace HTTPBackend
{
    public abstract class BaseMiddleware : IMiddleware
    {
        protected internal IEnumerable<Attribute> ControllerAttributes { get; internal set; }
        public IMiddleware Next { protected get; set; }

        public abstract void ResolveRequest(HttpListenerContext context);
    }
}
