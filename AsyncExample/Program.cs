namespace AsyncExample
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            Console.WriteLine("Start Test.");
            for (int i = 0; i < 10; i++)
                Test();

            Console.WriteLine(timer.ElapsedTicks + ": End Test.");
            
            timer.Restart();
            Console.WriteLine("Start TestAsync.");
            for (int i = 0; i < 10; i++)
                TestAsync().ContinueWith(task => Console.WriteLine());

            Console.WriteLine(timer.ElapsedTicks + ": End TestAsync.");
        }

        private static async Task TestAsync()
        {
            Console.WriteLine("TestAsync Begin.");
            await Task.Delay(1000);
            Console.WriteLine("TestAsync End.");
        }

        private static void Test()
        {
            Console.WriteLine("Test Begin.");
            Thread.Sleep(1000);
            Console.WriteLine("Test End.");
        }
    }
}
