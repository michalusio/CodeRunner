using System;

namespace Reactive.Observables
{
    public static partial class ObservableCreationExtensions
    {
        public static Observable<EventArgs> Observable(this EventHandler eventObject)  => new EventObservable(eventObject);
        public static Observable<T> Observable<T>(this EventHandler<T> eventObject) => new EventObservable<T>(eventObject);

        private class EventObservable : Observable<EventArgs>
        {
            private EventHandler eventObject;
            public EventObservable(EventHandler eventObject)
            {
                this.eventObject = eventObject;
                eventObject += HandlerMethod;
            }

            public override void Close()
            {
                eventObject -= HandlerMethod;
            }

            private void HandlerMethod(object sender, EventArgs value)
            {
                foreach (var action in Actions)
                {
                    action(value);
                }
            }
        }

        private class EventObservable<T> : Observable<T>
        {
            private EventHandler<T> eventObject;
            public EventObservable(EventHandler<T> eventObject)
            {
                this.eventObject = eventObject;
                eventObject += HandlerMethod;
            }

            public override void Close()
            {
                eventObject -= HandlerMethod;
            }

            private void HandlerMethod(object sender, T value)
            {
                foreach (var action in Actions)
                {
                    action(value);
                }
            }
        }
    }
}
