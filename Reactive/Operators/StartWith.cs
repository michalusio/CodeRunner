using Reactive.Observables;

namespace Reactive.Operators
{
    public static partial class ObservableOperatorExtensions
    {
        public static Observable<T> StartWith<T>(this Observable<T> observable, T value) => new[] { value }.Observable().Join(observable);

    }
}
