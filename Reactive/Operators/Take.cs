using Reactive.Observables;
using System.ComponentModel;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Take<T>(this Observable<T> observable, int times) => new TakeObservable<T>(observable, times);

        private class TakeObservable<T> : Observable<T>
        {
            private readonly Observable<T> observable;
            private bool completed;
            public TakeObservable(Observable<T> observable, int times)
            {
                this.observable = observable;
                observable.Subscribe(value =>
                {
                    if (times > 0)
                    {
                        times--;
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }
                    else if (!completed)
                    {
                        observable.Close();
                        completed = true;
                        CompleteAll();
                    }
                }, () =>
                {
                    if (!completed)
                    {
                        completed = true;
                        CompleteAll();
                    }
                });
            }

            public override void Close()
            {
                observable.Close();
            }
        }
    }
}
