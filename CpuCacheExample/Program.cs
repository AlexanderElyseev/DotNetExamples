using System;

namespace CpuCacheExample
{
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Example of CPU cache impact in matrix multiplication.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Count of iterations for calculating average time.
        /// </summary>
        const int Iterations = 10;

        /// <summary>
        /// Aplication entry point.
        /// </summary>
        static void Main()
        {
            Console.WriteLine("[{0}] Warming JIT..", DateTime.Now);
            Test(100, NaiveMultiplication);
            Test(100, (m1, m2) => BlockMultiple(m1, m2, 10));

            var dimensions = new [] { 128, 256, 512, 1024, 2048 };
            var blockSizes = new[] { 8, 16, 32, 64, 128, 256, 512, 1024 };

            foreach (var dimension in dimensions)
            {
                Console.WriteLine("[{0}] Starting naive version. Dimension: {1}.", DateTime.Now, dimension);
                double secondsWithNaive = Test(dimension, NaiveMultiplication);
                Console.WriteLine("Dimension: {0}, Time (s): {1}.", dimension, secondsWithNaive);

                int clojureDimension = dimension;
                foreach (var blockSize in blockSizes.Where(i => i < clojureDimension))
                {
                    Console.WriteLine("[{0}] Starting partial version. Block size: {1}.", DateTime.Now, blockSize);
                    int clojureBlock = blockSize;
                    double secondsWithBlocks = Test(dimension, (m1, m2) => BlockMultiple(m1, m2, clojureBlock));
                    Console.WriteLine("Dimension: {0}. Block size: {1}. Time (s): {2}.", dimension, blockSize, secondsWithBlocks);

                    Console.WriteLine(">>>>> Dimension: {0}. Block size: {1}. Ratio: {2:f2}%.", dimension, blockSize, secondsWithNaive / secondsWithBlocks * 100);
                }
            }

            Console.Read();
        }

        /// <summary>
        /// Launch test with cleanup.
        /// </summary>
        /// <param name="dimension">Dimension of square matrix.</param>
        /// <param name="multiply">Multiplication algorithm.</param>
        /// <returns>Average time in seconds.</returns>
        private static double Test(int dimension, Func<int[,], int[,], int[,]> multiply)
        {
            var m1 = GetRandomMatrix(dimension);
            var m2 = GetRandomMatrix(dimension);

            var watch = new Stopwatch();

            double[] times = new double[Iterations];
            for (int i = 0; i < Iterations; i++)
            {
                Console.WriteLine("[{0}] Iteration started.", DateTime.Now);

                watch.Restart();
                multiply(m1, m2);
                watch.Stop();

                times[i] = watch.Elapsed.TotalSeconds;

                Console.WriteLine("[{0}] Iteration ended. Elapsed (s): {1}.", DateTime.Now, times[i]);
            }

            double result = times.Average();
            
            GC.Collect();

            return result;
        }

        /// <summary>
        /// Naive multiplication algorithm of two matrices.
        /// Cause many cache misses when accessing "columns".
        /// </summary>
        /// <param name="m1">Firts matrix.</param>
        /// <param name="m2">Second matrix.</param>
        /// <returns>Result of multiplication.</returns>
        private static int[,] NaiveMultiplication(int[,] m1, int[,] m2)
        {
            int dimension = m1.GetLength(0);
            int[,] m = new int[dimension, dimension];

            for (int i = 0; i < dimension; ++i)
                for (int j = 0; j < dimension; ++j)
                    for (int k = 0; k < dimension; ++k)
                        m[i, j] += m1[i, k] * m2[k, j];

            return m;
        }

        /// <summary>
        /// Multiplication by blocks of matrices.
        /// Cause less cache misses in comparisson with <see cref="NaiveMultiplication"/>.
        /// </summary>
        /// <param name="m1">Firts matrix.</param>
        /// <param name="m2">Second matrix.</param>
        /// <param name="bs">Block size.</param>
        /// <returns>Result of multiplication.</returns>
        private static int[,] BlockMultiple(int[,] m1, int[,] m2, int bs)
        {
            int dimension = m1.GetLength(0);
            int[,] m = new int[dimension, dimension];

            for (int ii = 0; ii < dimension; ii += bs)
                for (int jj = 0; jj < dimension; jj += bs)
                    for (int kk = 0; kk < dimension; kk += bs)
                        for (int i = ii; i < ii + bs; ++i)
                            for (int j = jj; j < jj + bs; ++j)
                                for (int k = kk; k < kk + bs; ++k)
                                    m[i, j] += m1[i, k] * m2[k, j];

            return m;
        }
        
        /// <summary>
        /// Generates random square matrix.
        /// </summary>
        /// <param name="dimension">Dimension of square matrix.</param>
        /// <returns>Matrix with random components.</returns>
        static int[,] GetRandomMatrix(int dimension)
        {
            var rand = new Random();

            int[,] m = new int[dimension, dimension];
            for (int i = 0; i < dimension; i++)
                for (int j = 0; j < dimension; j++)
                    m[i, j] = rand.Next(10);

            return m;
        }
    }
}
