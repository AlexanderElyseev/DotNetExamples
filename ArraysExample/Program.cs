namespace ArraysExample
{
    using System;

    class Program
    {
        static void Main()
        {
            PrintInterfaces("One dimesnion array of value type:", typeof(int[]));
            PrintInterfaces("\nTwo dimesnion array of value type:", typeof(int[,]));
            PrintInterfaces("\nTwo dimesnion jagged array of value type:", typeof(int[][]));
            PrintInterfaces("\nOne dimesnion array of reference type:", typeof(string[]));
            PrintInterfaces("\nTwo dimesnion array of reference type:", typeof(string[,]));
            PrintInterfaces("\nTwo dimesnion jagged array of reference type:", typeof(string[][]));

            CheckOutOfBoundBeforeCycle();

            Console.ReadKey();
        }

        static void PrintInterfaces(string message, Type type)
        {
            Console.WriteLine(message);
            Console.WriteLine(type);
            foreach (var interfaceType in type.GetInterfaces())
                Console.WriteLine(interfaceType);
        }

        static void CheckOutOfBoundBeforeCycle()
        {
            int[] array = { 1, 2, 3 };
            for (int i = -1; i < array.Length; i++)
            {
                Console.WriteLine("Iteration");
                Console.WriteLine(array[i]);
            }
        }
    }
}
