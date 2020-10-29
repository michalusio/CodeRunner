using System;
using System.Net;
using System.Net.Http;
using System.Threading;
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

        private static readonly Mutex mutex = new Mutex(true, "{EC5C5F21-50FE-4574-B199-B8B384A24BC7}");
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                NativeMethods.PostMessage(
                    (IntPtr)NativeMethods.HWND_BROADCAST,
                    NativeMethods.WM_SHOWME,
                    IntPtr.Zero,
                    IntPtr.Zero);
                Environment.Exit(0);
            }
            base.OnStartup(e);
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
                mutex.ReleaseMutex();
            }
        }
    }
}
