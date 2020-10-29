using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Reactive.Observables
{
    public static partial class ObservableCreationExtensions
    {
        public static Observable<T> Observable<T>(this IEnumerable<T> enumerable) => new FromObservable<T>(enumerable);
        private class FromObservable<T> : Observable<T>
        {
            private DispatcherOperation operation;
            public FromObservable(IEnumerable<T> enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                QueueItem(enumerator);
            }

            private void QueueItem(IEnumerator<T> enumerator)
            {
                if (!enumerator.MoveNext())
                {
                    CompleteAll();
                    return;
                }
                operation = Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    var value = enumerator.Current;
                    foreach (var action in Actions)
                    {
                        action(value);
                    }
                    QueueItem(enumerator);
                }));
            }

            public override void Close()
            {
                operation?.Abort();
                operation = null;
            }
        }
    }
    
}
