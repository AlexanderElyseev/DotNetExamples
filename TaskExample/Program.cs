namespace TaskExample
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Examples of parallel applications using <see cref="Task{TResult}" /> and <see cref="Task" />.
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
            Example5();

            // Do not exit the application (thread pool threads are background).
            Console.ReadLine();
        }

        /// <summary>
        /// Launching task using <see cref="Task.Start()"/>.
        /// </summary>
        static void Example1()
        {
            new Task(() => DoOperation(CancellationToken.None)).Start();
        }
        
        /// <summary>
        /// Launching task using <see cref="Task.Run(System.Action)"/>.
        /// </summary>
        static void Example2()
        {
            Task.Run(() => DoOperation(CancellationToken.None));
        }

        /// <summary>
        /// Handling exceptions inside tasks.
        /// </summary>
        static void Example3()
        {
            try
            {
                var parent = new Task(() =>
                {
                    new Task(() =>
                    {
                        Console.WriteLine("[{0}] Starting child operation 1.", Thread.CurrentThread.ManagedThreadId);
                        throw new Exception();
                    }, TaskCreationOptions.AttachedToParent).Start();

                    new Task(() =>
                    {
                        Console.WriteLine("[{0}] Starting child operation 2.", Thread.CurrentThread.ManagedThreadId);
                        throw new Exception();
                    }, TaskCreationOptions.AttachedToParent).Start();

                    Console.WriteLine("[{0}] Starting parent operation.", Thread.CurrentThread.ManagedThreadId);
                    throw new Exception();
                });

                parent.Start();

                // Exception only on Wait or task.Result.
                //parent.Wait();
            }
            catch (AggregateException e)
            {
                // e.InnerExceptions contains 3 elements
                Console.WriteLine("[{0}] Aggregate exception: {1}", Thread.CurrentThread.ManagedThreadId, e.InnerExceptions.Count);
            }
        }

        /// <summary>
        /// Using <see cref="TaskScheduler.UnobservedTaskException"/> for handling
        /// exceptions that occured in tasks, but without calling <see cref="Task.Wait()"/>
        /// or using <see cref="Task{TResult}.Result"/>.
        /// 
        /// !!!!!!!!!!!!!!!!!!!!!! NOT WORKING !!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        static void Example4()
        {
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Console.WriteLine("[{0}] UnobservedTaskException: {1}", Thread.CurrentThread.ManagedThreadId, args.Exception.InnerExceptions.Count);
            };

            var task = new Task(() =>
            {
                Console.WriteLine("[{0}] Starting parent operation.", Thread.CurrentThread.ManagedThreadId);
                throw new Exception();
            });

            task.Start();

            Thread.Sleep(2000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <summary>
        /// Using <see cref="CancellationToken"/> with <see cref="Task{TResult}"/> for
        /// cancelling task.
        /// Using <see cref="CancellationToken.ThrowIfCancellationRequested"/> is need to
        /// distinguishing correct result and cancelled operation. 
        /// </summary>
        static void Example5()
        {
            var cts = new CancellationTokenSource();

            var task = new Task<int>(() => { DoOperation(cts.Token); return 42; });
            task.Start();

            Thread.Sleep(1500);

            cts.Cancel();

            try
            {
                Console.WriteLine("[{0}] Task result: {1}", Thread.CurrentThread.ManagedThreadId, task.Result);
            }
            catch (AggregateException e)
            {
                Console.WriteLine("[{0}] {1}", Thread.CurrentThread.ManagedThreadId, e.InnerExceptions.First().GetType().FullName);
            }
        }

        /// <summary>
        /// Long operation.
        /// </summary>
        /// <param name="token">Cancellation token for operation.</param>
        static void DoOperation(CancellationToken token)
        {
            Console.WriteLine("[{0}] Operation started.", Thread.CurrentThread.ManagedThreadId);
            for (int i = 0; i < 10; i++)
            {
                token.ThrowIfCancellationRequested();

                Console.WriteLine("[{0}] Operation iteration " + i, Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(500);
            }

            Console.WriteLine("[{0}] Operation ended.", Thread.CurrentThread.ManagedThreadId);
        }
    }
}
