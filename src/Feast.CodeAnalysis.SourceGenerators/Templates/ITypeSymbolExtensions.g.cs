using System;
using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class ITypeSymbolExtensions
    {
        /// <summary>
        /// Checks whether or not a given type symbol has a specified fully qualified metadata name.
        /// </summary>
        /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
        /// <param name="name">The full name to check.</param>
        /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
        public static global::System.Boolean HasFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.String name)
        {
            using global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder = global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char>.Rent();
        
            symbol.AppendFullyQualifiedMetadataName(in builder);
        
            return builder.WrittenSpan.SequenceEqual(name.AsSpan());
        }

	    internal const string HasFullyQualifiedMetadataNameText =
        """
        
        /// <summary>
        /// Checks whether or not a given type symbol has a specified fully qualified metadata name.
        /// </summary>
        /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
        /// <param name="name">The full name to check.</param>
        /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
        public static global::System.Boolean HasFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.String name)
        {
            using global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder = global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char>.Rent();
        
            symbol.AppendFullyQualifiedMetadataName(in builder);
        
            return builder.WrittenSpan.SequenceEqual(name.AsSpan());
        }
        """;

        /// <summary>
        /// Appends the fully qualified metadata name for a given symbol to a target builder.
        /// </summary>
        /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
        /// <param name="builder">The target <see cref="ImmutableArrayBuilder{T}"/> instance.</param>
        private static void AppendFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, 
            in global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder)
        {
            static void BuildFrom(global::Microsoft.CodeAnalysis.ISymbol? symbol, in global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder)
            {
                switch (symbol)
                {
                    // Namespaces that are nested also append a leading '.'
                    case global::Microsoft.CodeAnalysis.INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                        BuildFrom(symbol.ContainingNamespace, in builder);
                        builder.Add('.');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Other namespaces (ie. the one right before global) skip the leading '.'
                    case global::Microsoft.CodeAnalysis.INamespaceSymbol { IsGlobalNamespace: false }:
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Types with no namespace just have their metadata name directly written
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.INamespaceSymbol { IsGlobalNamespace: true } }:
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Types with a containing non-global namespace also append a leading '.'
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol }:
                        BuildFrom(namespaceSymbol, in builder);
                        builder.Add('.');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Nested types append a leading '+'
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.ITypeSymbol typeSymbol }:
                        BuildFrom(typeSymbol, in builder);
                        builder.Add('+');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
                    default:
                        break;
                }
            }
        
            BuildFrom(symbol, in builder);
        }

	    internal const string AppendFullyQualifiedMetadataNameText =
        """
        
        /// <summary>
        /// Appends the fully qualified metadata name for a given symbol to a target builder.
        /// </summary>
        /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
        /// <param name="builder">The target <see cref="ImmutableArrayBuilder{T}"/> instance.</param>
        private static void AppendFullyQualifiedMetadataName(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, 
            in global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder)
        {
            static void BuildFrom(global::Microsoft.CodeAnalysis.ISymbol? symbol, in global::Microsoft.CodeAnalysis.ImmutableArrayBuilder<global::System.Char> builder)
            {
                switch (symbol)
                {
                    // Namespaces that are nested also append a leading '.'
                    case global::Microsoft.CodeAnalysis.INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                        BuildFrom(symbol.ContainingNamespace, in builder);
                        builder.Add('.');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Other namespaces (ie. the one right before global) skip the leading '.'
                    case global::Microsoft.CodeAnalysis.INamespaceSymbol { IsGlobalNamespace: false }:
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Types with no namespace just have their metadata name directly written
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.INamespaceSymbol { IsGlobalNamespace: true } }:
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Types with a containing non-global namespace also append a leading '.'
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.INamespaceSymbol namespaceSymbol }:
                        BuildFrom(namespaceSymbol, in builder);
                        builder.Add('.');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
        
                    // Nested types append a leading '+'
                    case global::Microsoft.CodeAnalysis.ITypeSymbol { ContainingSymbol: global::Microsoft.CodeAnalysis.ITypeSymbol typeSymbol }:
                        BuildFrom(typeSymbol, in builder);
                        builder.Add('+');
                        builder.AddRange(symbol.MetadataName.AsSpan());
                        break;
                    default:
                        break;
                }
            }
        
            BuildFrom(symbol, in builder);
        }
        """;

        public static bool IsAssignableTo(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.Type type) => symbol.ToTypeInfo().IsAssignableTo(type);
        
        public static bool IsAssignableTo(this global::System.Type type, global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => symbol.ToTypeInfo().IsAssignableFrom(type);

	    internal const string IsAssignableToText =
        """
        
        public static bool IsAssignableTo(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.Type type) => symbol.ToTypeInfo().IsAssignableTo(type);
        
        public static bool IsAssignableTo(this global::System.Type type, global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => symbol.ToTypeInfo().IsAssignableFrom(type);
        """;

        public static bool IsAssignableFrom(this global::System.Type type, global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => symbol.ToTypeInfo().IsAssignableTo(type);
        
        public static bool IsAssignableFrom(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.Type type) => symbol.ToTypeInfo().IsAssignableTo(type);

	    internal const string IsAssignableFromText =
        """
        
        public static bool IsAssignableFrom(this global::System.Type type, global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => symbol.ToTypeInfo().IsAssignableTo(type);
        
        public static bool IsAssignableFrom(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol, global::System.Type type) => symbol.ToTypeInfo().IsAssignableTo(type);
        """;

        public static global::Feast.CodeAnalysis.Utils.TypeInfo ToTypeInfo(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => global::Feast.CodeAnalysis.Utils.TypeInfo.FromSymbol(symbol);

	    internal const string ToTypeInfoText =
        """
        
        public static global::Feast.CodeAnalysis.Utils.TypeInfo ToTypeInfo(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => global::Feast.CodeAnalysis.Utils.TypeInfo.FromSymbol(symbol);
        """;

        public static bool IsJsonBool(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => 
            symbol.SpecialType == global::Microsoft.CodeAnalysis.SpecialType.System_Boolean;

	    internal const string IsJsonBoolText =
        """
        
        public static bool IsJsonBool(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) => 
            symbol.SpecialType == global::Microsoft.CodeAnalysis.SpecialType.System_Boolean;
        """;

        public static bool IsJsonNumber(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) =>
        symbol is { SpecialType: >= global::Microsoft.CodeAnalysis.SpecialType.System_SByte and <= global::Microsoft.CodeAnalysis.SpecialType.System_Single };
        

	    internal const string IsJsonNumberText =
        """
        
        public static bool IsJsonNumber(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) =>
        symbol is { SpecialType: >= global::Microsoft.CodeAnalysis.SpecialType.System_SByte and <= global::Microsoft.CodeAnalysis.SpecialType.System_Single };
        
        """;

        public static bool IsJsonString(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) =>
            symbol.SpecialType is global::Microsoft.CodeAnalysis.SpecialType.System_String or global::Microsoft.CodeAnalysis.SpecialType.System_Char;

	    internal const string IsJsonStringText =
        """
        
        public static bool IsJsonString(this global::Microsoft.CodeAnalysis.ITypeSymbol symbol) =>
            symbol.SpecialType is global::Microsoft.CodeAnalysis.SpecialType.System_String or global::Microsoft.CodeAnalysis.SpecialType.System_Char;
        """;

    }
}