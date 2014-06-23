using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace IDisposableExample
{
    /// <summary>
    /// Examples of implementation and using <see cref="IDisposable"/>.
    /// </summary>
    class Program
    {
        #region Correct implementation of IDisposable

        /// <summary>
        /// Example of correctly implemented <see cref="IDisposable"/> interface.
        /// Extends <see cref="CriticalFinalizerObject"/> for managing sequence of calling finalizers.
        /// </summary>
        class CorrectDisposable : CriticalFinalizerObject, IDisposable
        {
            /// <summary>
            /// Flag of disposed object.
            /// We can't use this instance after disposing.
            /// </summary>
            private bool _disposed;

            /// <summary>
            /// Some internal managed resource.
            /// </summary>
            private IDisposable _managedResource;

            /// <summary>
            /// Some another internal managed resource, but initialized only
            /// without exception in constructor.
            /// </summary>
            private IDisposable _managedResource2;

            public CorrectDisposable(bool throwException)
            {
                _managedResource = new OtherDisposable();

                // In that case finalizer could be called.
                if (throwException)
                    throw new Exception("Oooops...");

                _managedResource2 = new OtherDisposable();
            }

            public string GetText()
            {
#if DEBUG
                // We can't use this instance after disposing.
                // BUT! Do we really need that, if user just can't use our class correctly?
                if (_disposed)
                    throw new ObjectDisposedException(null);
#endif
                return "some text";
            }

            ~CorrectDisposable()
            {
                // We can use Console object here, because it is given special consideration.
                Console.WriteLine("[{0}:{1}] Finalizer.", Thread.CurrentThread.ManagedThreadId, GetHashCode());

                Dispose(false);
            }

            public void Dispose()
            {
                Console.WriteLine("[{0}:{1}] Dispose.", Thread.CurrentThread.ManagedThreadId, GetHashCode());

#if DEBUG
                // We can't dispose twice.
                // BUT! Do we really need that, if user just can't use our class correctly?
                if (_disposed)
                    throw new ObjectDisposedException(null);
#endif

                try
                {
                    Dispose(true);
                }
                finally
                {
                    // We need to free all resources of parent class.
                    // base.Dispose();
                }

                // All resources have been released. We don't need to use finalizer (dispose again).
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Real disposing logic.
            /// </summary>
            /// <param name="itIsSafeToAlsoFreeManagedObjects">Is it safe to access managed resources.</param>
            protected virtual void Dispose(bool itIsSafeToAlsoFreeManagedObjects)
            {
                // Disposing unamanged resources (because we have to).

                // In case of exception in object constructor (or field initializer)
                // we have to check if unmamanged resource is acquired (see ConstructorExceptionExample). 

                // if (unmanagedResource != null)
                // {
                //     ...
                // }

                // Disposing managed resources (because we want to be helpful).
                // E.g. free used memory right now (when the user is asking about).

                // In case of CriticalFinalizerObject, all finalizers of resources have
                // already been called (where should be dispose logic). Otherwise finalizers
                // will be called later. Anyway, all managed resources will be disposed (if IDisposable
                // has been implemented correctly).

                if (itIsSafeToAlsoFreeManagedObjects)
                {
                    // We can operate only with managed resources when it is disposing from
                    // IDisposable (using). If this method is called from finalizer,
                    // GC could already destroyed them.

                    // This path cannot be executed in case of exception in constructor 
                    // or field initializer, because object won't be created.

                    _managedResource.Dispose();
                    _managedResource = null;

                    _managedResource2.Dispose();
                    _managedResource2 = null;

                    // Mark object as disposed. It's not required on GC.
                    _disposed = true;
                }
            }
        }

        class OtherDisposable : IDisposable
        {
            public void Dispose()
            {
            }

            ~OtherDisposable()
            {
            }
        }

        #endregion

        #region Incorrect implementation and using IDisposable

        class DisposableA : IDisposable
        {
            public void Dispose()
            {
            }

            ~DisposableA()
            {
            }

        }

        class DisposableB : IDisposable
        {
            private DisposableA disposableA;

            public DisposableB()
            {
                disposableA = new DisposableA();
                throw new Exception("OOPS!");
            }

            public void Dispose()
            {
            }

            ~DisposableB()
            {
            }
        }

        class Base : IDisposable
        {
            public Base()
            {
                // Acquire some resource...
            }

            public void Dispose()
            {
            }
        }

        class Derived : Base, IDisposable
        {
            public Derived(object data)
            {
                if (data == null)
                    throw new ArgumentNullException("data");

                // OOPS!!
            }

            ~Derived()
            {
                // We need to call base.Dispose
            }
        }

        class ComposedDisposable : IDisposable
        {
            private readonly DisposableA disposableA = new DisposableA();
            private readonly DisposableB disposableB = new DisposableB();

            public void Dispose()
            {
            }
        }
        
        #endregion

        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            Console.WriteLine("[{0}] Dispose.", Thread.CurrentThread.ManagedThreadId);

            // Managed dispose (dispose after using).
            using (var obj = new CorrectDisposable(false))
            {
                Console.WriteLine("[{0}] Using IDisposable object without exception: {1}", Thread.CurrentThread.ManagedThreadId, obj.GetText());
            }

            //// Unmanaged dispose (dispose on GC).
            var obj2 = new CorrectDisposable(false);
            Console.WriteLine("[{0}] IDisposable object without exception with GC: {1}", Thread.CurrentThread.ManagedThreadId, obj2.GetText());

            // Managed dispose of partially-constructed instance (dispose on GC).
            try
            {
                using (var obj3 = new CorrectDisposable(true))
                {
                    // This will never be called because of exception.
                    // But finalizer will be called (see ConstructorExceptionExample).
                    Console.WriteLine("[{0}] his will never be called: {1}", Thread.CurrentThread.ManagedThreadId, obj3.GetText());
                }
            }
            catch (Exception)
            {
            }

            // Resource leak #1.
            try
            {
                using (var disposable = new DisposableB())
                {
                    // Dispose won't be called nor for DisposableB, nor for DisposableA.
                    Console.WriteLine("[{0}] his will never be called.", Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception)
            {
            }

            // Resource leak #2.
            try
            {
                using (var disposable = new Derived(null))
                {
                    // Dispose won't be called nor for Derived, nor for Base.
                    Console.WriteLine("[{0}] his will never be called.", Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception)
            {
            }

            // Resource leak #3.
            try
            {
                using (var disposable = new ComposedDisposable())
                {
                    // Dispose won't be called nor for ComposedDisposable, nor for DisposableA, nor for DisposableB.
                    Console.WriteLine("[{0}] his will never be called.", Thread.CurrentThread.ManagedThreadId);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
