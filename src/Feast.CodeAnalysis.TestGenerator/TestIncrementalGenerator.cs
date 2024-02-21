using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Feast.CodeAnalysis.TestGenerator
{
    /// <summary>
    /// A sample source generator that creates a custom report based on class properties. The target class should be annotated with the 'Generators.ReportAttribute' attribute.
    /// When using the source code as a baseline, an incremental source generator is preferable because it reduces the performance overhead.
    /// </summary>
    [Generator]
    [System.Literal("System.Classes")]
    public class TestIncrementalGenerator : IIncrementalGenerator
    {
        private const string Namespace     = "Generators";
        private const string AttributeName = "AnalyzeAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(static ctx =>
            {
                ctx.AddSource($"{typeof(SampleAttribute).FullName}.g.cs",
                    SourceText.From(System.SampleAttribute.Text, Encoding.UTF8));
            });

            var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                $"{typeof(SampleAttribute).FullName}",
                (_, t) => true,
                (ctx, t) => ctx);

            context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
                static (ctx, t) =>
                {
                    foreach (var syntaxContext in t.Right)
                    {
                        var data = syntaxContext.Attributes
                            .FirstOrDefault(static x =>
                                x.AttributeClass.GetFullyQualifiedName() ==
                                "global::" + typeof(SampleAttribute).FullName);
                        if (data == null)
                        {
                            continue;
                        }

                        if (data.ConstructorArguments.Length == 1)
                        {
                            var arg = data.ConstructorArguments.First();
                            if (arg.Values.Length > 1)
                            {
                            }
                        }

                        var attr = data.ToAttribute<SampleAttribute>();
                    }
                });
        }

    }
}
