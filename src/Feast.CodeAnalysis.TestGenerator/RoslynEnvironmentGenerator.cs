using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Feast.CodeAnalysis.Scripting;

namespace Feast.CodeAnalysis.TestGenerator
{
    [Generator]
    public class RoslynEnvironmentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(c =>
            {
                var sb = new StringBuilder("public static class RoslynAssemblies{").AppendLine();

                sb.AppendLine("public static global::System.Collections.Generic.IEnumerable<string> Report(){");

                sb.Append("yield return ");
                sb.AppendLine($"\"Calling Assembly {Assembly.GetCallingAssembly().FullName}\";");

                sb.Append("yield return ");
                sb.AppendLine($"\"Entry Assembly {Assembly.GetEntryAssembly()?.FullName}\";");

                sb.Append("yield return ");
                sb.AppendLine($"\"Executing Assembly {Assembly.GetExecutingAssembly().FullName}\";");
                
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().OrderBy(x=>x.FullName))
                {
                    sb.Append("yield return ");
                    sb.AppendLine($"\"Loaded Assembly {assembly.FullName}\";");
                }
                
                sb.AppendLine("}");
                sb.AppendLine("}");
                c.AddSource("Roslyn_Assemblies.g.cs", SyntaxFactory.ParseCompilationUnit(sb.ToString())
                    .NormalizeWhitespace()
                    .GetText(Encoding.UTF8));
            });
            
            context.RegisterPostInitializationOutput(c =>
            {
                var sb = new StringBuilder("public static class RoslynTypes{").AppendLine();

                sb.AppendLine("public static global::System.Collections.Generic.IEnumerable<string> Report(){");

                var scripting = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .FirstOrDefault(static x => x.GetName().Name.Equals("Microsoft.CodeAnalysis.Scripting"));

                if (scripting is null)
                {
                    sb.AppendLine("yield return \"\";");
                    Finish();
                    return;
                }
                
                foreach (var type in scripting!.GetTypes())
                {
                    sb.AppendLine($"yield return \"{type.FullName}\";");
                }
                var resolver = scripting.GetType("Microsoft.CodeAnalysis.Scripting.Hosting.RuntimeMetadataReferenceResolver");
                sb.AppendLine($"yield return \"{resolver.FullName}\";");
                //var obj = FormatterServices.GetUninitializedObject(resolver);
                //Debugger.Launch();
                try
                {
                    sb.AppendLine($"yield return \"1{CSharpScript.EvaluateAsync("return \"Content\";").Result}\";");
                }
                catch (Exception e)
                {
                    Debugger.Break();
                }
                
                Finish();

                void Finish()
                {
                    sb.AppendLine("}");
                    sb.AppendLine("}");
                    c.AddSource("Roslyn_Environment.g.cs", SyntaxFactory.ParseCompilationUnit(sb.ToString())
                        .NormalizeWhitespace()
                        .GetText(Encoding.UTF8));
                }
            });
        }
    }
}
