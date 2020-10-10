﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace HTTPBackend.Middlewares
{
    public sealed class Authentication : BaseMiddleware
    {
        internal const string SessionCookieName = "Session";

        public static IReadOnlyDictionary<HttpListenerResponse, Session> Data => new ReadOnlyDictionary<HttpListenerResponse, Session>(data);
        private static readonly Dictionary<HttpListenerResponse, Session> data = new Dictionary<HttpListenerResponse, Session>();

        private static readonly List<(Guid Guid, Dictionary<string, string> Data)> sessionNonces = new List<(Guid, Dictionary<string, string>)>();

        internal static void RemoveNonce(string token)
        {
            sessionNonces.RemoveAll(nonce => nonce.Guid.ToString() == token);
        }

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

        public static void SignOut(HttpListenerResponse response)
        {
            var sessionCookie = response.Cookies[SessionCookieName];
            if (sessionCookie != null && !sessionCookie.Expired)
            {
                sessionCookie.Expires = DateTime.Now.AddDays(-1);
                sessionCookie.Expired = true;
                RemoveNonce(sessionCookie.Value);
            }
        }
    }

    public class Session
    {
        public bool IsAuthenticated { get; }
        public Dictionary<string, string> SessionData { get; }

        internal Session(Cookie sessionCookie, List<(Guid Guid, Dictionary<string, string> Data)> nonces)
        {
            if (sessionCookie == null)
            {
                IsAuthenticated = false;
                return;
            }
            if (sessionCookie.Expired)
            {
                IsAuthenticated = false;
                Authentication.RemoveNonce(sessionCookie.Value);
                return;
            }
            foreach (var nonce in nonces)
            {
                if (nonce.Guid.ToString() == sessionCookie.Value)
                {
                    IsAuthenticated = true;
                    SessionData = nonce.Data;
                    Console.WriteLine($"Session {nonce.Guid}");
                    return;
                }
            }
        }
    }
}
