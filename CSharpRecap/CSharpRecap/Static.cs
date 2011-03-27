using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace CSharpRecap
{
    class TestStatic 
    {
        public static void Run() 
        {
            #region load order
            Console.WriteLine(X.a + " " + Y.y);//5 2
            Console.WriteLine(X.b + " " + X.c);//3 5
            X x1 = new X();
            Y y1 = new Y();

            Console.WriteLine(Test1.Getddd());
            Console.WriteLine(Test1.ddd);
            #endregion

            //http://csharpindepth.com/Articles/General/Beforefieldinit.aspx
            //http://msmvps.com/blogs/jon_skeet/archive/2010/01/26/type-initialization-changes-in-net-4-0.aspx

            #region beforefieldinit

            //ImplicitConstructor 具有一个附加的元数据标志，名为 beforefieldinit。此标志使得运行库能够在任何时候执行类型构造函数方法，只要该方法在第一次访问该类型的静态字段之前执行即可。换句话说，beforefieldinit 为运行库提供了一个执行主动优化的许可。如果没有 beforefieldinit，运行库就必须在某个精确时间运行类型构造函数，即，恰好在第一次访问该类型的静态或实例字段和方法之前。当存在显式类型构造函数时，编译器不会用 beforefieldinit 标记该类型.
            //If marked BeforeFieldInit then the type's initializer method is executed at, or sometime before, first access to any static field defined for that type
            //If not marked BeforeFieldInit then that type's initializer method is executed at (i.e., is triggered by):
            //first access to any static or instance field of that type, or
            //first invocation of any static, instance or virtual method of that type, or
            //An instance of the class is created.
            //In other words, it causes types to be initialized immediately before the type is first used, either by constructing an instance or referring to a static member.
            Console.WriteLine("Starting Main");
            // Invoke a static method on Test
            Test.EchoAndReturn("Echo!");
            Console.WriteLine("After echo");
            // Reference a static field in Test
            string y = Test.x;
            // Use the value just to avoid compiler cleverness
            if (y != null)
            {
                Console.WriteLine("After field access");
            }

            #endregion

            #region lazy initialization in .NET 4
            //The CLR guarantees that the type initializer will be run at some point before the first reference to any static field. If you don't use a static field, the type doesn't have to be initialized... and it .NET 4.0 obeys that in a fairly lazy way
            //run in .net 4 mode, and comparing to .net 2 mode
            Console.WriteLine("Before static method");
            Lazy.StaticMethod();
            Console.WriteLine("Before construction");
            Lazy lazy = new Lazy();
            Console.WriteLine("Before instance method");
            lazy.InstanceMethod();
            Console.WriteLine("Before static method using field");
            Lazy.StaticMethodUsingField();
            Console.WriteLine("End");

            #endregion

            #region throw exception from type constructor
            //
            // throw exception from type constructor
            //
            //Another behavior unique to type constructors is how the runtime manages exceptions in a type constructor.
            //The runtime will stop any exceptions trying to leave a type constructor and wrap the exception inside of a new TypeInitializationException object. The original exception thrown inside the type constructor is then available from the InnerException property of the TypeInitializationException object
            string message = "";
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    //the second time you try to access a static property of the type, the runtime does not try to invoke the type constructor again, but throws the same exception observed in the first iteration. The runtime does not give a type constructor a second chance. The same rules hold true if the exception is thrown from a static field initializer.
                    //A TypeInitializationException can be a fatal error in an application since it renders a type useless. You should plan to catch any exceptions inside of a type constructor if there is any possibility of recovering from the error, and you should allow an application to terminate if the error cannot be reconciled.
                    message = ExplicitConstructor.StaticMessage;
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine("Caught ApplicationException: ");
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught Exception: ");
                    Console.WriteLine("   " + e.Message);
                    Console.WriteLine("   " + e.GetType().FullName);
                    Console.WriteLine("   " + e.InnerException.Message);
                }
                finally
                {
                    Console.WriteLine("Message = {0}", message);
                }
            }
            #endregion

            #region a type constructor will execute only once
            //
            //The Common Language Infrastructure (CLI) spec guarantees a type constructor will execute only once
            //
            //To enforce this guarantee in a multithreaded environment requires a lock to synchronize threads. A runtime thread must acquire this lock before invoking the type constructor
            //This sends thread A into the type constructor for Static1, and thread B into the type constructor for Static2, then puts both the threads to sleep. Thread A will wake up and need access to Static2, which B has locked. Thread B will wake up and need access to Static1, which A has locked. Thread A needs the lock held by Thread B, and Thread B needs the lock held by Thread A. This is a classic deadlock scenario.
            Thread threadA = new Thread(new ThreadStart(TouchStatic1));
            threadA.Name = "Thread A";
            Thread threadB = new Thread(new ThreadStart(TouchStatic2));
            threadB.Name = "Thread B";
            threadA.Start();
            threadB.Start();
            threadA.Join();
            threadB.Join();
            //It turns out the CLI specification also guarantees that the runtime will not allow type constructors to create a deadlock situation, unless additional locks are explicitly taken by user code
            //the runtime avoided deadlock by allowing Static2(Static1) to access the static Message property of Static1(Static2) before the type constructor for Static1(Static2) finished execution.
            #endregion

            //the CLR does not support the inheritance of static members. Nevertheless, both the Visual Basic and C# compilers allow you to touch static members of a base type through a derived type name.
        }

        static void TouchStatic1() { string s = Static1.Message; }
        static void TouchStatic2() { string s = Static2.Message; }
    }

    class Test
    {
        public static string x = EchoAndReturn("In type initializer");

        public static string EchoAndReturn(string s)
        {
            Console.WriteLine(s);
            return s;
        }
    }

    public class ImplicitConstructor
    {
        public static string StaticMessage = "this is a string";
    }

    public class ExplicitConstructor
    {
        public static string StaticMessage;

        static ExplicitConstructor()
        {
            StaticMessage = "this is a string";

            throw new ApplicationException("ExceptionFromStaticCctor always throws!");
        }
    }

    class Static1
    {
        static Static1()
        {
            Console.WriteLine("Begin Static1 .cctor on thread {0}",
                Thread.CurrentThread.Name);
            Thread.Sleep(5000);
            Console.WriteLine("Static1 has a message from Static2: {0}",
                Static2.Message);
            message = "Hello From Static1";
            Console.WriteLine("Exit Static1 .cctor on thread {0}",
                Thread.CurrentThread.Name);
        }

        static public string Message { get { return message; } }
        static string message = "blank";
    }

    class Static2
    {
        static Static2()
        {
            Console.WriteLine("Begin Static2 .cctor on thread {0}",
                Thread.CurrentThread.Name);
            Thread.Sleep(5000);
            Console.WriteLine("Static2 has a message from Static1: {0}",
                Static1.Message);
            message = "Hello From Static2";
            Console.WriteLine("Exit Static2 .cctor on thread {0}",
                Thread.CurrentThread.Name);
        }

        static public string Message { get { return message; } }
        static string message = "blank";
    }

    class X
    {
        public static int a = Y.y + 3;
        public static int b = c + 3;
        public static int c = b + 2;
    }
    class Y
    {
        public static int y = X.a + 2;
    }

    public class Test1
    {
        public static string ddd = Test2.kkk;
        public static string Getddd()
        {
            return "ddd";
        }
    }
    public class Test2
    {
        public static string kkk = "kkk";
    }

    class Lazy
    {
        private static int x = Log();
        private static int y = 0;

        private static int Log()
        {
            Console.WriteLine("Type initialized");
            return 0;
        }

        public static void StaticMethod()
        {
            Console.WriteLine("In static method");
        }

        public static void StaticMethodUsingField()
        {
            Console.WriteLine("In static method using field");
            Console.WriteLine("y = {0}", y);
        }

        public void InstanceMethod()
        {
            Console.WriteLine("In instance method");
        }
    }
}
