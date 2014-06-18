using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorsExample
{
    class A { }

    class B : Object
    {
        public B() : base()
        {
        
        }
    }

    class C
    {
        private readonly int v;

        public C(int v)
        {
            this.v = v;
        }
    }

    class D : C
    {
        public D(int v) : base(v)
        {
        }
    }

    struct E
    {
        private readonly int v;

        public E(int v)
        {
            this.v = v;
            Console.WriteLine("E has been constructed : " + v.ToString());
        }
    }

    struct F
    {
        private int x;
        private int y;

        public F(int x)
        {
            // All fileds have to be initialized in constructor of value type.
            // It can be made by default constructor.
            this = new F();

            // Update some other fields.
            this.x = x;
        }
    }

    struct G
    {
        public int x;

        static G()
        {
            Console.WriteLine("G type has been constructed.");
        }

        public G(int x)
        {
            this.x = x;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Constructor of value-type is called only explicitly.
            // No text in console.
            E e1;

            // Print text to console.
            E e2 = new E(1);

            // Static constructor is not called always.
            // Type constructor is not called on default constructor.
            G g1;
            g1.x = 10;

            // Type constructor is not called on boxing.
            Object g2 = new G();

            // Type constructor is called on explicit constructor.
            G g3 = new G(1);

            Console.ReadKey();
        }
    }
}
