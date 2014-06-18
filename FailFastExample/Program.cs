namespace FailFastExample
{
    using System;
    using System.Runtime.ConstrainedExecution;

    class MyCriticalClass : CriticalFinalizerObject, IDisposable
    {
        ~MyCriticalClass()
        {
            Console.WriteLine("Critical finalize.");
        }

        public void Dispose()
        {
            Console.WriteLine("Critical dispose.");
        }
    }

    class Program
    {
        static void Main()
        {
            try
            {
                new MyCriticalClass();
                throw new Exception();
            }
            catch (Exception e)
            {
                Environment.FailFast(e.Message);
            }
            finally
            {
                Console.WriteLine("This won't be printed.");
            }
        }
    }
}
