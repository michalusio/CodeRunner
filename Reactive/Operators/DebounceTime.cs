using Reactive.Observables;
using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> DebounceTime<T>(this Observable<T> observable, TimeSpan timeSpan) => new DebounceTimeObservable<T>(observable, timeSpan);

        private class DebounceTimeObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            private T currentValue;
            private readonly DispatcherTimer timer;
            private bool completed;
            public DebounceTimeObservable(Observable<T> observable, TimeSpan timeSpan)
            {
                this.observable = observable;
                timer = new DispatcherTimer
                {
                    Interval = timeSpan
                };
                timer.Tick += (s, e) =>
                {
                    foreach (var action in Actions)
                    {
                        action(currentValue);
                    }
                    timer.Stop();
                    if (completed)
                    {
                        CompleteAll();
                    }
                };
                observable.Subscribe(value =>
                {
                    currentValue = value;
                    timer.Stop();
                    timer.Start();
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
