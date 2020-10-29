using Reactive.Observables;
using System;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<(T, V)> WithLatestFrom<T, V>(this Observable<T> observable, Observable<V> second) => new WithLatestFromObservable<T, V, (T, V)>(observable, second, (t, v) => (t, v));
        public static Observable<U> WithLatestFrom<T, V, U>(this Observable<T> observable, Observable<V> second, Func<T, V, U> projection) => new WithLatestFromObservable<T, V, U>(observable, second, projection);

        private class WithLatestFromObservable<T, V, U> : Observable<U>
        {
            private readonly Observable<T> observable;
            private V latestSecondValue;
            private bool secondStarted;
            public WithLatestFromObservable(Observable<T> observable, Observable<V> second, Func<T, V, U> projection)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    if (secondStarted)
                    {
                        var transformed = projection(value, latestSecondValue);
                        foreach (var action in Actions)
                        {
                            action(transformed);
                        }
                    }
                }, CompleteAll);
                second.Subscribe(value =>
                {
                    secondStarted = true;
                    latestSecondValue = value;
                });
            }

            public override void Close()
            {
                observable.Close();
            }
        }
    }
}
