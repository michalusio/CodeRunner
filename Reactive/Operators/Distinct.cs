using Reactive.Observables;
using System;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Distinct<T>(this Observable<T> observable, Func<T, T, bool> equality = null) => new DistinctObservable<T>(observable, equality);

        private class DistinctObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            private T previousValue = default;
            public DistinctObservable(Observable<T> observable, Func<T, T, bool> equality)
            {
                this.observable = observable;
                if (equality == null) equality = (a, b) => Equals(a, b);

                observable.Subscribe(value =>
                {
                    if (!equality(previousValue, value))
                    {
                        previousValue = value;
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }
                }, CompleteAll);
            }

            public override void Close()
            {
                observable.Close();
            }
        }
    }
}
