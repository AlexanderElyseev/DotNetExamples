namespace WeakReferenceExample
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Example of using <see cref="WeakReference"/> for preventing memory leaks in events.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Example of class, that uses <see cref="WeakReference"/> for storing event listeners.
        /// This move prevent memory leaks in the cases when source lives longer than source.
        /// </summary>
        class WeakEventSource
        {
            struct WeakEventStorage
            {
                public WeakReference Target;

                public MethodInfo Callback;
            }

            private readonly List<WeakEventStorage> _handlers = new List<WeakEventStorage>(); 

            public event EventHandler<EventArgs> WeakEvent
            {
                add
                {
                    var storage = new WeakEventStorage
                    {
                        Target = new WeakReference(value.Target),
                        Callback = value.Method
                    };

                    _handlers.Add(storage);
                }
                remove { }
            }

            public void OnWeakEvent()
            {
                foreach (var storage in _handlers)
                {
                    object target = storage.Target.Target;
                    if (target != null)
                    {
                        storage.Callback.Invoke(target, new[] { target, EventArgs.Empty });
                    }
                }
            }
        }

        /// <summary>
        /// Example of class with regular event.
        /// Provide memory leaks (prvent listeners to be collected.
        /// </summary>
        class StrongEventSource
        {
            public event EventHandler<EventArgs> StrongEvent;

            public void OnStrongEvent()
            {
                EventHandler<EventArgs> handler = StrongEvent;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Regular event listener.
        /// </summary>
        class EventListener
        {
            public void EventHandler(object sender, EventArgs eventArgs)
            {
                Console.WriteLine("Handled");
            }
        }

        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            var weakSource = new WeakEventSource();
            var weakListener = new EventListener();

            // "Weak" event source stores only weak reference to the listener.
            weakSource.WeakEvent += weakListener.EventHandler;

            var strongSource = new StrongEventSource();
            var strongListener = new EventListener();

            // "Strong" event source stores regular reference to the listener (over delegate).
            strongSource.StrongEvent += strongListener.EventHandler;

            // After collecting listeners are not available.
            GC.Collect();

            Console.WriteLine("Testing weak:");

            // In Release configuration, it won't be printed "Handled", because listener is collected.
            // Weak reference doesn't prevent listener to be collected.
            // Ib Debug configuration, listener will be alive till the end of the method ("Handled" will be printed).
            weakSource.OnWeakEvent();

            Console.WriteLine("Testing strong:");

            // In both: release and debug configurations "Handled" will be printed, because the source stores
            // reference to the listener, that prevents listener to be collected.
            strongSource.OnStrongEvent();
        }
    }
}
