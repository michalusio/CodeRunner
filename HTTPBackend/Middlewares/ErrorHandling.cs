using System;
using System.Net;
using System.Text;

namespace HTTPBackend.Middlewares
{
    public class ErrorHandling : BaseMiddleware
    {
        public override void ResolveRequest(HttpListenerContext context)
        {
            try
            {
                Next.ResolveRequest(context);
            }
            catch (Exception e)
            {
                var errorMsg = "ERROR: " + e.ToString();
                Console.WriteLine(errorMsg);
                context.Response.Close(Encoding.UTF8.GetBytes(errorMsg), true);
            }
        }
    }
}
