using Reactive.Observables;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> TakeUntil<T, V>(this Observable<T> observable, Observable<V> blocker) => new TakeUntilObservable<T, V>(observable, blocker);

        private class TakeUntilObservable<T, V> : Observable<T>
        {
            private readonly Observable<T> observable;
            private bool completed;
            public TakeUntilObservable(Observable<T> observable, Observable<V> blocker)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    foreach (var action in Actions)
                    {
                        action(value);
                    }
                }, CompleteAll);
                blocker.Subscribe(value =>
                {
                    if (!completed)
                    {
                        completed = true;
                        CompleteAll();
                        observable.Close();
                    }
                });
            }

            public override void Close()
            {
                if (!completed)
                {
                    completed = true;
                    CompleteAll();
                    observable.Close();
                }
            }
        }
    }
}
