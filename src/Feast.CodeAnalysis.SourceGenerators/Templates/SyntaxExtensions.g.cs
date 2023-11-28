#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Feast.CodeAnalysis.SourceGenerators.Templates
{
    internal static class SyntaxExtensions
    {
        public static IEnumerable<AttributeSyntax> GetAllAttributes(this TypeDeclarationSyntax syntax)
        {
            return syntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }

	    internal const string GetAllAttributesText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetAllAttributes(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax)
        {
            return syntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }
        """;

        public static IEnumerable<AttributeSyntax> GetSpecifiedAttributes(this TypeDeclarationSyntax syntax, 
            SemanticModel semanticModel, 
            String fullAttributeName, 
            CancellationToken cancellationToken = default (CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetAllAttributes())
            {
                if(cancellationToken.IsCancellationRequested)
                    yield break;
                if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
        
                string attributeName = attributeSymbol.ContainingType.ToDisplayString();
        
                if (attributeName == fullAttributeName)
                    yield return attributeSyntax;
            }
        }

	    internal const string GetSpecifiedAttributesText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetSpecifiedAttributes(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName, 
            global::System.Threading.CancellationToken cancellationToken = default (global::System.Threading.CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetAllAttributes())
            {
                if(cancellationToken.IsCancellationRequested)
                    yield break;
                if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
        
                string attributeName = attributeSymbol.ContainingType.ToDisplayString();
        
                if (attributeName == fullAttributeName)
                    yield return attributeSyntax;
            }
        }
        """;

        public static AttributeSyntax? GetSpecifiedAttribute(this TypeDeclarationSyntax syntax, 
            SemanticModel semanticModel, 
            String fullAttributeName,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetSpecifiedAttributes(semanticModel, fullAttributeName, cancellationToken))
            {
                return attributeSyntax;
            }
            return null;
        }

	    internal const string GetSpecifiedAttributeText =
        """
        
        public static global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax? GetSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName,
            global::System.Threading.CancellationToken cancellationToken = default (global::System.Threading.CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetSpecifiedAttributes(semanticModel, fullAttributeName, cancellationToken))
            {
                return attributeSyntax;
            }
            return null;
        }
        """;

        public static Boolean HasSpecifiedAttribute(this TypeDeclarationSyntax syntax, 
            SemanticModel semanticModel, 
            String fullAttributeName)
        {
            return syntax.GetSpecifiedAttribute(semanticModel, fullAttributeName) is not null;
        }

	    internal const string HasSpecifiedAttributeText =
        """
        
        public static global::System.Boolean HasSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            return syntax.GetSpecifiedAttribute(semanticModel, fullAttributeName) is not null;
        }
        """;

    }
}