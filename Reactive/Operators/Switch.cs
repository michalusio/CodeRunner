using Reactive.Observables;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Switch<V, T>(this Observable<V> observable) where V : Observable<T> => new SwitchObservable<V, T>(observable);

        private class SwitchObservable<V, T> : Observable<T> where V : Observable<T>
        {
            private readonly Observable<V> higherObservable;
            private Observable<T> currentObservable;
            public SwitchObservable(Observable<V> observable)
            {
                this.higherObservable = observable;
                observable.Subscribe(observableValue =>
                {
                    currentObservable?.Close();
                    currentObservable = observableValue;
                    observableValue.Subscribe(value =>
                    {
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }, CompleteAll);
                });
            }

            public override void Close()
            {
                higherObservable.Close();
                currentObservable?.Close();
            }
        }
    }
}
