using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class TypedConstantExtensions
    {
        public static global::Microsoft.CodeAnalysis.INamedTypeSymbol? GetArgumentType(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Type ? constant.Value as global::Microsoft.CodeAnalysis.INamedTypeSymbol : throw new global::System.ArgumentException("constant is not a type");
        }

	    internal const string GetArgumentTypeText =
        """
        
        public static global::Microsoft.CodeAnalysis.INamedTypeSymbol? GetArgumentType(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Type ? constant.Value as global::Microsoft.CodeAnalysis.INamedTypeSymbol : throw new global::System.ArgumentException("constant is not a type");
        }
        """;

        public static string? GetArgumentString(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? constant.Value as string : throw new global::System.ArgumentException("constant is not a string");
        }

	    internal const string GetArgumentStringText =
        """
        
        public static string? GetArgumentString(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? constant.Value as string : throw new global::System.ArgumentException("constant is not a string");
        }
        """;

        public static T? GetArgumentEnum<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : global::System.Enum
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Enum ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not an enum");
        }

	    internal const string GetArgumentEnumText =
        """
        
        public static T? GetArgumentEnum<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : global::System.Enum
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Enum ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not an enum");
        }
        """;

        public static T? GetArgumentPrimitive<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : struct
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not a primitive");
        }

	    internal const string GetArgumentPrimitiveText =
        """
        
        public static T? GetArgumentPrimitive<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : struct
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not a primitive");
        }
        """;

        public static T?[] GetArgumentArray<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? constant.Values.Select(x => (T?)x.Value).ToArray() : throw new global::System.ArgumentException("constant is not an array");
        }

	    internal const string GetArgumentArrayText =
        """
        
        public static T?[] GetArgumentArray<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant)
        {
            return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? constant.Values.Select(x => (T?)x.Value).ToArray() : throw new global::System.ArgumentException("constant is not an array");
        }
        """;

    }
}