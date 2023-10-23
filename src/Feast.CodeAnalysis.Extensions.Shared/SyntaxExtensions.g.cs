using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class SyntaxExtensions
    {
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> AllAttributes(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax)
        {
            return syntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }

	    internal const string AllAttributesText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> AllAttributes(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax)
        {
            return syntax.AttributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }
        """;

        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetSpecifiedAttributes(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            foreach (var attributeSyntax in syntax.AllAttributes())
            {
                if (semanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
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
            global::System.String fullAttributeName)
        {
            foreach (var attributeSyntax in syntax.AllAttributes())
            {
                if (semanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    continue;
        
                string attributeName = attributeSymbol.ContainingType.ToDisplayString();
        
                if (attributeName == fullAttributeName)
                    yield return attributeSyntax;
            }
        }
        """;

        public static global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax? GetSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            foreach (var attributeSyntax in syntax.GetSpecifiedAttributes(semanticModel, fullAttributeName))
            {
                return attributeSyntax;
            }
            return null;
        }

	    internal const string GetSpecifiedAttributeText =
        """
        
        public static global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax? GetSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            foreach (var attributeSyntax in syntax.GetSpecifiedAttributes(semanticModel, fullAttributeName))
            {
                return attributeSyntax;
            }
            return null;
        }
        """;

        public static global::System.Boolean HasSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
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