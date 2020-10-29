using Reactive.Observables;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Delay<T>(this Observable<T> observable, TimeSpan timeSpan) => new DelayObservable<T>(observable, timeSpan);

        private class DelayObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            private readonly Queue<T> values;
            private readonly DispatcherTimer timer;
            private bool completed;
            public DelayObservable(Observable<T> observable, TimeSpan timeSpan)
            {
                this.observable = observable;
                values = new Queue<T>();
                timer = new DispatcherTimer
                {
                    Interval = timeSpan
                };
                timer.Tick += (s, e) =>
                {
                    if (values.Count > 0)
                    {
                        var value = values.Dequeue();
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }
                    else
                    {
                        timer.Stop();
                        if (completed)
                        {
                            CompleteAll();
                        }
                    }
                };
                observable.Subscribe(value =>
                {
                    values.Enqueue(value);
                    if (values.Count == 1)
                    {
                        timer.Start();
                    }
                }, () => completed = true);
            }

            public override void Close()
            {
                timer.Stop();
                observable.Close();
            }
        }
    }
}
