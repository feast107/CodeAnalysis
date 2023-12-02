
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class TypedConstantExtensions
    {
        public static global::Microsoft.CodeAnalysis.INamedTypeSymbol? GetArgumentType(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Type ? constant.Value as global::Microsoft.CodeAnalysis.INamedTypeSymbol : null;
        }

	    internal const string GetArgumentTypeText =
        """
        
        public static global::Microsoft.CodeAnalysis.INamedTypeSymbol? GetArgumentType(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Type ? constant.Value as global::Microsoft.CodeAnalysis.INamedTypeSymbol : null;
        }
        """;

    }
}