using System;
using System.Collections.Generic;

namespace Reactive.Observables
{
    public abstract class Observable<T>
    {
        private readonly List<Action<T>> registeredActions = new List<Action<T>>();
        private readonly List<Action> registeredCompletes = new List<Action>();

        protected IReadOnlyCollection<Action<T>> Actions => registeredActions;
        protected void CompleteAll()
        {
            foreach (var action in registeredCompletes)
            {
                action();
            }
        }

        public abstract void Close();

        public void Subscribe(Action<T> onNext = null, Action onComplete = null)
        {
            if (onNext != null)
            {
                registeredActions.Add(onNext);
            }
            if (onComplete != null)
            {
                registeredCompletes.Add(onComplete);
            }
        }
    }
}
