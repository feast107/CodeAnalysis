using Feast.CodeAnalysis.Utils;
using Microsoft.CodeAnalysis;
using TypeInfo = Feast.CodeAnalysis.Utils.TypeInfo;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class TypeInfoGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(nameof(TypeInfo)),
                TypeInfo.TypeInfoText);
            ctx.AddSource(GenerateFileName(nameof(RuntimeTypeInfo)),
                RuntimeTypeInfo.RuntimeTypeInfoText);
            ctx.AddSource(GenerateFileName(nameof(SymbolTypeInfo)),
                SymbolTypeInfo.SymbolTypeInfoText);
        });
    }
}