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
            for (int i = 0; i < 100000; i++)
            {
                // Example of race with not synchronized data. Won't output 0.
//                var data = new ThreadSynchronizedData();

                // Example of race with not synchronized data. Possible outputs 0.
                var data = new ThreadRaceData();

                var t1 = new Thread(data.Thread1);
                var t2 = new Thread(data.Thread2);

                t2.Start();
                t1.Start();

                t1.Join();
                t2.Join();
            }
        }
    }
}
