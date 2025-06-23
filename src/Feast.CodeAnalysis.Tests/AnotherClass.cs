#nullable enable
using System.Collections;
using System.Diagnostics;
using System.Text;
using Feast.CodeAnalysis.TestGenerator;

[assembly: ForAssembly]

namespace Feast.CodeAnalysis.Tests;


public class CustomAttribute : Attribute;

[CustomAttribute]
[System.Literal("Feast.CodeAnalysis.Tests.AnotherClasses")]
[Sample]
public partial class AnotherClass<T> where T : IEnumerable<T>
{
    public string EE(E e) => e is E.e ? To("") : To(""); 
    
    public enum E
    {
        e
    }

    public string[] Member = new List<string> { nameof(StringBuilder) }.ToArray();

    private string To(string str) => str;
    
    public string Generate(StringBuilder[] args, StringBuilder[] unused)
    {
        var sb = new StringBuilder();
        foreach (var i in args)
        {
            sb.Append(i);
        }

        if (true)
        {
            return Member.Length > 0 ? "" : (Member as IEnumerable).ToString();
        }
    }
}


public class Foo<T1, T2, T3>
{

}

public class Foo<T2,T3> : Foo<int,T2,T3>;