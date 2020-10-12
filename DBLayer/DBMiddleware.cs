using HTTPBackend;
using System.Net;

namespace DBLayer
{
    public class DBMiddleware : BaseMiddleware
    {
        public static CodeRunnerEntities Context { get; private set; }

        public override void ResolveRequest(HttpListenerContext context)
        {
            using (Context = new CodeRunnerEntities())
            {
                Next.ResolveRequest(context);
            }
        }
    }
}
