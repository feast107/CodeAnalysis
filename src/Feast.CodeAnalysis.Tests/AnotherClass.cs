using System.Text;

namespace Feast.CodeAnalysis.Tests;

[System.Literal("Feast.CodeAnalysis.Tests.AnotherClasses")]
public class AnotherClass
{
    public enum E
    {
        A
    }

    public string[,][] Member = new string[4,typeof(StringBuilder).ToString().Length][];
    
    public string Getter
    {
        get => new StringBuilder().ToString();

    }

    public string Generate(StringBuilder[] args, StringBuilder[] unused)
    {
        var sb = new StringBuilder();
        foreach (var i in args)
        {
            sb.Append(i);
        }

        return sb.ToString();
    }
}
