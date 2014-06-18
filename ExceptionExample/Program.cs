namespace ExceptionExample
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    throw new Exception("Some error.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error has been catched: {0}", e.Message);
                }
                finally
                {
                    Console.WriteLine("Finally is executing.");

                    // Excpetion inside catch/finally is treated as exception after finaly.
                    // It can be catched at the outer try block.
                    throw new Exception("Error in finally.");

                    Console.WriteLine("It will never be printed.");
                }

                Console.WriteLine("It will never be printed too.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
