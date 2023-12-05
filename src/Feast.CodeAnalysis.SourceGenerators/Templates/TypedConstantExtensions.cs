using System.Linq;
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

        public static string? GetArgumentString(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? constant.Value as string : null;
        }

	    internal const string GetArgumentStringText =
        """
        
        public static string? GetArgumentString(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? constant.Value as string : null;
        }
        """;

        public static T? GetArgumentEnum<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : global::System.Enum
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? (T?)constant.Value : default;
        }

	    internal const string GetArgumentEnumText =
        """
        
        public static T? GetArgumentEnum<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : global::System.Enum
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? (T?)constant.Value : default;
        }
        """;

        public static T?[]? GetArgumentArray<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? constant.Values.Select(x => (T?)x.Value).ToArray() : null;
        }

	    internal const string GetArgumentArrayText =
        """
        
        public static T?[]? GetArgumentArray<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? constant.Values.Select(x => (T?)x.Value).ToArray() : null;
        }
        """;

    }
}