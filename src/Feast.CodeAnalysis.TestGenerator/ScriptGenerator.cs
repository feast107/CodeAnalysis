using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Feast.CodeAnalysis.Scripting;
using Feast.CodeAnalysis.Scripting.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Feast.CodeAnalysis.TestGenerator;

[Generator]
public class ScriptGenerator : IIncrementalGenerator
{
    static ScriptGenerator()
    {
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c =>
        {
            try
            {
               var code =
                    """
                    return "public class ScriptGeneratedClass{ public static string Report() => \"Success\"; }";
                    """;
                var result = CSharpScript.EvaluateAsync(code).Result;

                c.AddSource("ScriptGenerated.g.cs",
                    SourceText.From(result.ToString(), Encoding.UTF8));
            }
            catch (Exception ex)
            {
                
                c.AddSource("ScriptGenerated.err.g.cs",
                    SourceText.From(
                        $$"""
                          public class ScriptGeneratedClass{ 
                              public static string Report() => global::System.Text.Encoding.UTF8.GetString([{{
                                  string.Join(",", Encoding.UTF8.GetBytes(ex.ToString()).Select(x=>"0x" + x.ToString("x")))
                              }}]);
                          }
                          """, Encoding.UTF8));
            }
        });
    }
}