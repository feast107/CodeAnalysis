using System;
using System.Diagnostics;
using System.Reflection;
using Feast.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

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
                var result = CSharpScript.EvaluateAsync("""
                    return "public class ScriptGeneratedClass{}";
                    """                    
                    , ScriptOptions.Default).Result;
            
                c.AddSource("Result", result.ToString());
            }
            catch (Exception ex)
            {
                //c.AddSource("Error", "// " + ex.ToString());
            }
        });
    }
}