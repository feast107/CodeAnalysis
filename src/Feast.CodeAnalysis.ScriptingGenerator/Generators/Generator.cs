using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.ScriptingGenerator.Generators;

[Generator]
public class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c =>
        {
            const string nameSpace = "Feast.CodeAnalysis.Scripting.Generates";
            foreach (var type in GetType().Assembly.GetTypes().Where(x=>x.Namespace?.Contains(nameSpace) is true))
            {
                c.AddSource(type.FullName!.Replace(nameSpace, "Feast.CodeAnalysis.Scripting"),
                    type.GetField("Text", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as string);
            }
        });
    }

    private string FileName(string name)
    {
        return $"{nameof(Feast)}.{nameof(CodeAnalysis)}.{nameof(Scripting)}.{name}";
    }
}