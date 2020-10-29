using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Reactive.Observables
{
    public static partial class ObservableCreationExtensions
    {
        public static Observable<HttpResponseMessage> GetObservable(this HttpClient client, string requestUri) => new HttpObservable(client, HttpMethod.Get, requestUri, null);
        public static Observable<HttpResponseMessage> PostObservable(this HttpClient client, string requestUri, HttpContent content) => new HttpObservable(client, HttpMethod.Post, requestUri, content);
        public static Observable<HttpResponseMessage> PutObservable(this HttpClient client, string requestUri, HttpContent content) => new HttpObservable(client, HttpMethod.Put, requestUri, content);
        public static Observable<HttpResponseMessage> DeleteObservable(this HttpClient client, string requestUri) => new HttpObservable(client, HttpMethod.Delete, requestUri, null);

        private enum HttpMethod
        {
            Get,
            Post,
            Put,
            Delete
        }

        private class HttpObservable : Observable<HttpResponseMessage>
        {
            private CancellationTokenSource tokenSource;
            private DispatcherOperation operation;
            public HttpObservable(HttpClient client, HttpMethod method, string requestUri, HttpContent content)
            {
                var requestTask = SendRequestTask(client, method, requestUri, content);
                requestTask.ContinueWith(async (response) =>
                {
                    HttpResponseMessage value = await response;
                    await Dispatcher.CurrentDispatcher.InvokeAsync(() =>
                    {
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                        CompleteAll();
                    }, DispatcherPriority.Normal, tokenSource.Token);
                }, tokenSource.Token, TaskContinuationOptions.NotOnCanceled,TaskScheduler.FromCurrentSynchronizationContext());
            }

            private Task<HttpResponseMessage> SendRequestTask(HttpClient client, HttpMethod method, string requestUri, HttpContent content)
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();
                switch (method)
                {
                    case HttpMethod.Get:
                        return client.GetAsync(requestUri, tokenSource.Token);
                    case HttpMethod.Post:
                        return client.PostAsync(requestUri, content, tokenSource.Token);
                    case HttpMethod.Put:
                        return client.PutAsync(requestUri, content, tokenSource.Token);
                    case HttpMethod.Delete:
                        return client.DeleteAsync(requestUri, tokenSource.Token);
                    default:
                        throw new ArgumentException("The HttpMethod specified is invalid!", nameof(method));
                }
            }

            public override void Close()
            {
                tokenSource?.Cancel();
                tokenSource?.Dispose();
                tokenSource = null;
                operation?.Abort();
                operation = null;
            }
        }
    }
}
