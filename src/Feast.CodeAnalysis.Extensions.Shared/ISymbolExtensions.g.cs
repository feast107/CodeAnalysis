using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class ISymbolExtensions
    {
        public static global::System.String GetFullyQualifiedName(this global::Microsoft.CodeAnalysis.ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

	    internal const string GetFullyQualifiedNameText =
        """
        
        public static global::System.String GetFullyQualifiedName(this global::Microsoft.CodeAnalysis.ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
        """;

        /// <summary>
        /// Tries to get an attribute with the specified fully qualified metadata name.
        /// </summary>
        /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
        /// <param name="name">The attribute name to look for.</param>
        /// <param name="attributeData">The resulting attribute, if it was found.</param>
        /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
        public static bool TryGetAttributeWithFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ISymbol symbol, 
            global::System.String name, 
            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Microsoft.CodeAnalysis.AttributeData? attributeData)
        {
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass?.HasFullyQualifiedMetadataName(name) == true)
                {
                    attributeData = attribute;
        
                    return true;
                }
            }
        
            attributeData = null;
        
            return false;
        }

	    internal const string TryGetAttributeWithFullyQualifiedMetadataNameText =
        """
        
        /// <summary>
        /// Tries to get an attribute with the specified fully qualified metadata name.
        /// </summary>
        /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
        /// <param name="name">The attribute name to look for.</param>
        /// <param name="attributeData">The resulting attribute, if it was found.</param>
        /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
        public static bool TryGetAttributeWithFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ISymbol symbol, 
            global::System.String name, 
            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out global::Microsoft.CodeAnalysis.AttributeData? attributeData)
        {
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass?.HasFullyQualifiedMetadataName(name) == true)
                {
                    attributeData = attribute;
        
                    return true;
                }
            }
        
            attributeData = null;
        
            return false;
        }
        """;

    }
}