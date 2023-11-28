using System.Linq;
using System.Reflection;
using Feast.CodeAnalysis.SourceGenerators.Templates;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class ITypeSymbolExtensionsGenerator : IIncrementalGenerator
{
    private const string ClassName = nameof(ITypeSymbolExtensions);
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(ClassName),
                Generate(ClassName,GetGenerateTexts(typeof(ITypeSymbolExtensions)))
            );
        });
    }
}