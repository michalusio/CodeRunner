using Reactive.Observables;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Skip<T>(this Observable<T> observable, int times) => new SkipObservable<T>(observable, times);

        private class SkipObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            public SkipObservable(Observable<T> observable, int times)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    if (times > 0)
                    {
                        times--;
                        return;
                    }
                    foreach (var action in Actions)
                    {
                        action(value);
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
