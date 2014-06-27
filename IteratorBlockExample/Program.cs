namespace IteratorBlockExample
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Examples of implementing and using iterators.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            IteratorExceptionExample();
        }

        /// <summary>
        /// Example of handling exceptions in iterator.
        /// </summary>
        static void IteratorExceptionExample()
        {
            try
            {
                // Iterator block will be executed only on calling MoveNext.
                // See standard 10.14.4.
                IEnumerable<int> ints = GetIteratorWithException();

                Console.WriteLine("This will be written.");
            }
            catch (Exception)
            {
                Console.WriteLine("This will never be written.");
            }
        }

        /// <summary>
        /// Example of iterator that always throws an exception.
        /// Subtle thing is that exception will be generated only when iteration starts.
        /// </summary>
        /// <returns>Some collection.</returns>
        static IEnumerable<int> GetIteratorWithException()
        {
            throw new InvalidOperationException();
            yield return 42;
        }
    }
}
