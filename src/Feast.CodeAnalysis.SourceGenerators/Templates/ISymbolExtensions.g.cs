﻿using System.Linq;
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

        public static bool IsInitOnly(this global::Microsoft.CodeAnalysis.IPropertySymbol symbol)
        {
            return !symbol.IsReadOnly && symbol.SetMethod!.IsInitOnly;
        }

	    internal const string IsInitOnlyText =
        """
        
        public static bool IsInitOnly(this global::Microsoft.CodeAnalysis.IPropertySymbol symbol)
        {
            return !symbol.IsReadOnly && symbol.SetMethod!.IsInitOnly;
        }
        """;

        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.SyntaxKind> GetSyntaxKind(this global::Microsoft.CodeAnalysis.Accessibility accessibility)
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
                    throw new System.ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
            }
        
        }

	    internal const string GetSyntaxKindText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.SyntaxKind> GetSyntaxKind(this global::Microsoft.CodeAnalysis.Accessibility accessibility)
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
                    throw new System.ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
            }
        
        }
        """;

        #if !ROSLYN_4_3_1_OR_GREATER
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
        #endif

	    internal const string TryGetAttributeWithFullyQualifiedMetadataNameText =
        """
        
        #if !ROSLYN_4_3_1_OR_GREATER
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
        #endif
        """;

    }
}