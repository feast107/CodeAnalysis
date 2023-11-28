using Feast.CodeAnalysis.SourceGenerators.Templates;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class SyntaxExtensionsGenerator : IIncrementalGenerator
{
    private const string ClassName = nameof(SyntaxExtensions);
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(ClassName),
                Generate(ClassName, GetGenerateTexts(typeof(SyntaxExtensions))
                ));
        });
    }
}