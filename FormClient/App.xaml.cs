using System;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace FormClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        internal static HttpClient HttpClient { get; private set; }
        internal static string ServerIp { get; set; } = "https://localhost:8443/";

        private static HttpClientHandler Handler { get; set; }

        static App()
        {
            Handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                },
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            HttpClient = new HttpClient(Handler);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient?.Dispose();
                HttpClient = null;
                Handler?.Dispose();
                Handler = null;
            }
        }
    }
}
