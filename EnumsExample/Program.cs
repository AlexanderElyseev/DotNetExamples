using System;

namespace EnumsExample
{
    enum Color
    {
        Black,
        White,
        Red = 100,
        AnotherRed = 100
    }
    
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Underlying type: {0}.", Enum.GetUnderlyingType(typeof(Color)));

            var definedColor = (Color)1;
            Console.WriteLine(
                "Defined value: {0}. Name: {3}. Defined: {1}. Type: {2}.",
                definedColor,
                Enum.IsDefined(typeof(Color), definedColor),
                definedColor.GetType(),
                Enum.GetName(typeof(Color), definedColor));
            
            // 123 is not defined, but still is correct 
            var undefinedColor = (Color)123;
            Console.WriteLine(
                "Undefined value: {0}. Name: {3}. Defined: {1}. Type: {2}.",
                undefinedColor,
                Enum.IsDefined(typeof(Color), undefinedColor),
                undefinedColor.GetType(),
                Enum.GetName(typeof(Color), undefinedColor));
            
            Console.WriteLine("Names:");
            foreach (string name in Enum.GetNames(typeof(Color)))
                Console.WriteLine("\t{0}. Type: {1}.", name, name.GetType());

            Console.WriteLine("Values:");
            foreach (object value in Enum.GetValues(typeof(Color)))
                Console.WriteLine("\t{0}. Value: {2:D}. Type: {1}.", value, value.GetType(), value);

            Console.WriteLine("Value: {0}. Defined: {1}", 0, Enum.IsDefined(typeof(Color), 0));
            Console.WriteLine("Value: {0}. Defined: {1}", 123, Enum.IsDefined(typeof(Color), 123));
            Console.WriteLine("Value: {0}. Defined: {1}", "(Color)123", Enum.IsDefined(typeof(Color), (Color)123));
            Console.WriteLine("Value: {0}. Defined: {1}", "Black", Enum.IsDefined(typeof(Color), "Black"));
            Console.WriteLine("Value: {0}. Defined: {1}", "black", Enum.IsDefined(typeof(Color), "black"));

            Console.ReadKey();
        }
    }
}
