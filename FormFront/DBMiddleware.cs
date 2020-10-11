using DBLayer;
using HTTPBackend;
using System.Net;

namespace FormFront
{
    internal class DBMiddleware : BaseMiddleware
    {
        public static CodeRunnerEntities Context;

        public override void ResolveRequest(HttpListenerContext context)
        {
            using (Context = new CodeRunnerEntities())
            {
                Next.ResolveRequest(context);
            }
        }
    }
}
