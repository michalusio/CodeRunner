using Reactive.Observables;

namespace Reactive
{
    public class Subject<T> : Observable<T>
    {
        public void Next(T value)
        {
            foreach (var action in Actions)
            {
                action(value);
            }
        }

        public void Complete()
        {
            CompleteAll();
        }

        public override void Close()
        {
            
        }
    }
}
