using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpRecap
{
    class Equality
    {
        public static void RunTest() 
        {
            //C# provides 4 defferent functions to determine whether the two different objects are equal:
            //the mathematical properties of equality: Equality is reflexive, symmetric, and transitive. 
            //
            // 1. ReferenceEquals
            //
            //public static bool ReferenceEquals( object left, object right ); test the object identity of two different variables.
            //returns TRUE if two variables refer to the same object, that is the two variables have the same object identity. Whether the types being compared are reference types or value types, this method always tests object identity, not object contents. Yes, that means that ReferenceEquals() always returns false when you use it to test equality for value types. Even when you compare a value type to itself, ReferenceEquals() returns false. This is due to boxing.

            //
            // 2. static Equals
            //
            //public static bool Equals( object left, object right ); determines whether two objects are the same when you don't know the runtime type
            //This method tests whether two variables are equal when you don't know the runtime type of the two arguments. The static Equals() uses the instance Equals() method of the left argument to determine whether two objects are equal.

            //
            // 3. instance Equals
            //
            //You create your own instance version of Equals() when the default behavior is inconsistent with your type.
            //The default Object.Equals() function behaves exactly the same as Object.ReferenceEquals().
            //But value types are different. System.ValueType does override Object.Equals(). Two variables of a value type are equal if they are the same type and they have the same contents. Unfortunately, ValueType.Equals() does not have an efficient implementation. ValueType.Equals() is the base class for all value types. To provide the correct behavior, it must compare all the member variables in any derived type, without knowing the runtime type of the object. In C#, that means using reflection. 
            //The recommendation for value types is simple: Always create an override of ValueType.Equals() whenever you create a value type.
            //You should override the instance Equals() function only when you want to change the defined semantics for a reference type.
            //public virtual bool Equals(object right);

            //the standard pattern for reference type to override Equals
            //You should call the base.Equals only if the base version is not provided by System.Object or System.ValueType.
            //public class Foo
            //{
            //  public override bool Equals( object right )
            //  {
            //    // check null:
            //    // the this pointer is never null in C# methods.
            //    if (right == null)
            //      return false;

            //    if (object.ReferenceEquals( this, right ))
            //      return true;

            //    // Discussed below.
            //    if (this.GetType() != right.GetType())
            //      return false;

            //You should call the base.Equals only if the base version is not provided by System.Object or System.ValueType.
            //    if (base.Equals(rightAsD) == false)
            //      return false;

            //    // Compare this type's contents here:
            //    return CompareFooMembers(
            //      this, right as Foo );
            //  }
            //}

            //The rule is to override Equals() whenever you create a value type, and to override Equals() on reference types when you do not want your reference type to obey reference semantics, as defined by System.Object.
            //Overriding Equals() means that you should write an override for GetHashCode().

            //
            // 4. operator ==
            //
            //Anytime you create a value type, redefine operator==(). The reason is exactly the same as with the instance Equals() function. The default version uses reflection to compare the contents of two value types.
            //You should rarely override operator==()when you create reference types. The .NET Framework classes expect operator==() to follow reference semantics for all reference types.
            //public static bool operator==(MyClass left, MyClass right);

            //You always override instance Equals() and operator==() for value types to provide better performance. You override instance Equals() for reference types when you want equality to mean something other than object identity.



            //Two variables of a reference type are equal if they refer to the same object, referred to as object identity. 
            //Two variables of a value type are equal if they are the same type and they contain the same contents.
            //Object的Equal，对于引用类型比较引用相当，即恒等，对值类型比较值相等
            //ValueType的Equal比较值相等, 使用反射进行比较
            //未重载 == 的引用类型比较两个引用类型是否引用同一对象
            //未重载 == 的值类型，会比较这两个值是否"按位"相等，即是否这两个值中的每个字段都相等
            S s1 = new S();
            s1.i = 5;
            S s2 = new S();
            s2.i = 5;
            //struct doesn't have default == operator
            //Console.WriteLine(s1 == s2);
            Console.WriteLine(s1.Equals(s2));

            Test2 t1 = new Test2();
            Test2 t2 = new Test2();
            Console.WriteLine(t1.Equals(t2));
            Console.WriteLine(t1 == t2);

            // Create two equal but distinct strings
            string a = new string(new char[] { 'h', 'e', 'l', 'l', 'o' });
            string b = new string(new char[] { 'h', 'e', 'l', 'l', 'o' });
            string x = "hello";
            string y = "hello";
            Console.WriteLine(object.ReferenceEquals(a, b));
            Console.WriteLine(object.ReferenceEquals(x, y));

            Console.WriteLine(a == b);
            Console.WriteLine(a.Equals(b));
            // Now let's see what happens with the same tests but
            // with variables of type object
            object c = a;
            object d = b;
            //运算符被重载而不是被重写，这意味着除非编译器知道调用更为具体的版本，否则它只是调用恒等版本
            //编译器不知道 c 和 d 都是字符串引用，因而只能调用 == 的非重载版本(object的)
            Console.WriteLine(c == d);
            Console.WriteLine(c.Equals(d));
        }
    }

    struct S
    {
        public int i;

        //public static bool operator ==(S a, S b)
        //{
        //    return a.i == b.i;
        //}

        //public static bool operator !=(S a, S b)
        //{
        //    return a.i != b.i;
        //}
    }
}
