using System;
using System.Collections.Generic;
using System.Linq;

namespace HTTPBackend
{
    public static class HTTPService
    {
        private static readonly List<Type> registeredMiddlewares = new List<Type>();

        public static void RegisterMiddleware<T>() where T: BaseMiddleware
        {
            registeredMiddlewares.Add(typeof(T));
        }

        public static List<IMiddleware> GetMiddlewareStack()
        {
            return registeredMiddlewares.Select(type => Activator.CreateInstance(type) as IMiddleware).ToList();
        }
    }
}
