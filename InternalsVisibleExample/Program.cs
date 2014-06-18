namespace InternalsVisibleExample
{
    using System;
    using System.Reflection;

    using InernalsVisisbleLib;

    public class Program
    {
        static void Main()
        {
            Console.WriteLine("Export types:");
            foreach (Type exportedType in typeof(InternalClass).Assembly.ExportedTypes)
            {
                Console.WriteLine(exportedType.FullName);
            }

            Console.WriteLine();

            Console.WriteLine("Defined types:");
            foreach (Type exportedType in typeof(InternalClass).Assembly.DefinedTypes)
            {
                Console.WriteLine(exportedType.FullName);
            }

            Console.ReadKey();
        }
    }
}
