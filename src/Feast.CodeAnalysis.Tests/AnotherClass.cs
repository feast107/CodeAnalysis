using System.Text;

namespace Feast.CodeAnalysis.Tests;

[System.Literal("Feast.CodeAnalysis.Tests.GlobalUsings")]
public class AnotherClass
{
    public enum E
    {
        A
    }

    public string Member = nameof(Member);
    
    public string Getter
    {
        get
        {
            return new global::System.Collections.Generic.List<E>().Where(static X =>
            {
                var i = 1;
                if (i == (int)E.A)
                {
                    i |= (int)E.A;
                    return i > 1;
                }

                return X == (E)i;
            }).ToString();
        }
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