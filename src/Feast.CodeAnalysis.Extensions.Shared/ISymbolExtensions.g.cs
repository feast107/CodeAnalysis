using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class ISymbolExtensions
    {
        public static global::System.String AllAttributes(this global::Microsoft.CodeAnalysis.ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

	    internal const string GetFullyQualifiedNameText =
        """
        
        public static global::System.String AllAttributes(this global::Microsoft.CodeAnalysis.ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
        """;

    }
}