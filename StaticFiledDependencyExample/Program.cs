namespace StaticFiledDependencyExample
{
    using System;

    class A { public static int AVal = B.BVal + 1; }

    class B { public static int BVal = A.AVal + 1; }

    class Program
    {
        static void Main()
        {
            Console.WriteLine(A.AVal);
            Console.WriteLine(B.BVal);
            Console.ReadLine();
        }
    }
}
