using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Base
    {
        public void M1()
        {
            Console.WriteLine("Base - 1");
        }

        public void M2()
        {
            Console.WriteLine("Base - 2");
        }

        public virtual void M3()
        {
            Console.WriteLine("Base - 3");
        }
    }

    class Derived : Base
    {
        public void M1()
        {
            Console.WriteLine("Derived - 1");
        }

        public new void M2()
        {
            Console.WriteLine("Derived - 2");
        }

        public override void M3()
        {
            Console.WriteLine("Derived - 3");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Base b = new Base();
            Derived d = new Derived();
            Base bd = new Derived();

            b.M1();
            b.M2();
            b.M3();
            d.M1();
            d.M2();
            d.M3();
            bd.M1();
            bd.M2();
            bd.M3();

            Console.ReadKey();
        }
    }
}
