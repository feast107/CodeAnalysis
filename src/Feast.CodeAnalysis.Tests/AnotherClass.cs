using System.Collections;
using System.Text;
using Feast.CodeAnalysis.TestGenerator;

namespace Feast.CodeAnalysis.Tests;

[System.Literal("Feast.CodeAnalysis.Tests.AnotherClasses")]
public class AnotherClass<T> where T : IEnumerable<T>
{
    public enum E
    {
        A
    }

    public string[] Member = new List<string> { typeof(StringBuilder).Name }.ToArray();
    
    public string Getter => (Member as object)?.ToString();

    public string Get() => Member switch
    {
        IEnumerable<string> => ToString()
    };
    
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


[Sample(typeof(Foo<>), typeof(Foo<>))]
public class Foo<T>
{

}