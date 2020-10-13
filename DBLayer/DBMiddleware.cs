using HTTPBackend;
using Effort;
using System.Net;
using Effort.Provider;

namespace DBLayer
{
    public class DBMiddleware : BaseMiddleware
    {
#if (IN_MEMORY_DB)
        private static readonly EffortConnection connection = DbConnectionFactory.CreateTransient();
#endif

        public static CodeRunnerEntities Context { get; private set; }

        public override void ResolveRequest(HttpListenerContext context)
        {
#if (IN_MEMORY_DB)
            using (Context = new CodeRunnerEntities(connection))
            {
                Next.ResolveRequest(context);
            }
#else
            using (Context = new CodeRunnerEntities())
            {
                Next.ResolveRequest(context);
            }
#endif
        }
    }
}
