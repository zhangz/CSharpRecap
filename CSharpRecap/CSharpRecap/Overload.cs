using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpRecap
{
    //overloading is what happens when you have two methods with the same name but different signatures. At compile time, the compiler works out which one it's going to call, based on the compile time types of the arguments and the target of the method call.
    class Overload
    {
        public static void Run()
        {
            //when choosing an overload, if there are any compatible methods declared in a derived class, all signatures declared in the base class are ignored - even if they're overridden in the same derived class!
            Derived d = new Derived();
            int i = 10;
            d.Foo(i);
        }
    }

    class Base
    {
        public virtual void Foo(int x)
        {
            Console.WriteLine("Base.Foo(int)");
        }
    }

    class Derived : Base
    {
        public override void Foo(int x)
        {
            Console.WriteLine("Derived.Foo(int)");
        }

        public void Foo(object o)
        {
            Console.WriteLine("Derived.Foo(object)");
        }
    }
}
