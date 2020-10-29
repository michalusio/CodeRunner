using Reactive.Observables;
using System;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<V> Map<T, V>(this Observable<T> observable, Func<T, V> map) => new MapObservable<T, V>(observable, map);

        private class MapObservable<T, V> : Observable<V>
        {
            private readonly Observable<T> observable;
            public MapObservable(Observable<T> observable, Func<T, V> map)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    var transformed = map(value);
                    foreach (var action in Actions)
                    {
                        action(transformed);
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
