namespace InterlockedExample
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class with example of threads synchronizing using <see cref="Interlocked"/> class.
    /// </summary>
    class ThreadSyncronizedData
    {
        /// <summary>
        /// Flag of executed operation.
        /// </summary>
        private int executed;
        
        /// <summary>
        /// Example of some operation executed by multiple threads.
        /// With <see cref="Interlocked.Exchange(ref int,int)"/> it will nerver output "11..".
        /// </summary>
        public void ThreadFunc()
        {
            Thread.Sleep(100);
            if (Interlocked.Exchange(ref executed, int.MaxValue) == 0)
            {
                Console.Write(1);
            }
        }
    }

    /// <summary>
    /// Class without threads synchronizing.
    /// </summary>
    class ThreadRaceData
    {
        /// <summary>
        /// Flag of executed operation.
        /// </summary>
        private int executed;

        /// <summary>
        /// Example of some operation executed by multiple threads.
        /// In situation of high parallelism degree (many working threads) probability
        /// of outputing "11.." is not zero.
        /// </summary>
        public void ThreadFunc()
        {
            Thread.Sleep(100);
            if (executed != int.MaxValue)
            {
                executed = int.MaxValue;
                Console.Write(1);
            }
        }
    }

    /// <summary>
    /// Example of syncronizing threads using <see cref="Interlocked"/> class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            const int ThredsCount = 100;

            for (int i = 0; i < 1000; i++)
            {
                var data = new ThreadRaceData();
//                var data = new ThreadSyncronizedData();

                var threads = new Thread[ThredsCount];
                for (int j = 0; j < ThredsCount; j++)
                    threads[j] = new Thread(data.ThreadFunc);

                for (int j = 0; j < ThredsCount; j++)
                    threads[j].Start();

                for (int j = 0; j < ThredsCount; j++)
                    threads[j].Join();

                Console.Write(" ");
            }
        }
    }
}
