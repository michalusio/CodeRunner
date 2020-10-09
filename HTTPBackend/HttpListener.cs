using Backend;
using System;
using System.Threading;

namespace HTTPBackend
{
    public class HttpListener
    {
        private const int PORT = 8443;

        private readonly System.Net.HttpListener listener;
        private Thread listenerThread;
        private readonly BaseController responses;
        private readonly ILogger Logger;

        public HttpListener(BaseController responseController, ILogger logger)
        {
            Logger = logger;
            listener = new System.Net.HttpListener();
            listener.Prefixes.Add($"https://localhost:{PORT}/");
            responses = responseController;
            Logger.OuterLevelWrite("HTTP", () => Logger.Log($"Awaiting requests on port {PORT}"));
        }

        public void StartListener()
        {
            listenerThread = new Thread(ListeningAction)
            {
                IsBackground = true
            };
            listenerThread.Start();
        }

        private void ListeningAction()
        {
            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var method = context.Request.HttpMethod;
                var url = context.Request.RawUrl;
                try
                {
                    Logger.OuterLevelWrite("HTTP", () => Logger.Log($"Got request: {method};{context.Request.RawUrl}"));
                    if (!responses.ResolveRequest(context))
                    {
                        Logger.OuterLevelWrite("HTTP", () => Logger.Log($"Web Error: Cannot get response action for ({method};{url})"));
                    }
                }
                catch (Exception e)
                {
                    Logger.OuterLevelWrite("HTTP", () => Logger.Log($"Web Error: {e}"));
                }

            }
        }
    }
}
