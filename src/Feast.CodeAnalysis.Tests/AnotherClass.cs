#nullable enable
using System.Collections;
using System.Text;

namespace Feast.CodeAnalysis.Tests;
[System.Literal("Feast.CodeAnalysis.Tests.AnotherClasses")]
public partial class AnotherClass<T> where T : IEnumerable<T>
{
    public string EE(E e) => e is E.e ? "" : ""; 
    
    public enum E
    {
        e
    }

    public string[] Member = new List<string> { nameof(StringBuilder) }.ToArray();
    
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


public class Foo<T>
{

}