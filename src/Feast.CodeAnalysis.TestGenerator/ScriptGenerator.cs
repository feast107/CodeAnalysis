using System;
using Feast.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;

namespace Feast.CodeAnalysis.TestGenerator;

[Generator]
public class ScriptGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c =>
        {
            try
            {
                var result = CSharpScript.EvaluateAsync("return 1+1;", ScriptOptions.Default).Result;
                c.AddSource("Result", result.ToString());
            }
            catch (Exception ex)
            {
                c.AddSource("Error", ex.ToString());
            }
        });
    }
}