using Feast.CodeAnalysis.SourceGenerators;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

// ReSharper disable once InconsistentNaming
[Generator]
public class ExtendedClassGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(nameof(Global)), 
                """
                #if !ROSLYN_4_3_1_OR_GREATER
                using Microsoft.CodeAnalysis.Internal;
                #endif
                """);
            ctx.AddSource(GenerateFileName(nameof(SyntaxExtensions)),
                LiteralGenerator.SyntaxExtensions.Text.Replace("#endif", ""));
            ctx.AddSource(GenerateFileName(nameof(GeneratorAttributeSyntaxContext)),
                GeneratorAttributeSyntaxContext.Text);
            ctx.AddSource(GenerateFileName("ImmutableArrayBuilder{T}"),
                ImmutableArrayBuilder.Text);
            ctx.AddSource(GenerateFileName(nameof(SyntaxValueProviderExtensions)),
                SyntaxValueProviderExtensions.Text);
        });
    }
}