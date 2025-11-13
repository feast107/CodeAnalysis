using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Feast.CodeAnalysis.TestGenerator
{
    [Generator]
    public class RoslynEnvironmentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(c =>
            {
                var sb = new StringBuilder("public static class RoslynEnvironment{").AppendLine();

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
                c.AddSource("Roslyn_Environment.g.cs", SyntaxFactory.ParseCompilationUnit(sb.ToString())
                    .NormalizeWhitespace()
                    .GetText(Encoding.UTF8));
            });
        }
    }
}
