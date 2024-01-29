using System;
using Feast.CodeAnalysis;

#nullable enable
namespace Microsoft.CodeAnalysis;

[Literal("Feast.CodeAnalysis.ISymbolExtensions")]
internal static class ISymbolExtensions
{
    public static bool Is(this global::Microsoft.CodeAnalysis.ISymbol symbol,
        global::Microsoft.CodeAnalysis.ISymbol other) =>
        global::Microsoft.CodeAnalysis.SymbolEqualityComparer.Default.Equals(symbol, other);

    public static string GlobalName(this global::Microsoft.CodeAnalysis.ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static global::System.String GetFullyQualifiedName(this global::Microsoft.CodeAnalysis.ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static bool IsInitOnly(this global::Microsoft.CodeAnalysis.IPropertySymbol symbol) => !symbol.IsReadOnly && symbol.SetMethod!.IsInitOnly;


    public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.SyntaxKind>
        GetSyntaxKind(this global::Microsoft.CodeAnalysis.Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.Public:
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword;
                yield break;
            case Accessibility.Protected:
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword;
                yield break;
            case Accessibility.Internal:
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword;
                yield break;
            case Accessibility.Private:
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword;
                yield break;
            case Accessibility.ProtectedOrInternal:
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword;
                yield return global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword;
                yield break;
            default:
                throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
        }
    }


#if !ROSLYN_4_3_1_OR_GREATER
    /// <summary>
    /// Tries to get an attribute with the specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The attribute name to look for.</param>
    /// <param name="attributeData">The resulting attribute, if it was found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has an attribute with the specified name.</returns>
    public static bool TryGetAttributeWithFullyQualifiedMetadataName(
        this ISymbol symbol,
        string name,
        [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
        out AttributeData? attributeData)
    {
        foreach (var attribute in symbol.GetAttributes())
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
#endif
}