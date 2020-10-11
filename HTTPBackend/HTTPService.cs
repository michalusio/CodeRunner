using System;
using System.Collections.Generic;
using System.Linq;

namespace HTTPBackend
{
    public static class HTTPService
    {
        private static readonly List<(Type type, object[] args)> registeredMiddlewares = new List<(Type type, object[] args)>();

        public static void RegisterMiddleware<T>(params object[] args) where T : BaseMiddleware
        {
            registeredMiddlewares.Add((typeof(T), args));
        }

        public static List<IMiddleware> GetMiddlewareStack()
        {
            return registeredMiddlewares.Select(data => Activator.CreateInstance(data.type, data.args) as IMiddleware).ToList();
        }
    }
}
