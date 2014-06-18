namespace CancellationExample
{
    using System;
    using System.Threading;

    /// <summary>
    /// Examples of cancelling parallel operations using <see cref="CancellationTokenSource"/>
    /// and <see cref="CancellationToken"/>.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
//            Example1();
//            Example2();
//            Example3();
//            Example4();
//            Example5();
//            Example6();
            Example7();
//            Example8();

            // Do not exit the application (thread pool threads are background).
            Console.ReadLine();
        }

        /// <summary>
        /// Cancellation delegate.
        /// If source is cancelled, cancellation delegate will be triggered.
        /// </summary>
        static void Example1()
        {
            CancellationTokenSource cancelledSource = new CancellationTokenSource();
            cancelledSource.Cancel();
            cancelledSource.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate on cancelled source.", Thread.CurrentThread.ManagedThreadId));
        }

        /// <summary>
        /// Cancellation delegate.
        /// Exceptions on delegates could be aggregated or could stop executing all other delegates
        /// after first exception.
        /// </summary>
        static void Example2()
        {
            CancellationTokenSource sourceWithExceptions = new CancellationTokenSource();
            sourceWithExceptions.Token.Register(() => { throw new Exception("Exception 1"); });
            sourceWithExceptions.Token.Register(() => { throw new Exception("Exception 2"); });

            try
            {
                // false => aggregating all exceptions
                sourceWithExceptions.Cancel(false);
            }
            catch (AggregateException)
            {
                // Unfortunately, you can not know the source of the exception.

                Console.WriteLine("[{0}] Aggregate exception.", Thread.CurrentThread.ManagedThreadId);
            }
        }

        /// <summary>
        /// Cancellation token source.
        /// Using one source for multiple operations.
        /// </summary>
        static void Example3()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(state => DoOperation(source.Token));
            ThreadPool.QueueUserWorkItem(state => DoOperation(source.Token));

            Thread.Sleep(2500);
            source.Cancel();
        }

        /// <summary>
        /// Cancellation token.
        /// CancellationToken.None always returns false on IsCancellationRequested.
        /// </summary>
        static void Example4()
        {
            ThreadPool.QueueUserWorkItem(state => DoOperation(CancellationToken.None));
        }

        /// <summary>
        /// Cancellation delegate.
        /// Cancellation delegates relates to source, not to the token (it's a struct).
        /// </summary>
        static void Example5()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken unusedToken = cts.Token;
            unusedToken.Register(() => Console.WriteLine("[{0}] Cancellation delegate 0.", Thread.CurrentThread.ManagedThreadId));

            // Token is not used, but the message will be printed.
            cts.Cancel();
        }

        /// <summary>
        /// Cancellation delegate.
        /// You can delete cancellation delegate disposing <see cref="CancellationTokenRegistration"/>.
        /// </summary>
        static void Example6()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationTokenRegistration reg1 = cts.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 1.", Thread.CurrentThread.ManagedThreadId));
            CancellationTokenRegistration reg2 = cts.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 2.", Thread.CurrentThread.ManagedThreadId));

            reg2.Dispose();

            cts.Cancel();
        }

        /// <summary>
        /// Cancellation token source.
        /// Using linked sources.
        /// </summary>
        static void Example7()
        {
            CancellationTokenSource cts1 = new CancellationTokenSource();
            cts1.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 1.", Thread.CurrentThread.ManagedThreadId));

            CancellationTokenSource cts2 = new CancellationTokenSource();
            cts2.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 2.", Thread.CurrentThread.ManagedThreadId));

            CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
            cts.Token.Register(() => Console.WriteLine("[{0}] Cancellation delegate.", Thread.CurrentThread.ManagedThreadId));

            // Executes delegate only from cts. Only cts is cancelled.
//            cts.Cancel();

            // Executes delegate from cts and cts1. Both are cancelled.
            cts1.Cancel();

            Console.WriteLine("cts1 cancelled={0}; cts2 cancelled={1}; cts cancelled={2};", cts1.IsCancellationRequested, cts2.IsCancellationRequested, cts.IsCancellationRequested);
        }

        /// <summary>
        /// Cancellation token source.
        /// Using cancelling timeout.
        /// </summary>
        static void Example8()
        {
            CancellationTokenSource cts = new CancellationTokenSource(2500);
            ThreadPool.QueueUserWorkItem(state => DoOperation(cts.Token));
        }

        /// <summary>
        /// Long operation.
        /// </summary>
        /// <param name="token">Cancellation token for operation.</param>
        static void DoOperation(CancellationToken token)
        {
            token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 1.", Thread.CurrentThread.ManagedThreadId));
            token.Register(() => Console.WriteLine("[{0}] Cancellation delegate 2.", Thread.CurrentThread.ManagedThreadId));

            Console.WriteLine("[{0}] Operation started.", Thread.CurrentThread.ManagedThreadId);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("[{0}] Operation iteration " + i, Thread.CurrentThread.ManagedThreadId);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("[{0}] Operation cancelled.", Thread.CurrentThread.ManagedThreadId);
                    break;
                }

                Thread.Sleep(1000);
            }

            Console.WriteLine("[{0}] Operation ended.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
