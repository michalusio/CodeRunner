using System;
using System.Collections.Generic;
using System.Net;

namespace HTTPBackend
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class AttributeMiddleware : Attribute, IMiddleware
    {
        protected internal IEnumerable<Attribute> ControllerAttributes { get; internal set; }
        public IMiddleware Next { private get; set; }

        public abstract void ResolveRequest(HttpListenerContext context);
    }
}
