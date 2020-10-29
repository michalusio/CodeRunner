using Reactive.Observables;
using System;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Filter<T>(this Observable<T> observable, Predicate<T> condition) => new FilterObservable<T>(observable, condition);

        private class FilterObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            public FilterObservable(Observable<T> observable, Predicate<T> condition)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    if (condition(value))
                    {
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
