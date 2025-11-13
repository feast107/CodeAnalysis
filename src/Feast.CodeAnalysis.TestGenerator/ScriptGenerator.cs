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
        try
        {
            var assembly = Assembly.LoadFile(@"C:\Users\feast\Downloads\microsoft.codeanalysis.scripting.common.4.0.1\lib\netstandard2.0\Microsoft.CodeAnalysis.Scripting.dll");
        }
        catch (Exception ex)
        {
            Debugger.Launch();
        }

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
                Debugger.Launch();
                c.AddSource("Error", ex.ToString());
            }
        });
    }
}