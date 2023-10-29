using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class SyntaxValueProviderExtensions
    {
        public static global::Microsoft.CodeAnalysis.IncrementalValuesProvider<T> ForAttributeWithMetadataName<T>(
            this global::Microsoft.CodeAnalysis.SyntaxValueProvider syntaxValueProvider,
            global::System.String fullyQualifiedMetadataName,
            global::System.Func<global::Microsoft.CodeAnalysis.SyntaxNode, global::System.Threading.CancellationToken, global::System.Boolean> predicate,
            global::System.Func<global::Microsoft.CodeAnalysis.GeneratorAttributeSyntaxContext, global::System.Threading.CancellationToken, T> transform)
        {
            return
                syntaxValueProvider
                .CreateSyntaxProvider(
                    predicate,
                    (context, token) =>
                    {
                        ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token);
        
                        // If the syntax node doesn't have a declared symbol, just skip this node. This would be
                        // the case for eg. lambda attributes, but those are not supported by the MVVM Toolkit.
                        if (symbol is null)
                        {
                            return null;
                        }
        
                        // Skip symbols without the target attribute
                        if (!symbol.TryGetAttributeWithFullyQualifiedMetadataName(fullyQualifiedMetadataName, out AttributeData? attributeData))
                        {
                            return null;
                        }
        
                        // Edge case: if the symbol is a partial method, skip the implementation part and only process the partial method
                        // definition. This is needed because attributes will be reported as available on both the definition and the
                        // implementation part. To avoid generating duplicate files, we only give priority to the definition part.
                        // On Roslyn 4.3+, ForAttributeWithMetadataName will already only return the symbol the attribute was located on.
                        if (symbol is IMethodSymbol { IsPartialDefinition: false, PartialDefinitionPart: not null })
                        {
                            return null;
                        }
        
                        // Create the GeneratorAttributeSyntaxContext value to pass to the input transform. The attributes array
                        // will only ever have a single value, but that's fine with the attributes the various generators look for.
                        GeneratorAttributeSyntaxContext syntaxContext = new(
                            targetNode: context.Node,
                            targetSymbol: symbol,
                            semanticModel: context.SemanticModel,
                            attributes: ImmutableArray.Create(attributeData));
        
                        return new Option<T>(transform(syntaxContext, token));
                    })
                .Where(static item => item is not null)
                .Select(static (item, _) => item!.Value)!;
        }

	    internal const string ForAttributeWithMetadataNameText =
        """
        
        public static global::Microsoft.CodeAnalysis.IncrementalValuesProvider<T> ForAttributeWithMetadataName<T>(
            this global::Microsoft.CodeAnalysis.SyntaxValueProvider syntaxValueProvider,
            global::System.String fullyQualifiedMetadataName,
            global::System.Func<global::Microsoft.CodeAnalysis.SyntaxNode, global::System.Threading.CancellationToken, global::System.Boolean> predicate,
            global::System.Func<global::Microsoft.CodeAnalysis.GeneratorAttributeSyntaxContext, global::System.Threading.CancellationToken, T> transform)
        {
            return
                syntaxValueProvider
                .CreateSyntaxProvider(
                    predicate,
                    (context, token) =>
                    {
                        ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(context.Node, token);
        
                        // If the syntax node doesn't have a declared symbol, just skip this node. This would be
                        // the case for eg. lambda attributes, but those are not supported by the MVVM Toolkit.
                        if (symbol is null)
                        {
                            return null;
                        }
        
                        // Skip symbols without the target attribute
                        if (!symbol.TryGetAttributeWithFullyQualifiedMetadataName(fullyQualifiedMetadataName, out AttributeData? attributeData))
                        {
                            return null;
                        }
        
                        // Edge case: if the symbol is a partial method, skip the implementation part and only process the partial method
                        // definition. This is needed because attributes will be reported as available on both the definition and the
                        // implementation part. To avoid generating duplicate files, we only give priority to the definition part.
                        // On Roslyn 4.3+, ForAttributeWithMetadataName will already only return the symbol the attribute was located on.
                        if (symbol is IMethodSymbol { IsPartialDefinition: false, PartialDefinitionPart: not null })
                        {
                            return null;
                        }
        
                        // Create the GeneratorAttributeSyntaxContext value to pass to the input transform. The attributes array
                        // will only ever have a single value, but that's fine with the attributes the various generators look for.
                        GeneratorAttributeSyntaxContext syntaxContext = new(
                            targetNode: context.Node,
                            targetSymbol: symbol,
                            semanticModel: context.SemanticModel,
                            attributes: ImmutableArray.Create(attributeData));
        
                        return new Option<T>(transform(syntaxContext, token));
                    })
                .Where(static item => item is not null)
                .Select(static (item, _) => item!.Value)!;
        }
        """;

    }
}