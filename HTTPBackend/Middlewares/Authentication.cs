using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace HTTPBackend.Middlewares
{
    public class Authentication : BaseMiddleware
    {
        internal const string SessionCookieName = "Session";

        public static IReadOnlyDictionary<HttpListenerResponse, Session> Data => new ReadOnlyDictionary<HttpListenerResponse, Session>(data);
        private static readonly Dictionary<HttpListenerResponse, Session> data = new Dictionary<HttpListenerResponse, Session>();

        private static readonly List<(Guid, Dictionary<string, string>)> sessionNonces = new List<(Guid, Dictionary<string, string>)>();

        public override void ResolveRequest(HttpListenerContext context)
        {
            data[context.Response] = new Session(context.Request.Cookies[SessionCookieName], sessionNonces);
            try
            {
                Next.ResolveRequest(context);
            }
            finally
            {
                data.Remove(context.Response);
            }
        }

        public static void SignIn(HttpListenerResponse response)
        {
            var nonce = Guid.NewGuid();
            sessionNonces.Add((nonce, new Dictionary<string, string>()));
            var cookie = new Cookie
            {
                Name = SessionCookieName,
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddHours(1),
                Value = nonce.ToString()
            };
            response.SetCookie(cookie);
        }
    }

    public class Session
    {
        public bool IsAuthenticated { get; }
        public Dictionary<string, string> SessionData { get; }

        internal Session(Cookie sessionCookie, List<(Guid, Dictionary<string, string>)> nonces)
        {
            if (sessionCookie == null || sessionCookie.Expired)
            {
                IsAuthenticated = false;
                return;
            }
            foreach (var nonce in nonces)
            {
                if (nonce.Item1.ToString() == sessionCookie.Value)
                {
                    IsAuthenticated = true;
                    SessionData = nonce.Item2;
                    Console.WriteLine($"Session {nonce.Item1}");
                    return;
                }
            }
        }
    }
}
