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
            using global::Microsoft.CodeAnalysis.PooledObjects.ImmutableArrayBuilder<global::System.Char> builder = global::Microsoft.CodeAnalysis.PooledObjects.ImmutableArrayBuilder<global::System.Char>.Rent();
        
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
            using global::Microsoft.CodeAnalysis.PooledObjects.ImmutableArrayBuilder<global::System.Char> builder = global::Microsoft.CodeAnalysis.PooledObjects.ImmutableArrayBuilder<global::System.Char>.Rent();
        
            symbol.AppendFullyQualifiedMetadataName(in builder);
        
            return builder.WrittenSpan.SequenceEqual(name.AsSpan());
        }
        """;

    }
}