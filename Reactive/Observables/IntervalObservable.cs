using System;
using System.Windows.Threading;

namespace Reactive.Observables
{
    public static partial class ObservableCreationExtensions
    {
        public static Observable<Unit> Observable(this TimeSpan timeSpan) => new IntervalObservable(timeSpan);

        private class IntervalObservable : Observable<Unit>
        {
            private readonly DispatcherTimer timer;
            public IntervalObservable(TimeSpan timeSpan)
            {
                timer = new DispatcherTimer
                {
                    Interval = timeSpan
                };
                timer.Tick += (s, e) =>
                {
                    foreach (var action in Actions)
                    {
                        action(Unit.Instance);
                    }
                };
                timer.Start();
            }

            public override void Close()
            {
                timer.Stop();
            }
        }
    }
}
