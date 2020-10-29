using Reactive.Observables;
using System.Collections.Generic;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Merge<V, T>(this Observable<V> observable) where V : Observable<T> => new MergeObservable<V, T>(observable);

        private class MergeObservable<V, T> : Observable<T> where V : Observable<T>
        {
            private readonly Observable<V> higherObservable;
            private readonly List<Observable<T>> observables = new List<Observable<T>>();
            private bool higherCompleted;
            private int openObservables;
            public MergeObservable(Observable<V> observable)
            {
                higherObservable = observable;
                observable.Subscribe(observableValue =>
                {
                    observables.Add(observableValue);
                    openObservables++;
                    observableValue.Subscribe(value =>
                    {
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }, () =>
                    {
                        openObservables--;
                        if (higherCompleted && openObservables == 0)
                        {
                            CompleteAll();
                        }
                    });
                }, () =>
                {
                    higherCompleted = true;
                    if (higherCompleted && openObservables == 0)
                    {
                        CompleteAll();
                    }
                });
            }

            public override void Close()
            {
                higherObservable.Close();
                foreach(var obs in observables)
                {
                    obs.Close();
                }
            }
        }
    }
}
