using Reactive.Observables;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> Join<T>(this Observable<T> observable, params Observable<T>[] observables) => new JoinObservable<T>(observable, observables);

        private class JoinObservable<T> : Observable<T>
        {
            private Observable<T> obs1;
            private Observable<T>[] obs;

            private int open;
            public JoinObservable(Observable<T> observable, params Observable<T>[] observables)
            {
                obs1 = observable;
                obs = observables;
                obs1.Subscribe(value =>
                {
                    foreach (var action in Actions)
                    {
                        action(value);
                    }
                }, () =>
                {
                    open--;
                    if (open == 0)
                    {
                        CompleteAll();
                    }
                });
                foreach (var obs2 in obs)
                {
                    obs2.Subscribe(value =>
                    {
                        foreach (var action in Actions)
                        {
                            action(value);
                        }
                    }, () =>
                    {
                        open--;
                        if (open == 0)
                        {
                            CompleteAll();
                        }
                    });
                }
                open = 1 + obs.Length;
            }

            public override void Close()
            {
                obs1?.Close();
                obs1 = null;
                foreach (var obs2 in obs)
                {
                    obs2?.Close();
                }
                obs = null;
            }
        }
    }
}
