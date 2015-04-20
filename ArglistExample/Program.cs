using System;

namespace ArglistExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Print(__arglist(3, 3, 4));
        }

        static void Print(__arglist)
        {
            var it = new ArgIterator(__arglist);
            var n = it.GetRemainingCount();
            for (int i = 0; i < n; i++)
            {
                TypedReference reference = it.GetNextArg();

                Type type = __reftype(reference);
                int val = __refvalue(reference, int);  // Only ints here (or InvalidCastException).

                Console.WriteLine("Type: {0}, Val: {1}.", type, val);
            }
        }
    }
}
