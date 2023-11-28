using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

// ReSharper disable once InconsistentNaming
[Generator]
public class ISymbolExtensionsGenerator : IIncrementalGenerator
{
    private const string ClassName = nameof(ISymbolExtensions);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(ClassName),
                Generate(ClassName,
                    GetGenerateTexts(typeof(ISymbolExtensions))
                )
            );
        });
    }
}