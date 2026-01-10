using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Feast.CodeAnalysis.Scripting.Generators;

[Generator]
public class AssemblyLoadGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c =>
        {
            c.AddSource($"{nameof(CSharpScript)}Initializer.g.cs", SourceText.From(
                Scripting.CSharpScriptInitializer.Text.Replace(
                    "global::Feast.CodeAnalysis.Scripting.Generators.ModuleInitializerAttribute",
                    "global::System.Runtime.CompilerServices.ModuleInitializerAttribute")
                , Encoding.UTF8));
            c.AddSource($"{nameof(CSharpScript)}Initializer.Assembly.g.cs", 
                SourceText.From(
                    $$"""
                    #pragma warning disable
                    namespace Feast.CodeAnalysis.Scripting.Generators{
                        partial class CSharpScriptInitializer{
                            private static byte[] {{nameof(Resources.Resources.Scripting_5_0_0_2_Final)}} => new byte[] { {{
                                string.Join(",",Resources.Resources.Scripting_5_0_0_2_Final.Select(x=> "0x" + x.ToString("x")))
                            }} };
                        }
                    }
                    """
                    , Encoding.UTF8));
        });
    }
}
