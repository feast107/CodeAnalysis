using Feast.CodeAnalysis.SourceGenerators.Templates;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

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
            ctx.AddSource(GenerateFileName(nameof(GeneratorAttributeSyntaxContext)),
                GeneratorAttributeSyntaxContext.GeneratorAttributeSyntaxContextText);
            ctx.AddSource(GenerateFileName("ImmutableArrayBuilder{T}"),
                ImmutableArrayBuilder<object>.ImmutableArrayBuilderText);
            ctx.AddSource(GenerateFileName(nameof(SyntaxValueProviderExtensions)),
                SyntaxValueProviderExtensions.SyntaxValueProviderText);
        });
    }
}