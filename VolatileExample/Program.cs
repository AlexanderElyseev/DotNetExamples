namespace VolatileExample
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class with data that shared between two threads without synchronization.
    /// </summary>
    class ThreadRaceData
    {
        private int _a;
        private int _b;

        public void Thread1()
        {
            Thread.Sleep(100);
            
            // WARNING: It could be differenct sequence.
            _b = 1;
            _a = 1;
        }

        public void Thread2()
        {
            Thread.Sleep(100);

            // WARNING: _b could be read later than _a.
            if (_a == 1)
                Console.Write(_b);
        }
    }

    /// <summary>
    /// Class with example of possible compiler oprimization that could bring undefined
    /// behaviour on multithread situation.
    /// If compiler caches value of Stop, thread that executed ThreadFunc won't stop.
    /// Needs /optimize+ and x86 platform.
    /// </summary>
    class ThreadOptimizedData
    {
        public bool Stop;

        public void ThreadFunc()
        {
            int x = 0;
            while (!Stop)
                x++;

            // It couldn't be printed.
            Console.WriteLine("Stopped.");
        }
    }

    /// <summary>
    /// Class with example of using volatile variable to prevent optimization.
    /// Value of Stop won't be cached.
    /// </summary>
    class ThreadNotOptimizedData
    {
        public volatile bool Stop;

        public void ThreadFunc()
        {
            int x = 0;
            while (!Stop)
                x++;
            
            // It'll be printed.
            Console.WriteLine("Stopped.");
        }
    }

    /// <summary>
    /// Class with data that shared between two threads with synchronization using <see cref="Volatile"/>.
    /// </summary>
    class ThreadSynchronizedData
    {
        private int _a;
        private int _b;

        public void Thread1()
        {
            _b = 2;
            Volatile.Write(ref _a, 1);
        }

        public void Thread2()
        {
            if (Volatile.Read(ref _a) == 1)
                Console.Write(_b);
        }
    }

    /// <summary>
    /// Class with data that shared between two threads with synchronization using volatile variables.
    /// IL code is different.
    /// </summary>
    class AnotherThreadSynchronizedData
    {
        private volatile int _a;
        private int _b;

        public void Thread1()
        {
            _b = 2;
            _a = 1;
        }

        public void Thread2()
        {
            if (_a == 1)
                Console.Write(_b);
        }
    }

    /// <summary>
    /// Example of using <see cref="Volatile"/> class and volatile variables for
    /// fyncronization of threads.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            // Example 1.
            // Optimization of ThreadFunc (saving value of Stop in register) break program flow.
            var data = new ThreadOptimizedData();
//            var data = new ThreadNotOptimizedData();
            
            var t = new Thread(data.ThreadFunc);
            t.Start();

            Thread.Sleep(1000);

            // In case of non-volatile variable, value of Stop could be cached in worker thread.
            data.Stop = true;

            Console.WriteLine("Waiting...");
            t.Join();

            //            // Example 2.
            //            // Undefined behaviour with non-volatile variables.
            //            for (int i = 0; i < 100000; i++)
            //            {
            //                // Example of race with not synchronized data. Won't output 0.
            ////                var data = new ThreadSynchronizedData();
            //
            //                // Example of race with not synchronized data. Possible outputs 0.
            //                var data = new ThreadRaceData();
            //
            //                var t1 = new Thread(data.Thread1);
            //                var t2 = new Thread(data.Thread2);
            //
            //                t2.Start();
            //                t1.Start();
            //
            //                t1.Join();
            //                t2.Join();
            //            }
        }
    }
}
