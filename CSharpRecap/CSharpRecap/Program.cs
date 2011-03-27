using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace CSharpRecap
{
    class Program
    {
        static void Main(string[] args)
        {

            #region equality

            //Equality.RunTest();

            #endregion

            #region constructor call sequence

            MyDerivedClass dc = new MyDerivedClass();

            #endregion

            #region Implicit Static Constructor

            //TestStatic.Run();

            #endregion

            #region Overload

            Overload.Run();

            #endregion
        }
    }

    public class MyBaseClass
    {
        public MyBaseClass()
        {
        }

        public MyBaseClass(int i)
        {
        }
    }

    public class MyDerivedClass : MyBaseClass
    {
        public MyDerivedClass()
        {
        }

        public MyDerivedClass(int i)
        {
        }
    }
}
