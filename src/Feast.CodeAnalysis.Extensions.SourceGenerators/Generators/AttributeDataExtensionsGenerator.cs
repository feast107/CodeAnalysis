using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Extensions.Generators;

[Generator]
public class AttributeDataExtensionsGenerator : IIncrementalGenerator
{
    private const string ClassName = "AttributeDataExtensions";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
    }
}