using System.Linq;
#nullable enable
namespace Microsoft.CodeAnalysis
{
    internal static class SyntaxExtensions
    {
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetAllAttributes(this global::Microsoft.CodeAnalysis.SyntaxNode syntax)
        {
            var attributeLists = syntax switch
            {
                global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnitSyntax     => compilationUnitSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax memberDeclarationSyntax => memberDeclarationSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax lambdaExpressionSyntax   => lambdaExpressionSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterSyntax baseParameterSyntax         => baseParameterSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statementSyntax                 => statementSyntax.AttributeLists,
                _                                                                                            => throw new global::System.NotSupportedException($"{syntax.GetType()} has no attribute")
            };
            return attributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }

	    internal const string GetAllAttributesText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetAllAttributes(this global::Microsoft.CodeAnalysis.SyntaxNode syntax)
        {
            var attributeLists = syntax switch
            {
                global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnitSyntax     => compilationUnitSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax memberDeclarationSyntax => memberDeclarationSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.LambdaExpressionSyntax lambdaExpressionSyntax   => lambdaExpressionSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.BaseParameterSyntax baseParameterSyntax         => baseParameterSyntax.AttributeLists,
                global::Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statementSyntax                 => statementSyntax.AttributeLists,
                _                                                                                            => throw new global::System.NotSupportedException($"{syntax.GetType()} has no attribute")
            };
            return attributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
        }
        """;

        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetSpecifiedAttributes(this global::Microsoft.CodeAnalysis.SyntaxNode syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName, 
            global::System.Threading.CancellationToken cancellationToken = default (global::System.Threading.CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetAllAttributes())
            {
                if(cancellationToken.IsCancellationRequested)
                    yield break;
                if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not global::Microsoft.CodeAnalysis.IMethodSymbol attributeSymbol)
                    continue;
        
                string attributeName = attributeSymbol.ContainingType.ToDisplayString();
        
                if (attributeName == fullAttributeName)
                    yield return attributeSyntax;
            }
        }

	    internal const string GetSpecifiedAttributesText =
        """
        
        public static global::System.Collections.Generic.IEnumerable<global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax> GetSpecifiedAttributes(this global::Microsoft.CodeAnalysis.SyntaxNode syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName, 
            global::System.Threading.CancellationToken cancellationToken = default (global::System.Threading.CancellationToken))
        {
            foreach (var attributeSyntax in syntax.GetAllAttributes())
            {
                if(cancellationToken.IsCancellationRequested)
                    yield break;
                if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not global::Microsoft.CodeAnalysis.IMethodSymbol attributeSymbol)
                    continue;
        
                string attributeName = attributeSymbol.ContainingType.ToDisplayString();
        
                if (attributeName == fullAttributeName)
                    yield return attributeSyntax;
            }
        }
        """;

        public static global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax? GetSpecifiedAttribute(this global::Microsoft.CodeAnalysis.SyntaxNode syntax, 
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

	    internal const string GetSpecifiedAttributeText =
        """
        
        public static global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax? GetSpecifiedAttribute(this global::Microsoft.CodeAnalysis.SyntaxNode syntax, 
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

        public static global::System.Boolean HasSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            return syntax.GetSpecifiedAttribute(semanticModel, fullAttributeName) is not null;
        }

	    internal const string HasSpecifiedAttributeText =
        """
        
        public static global::System.Boolean HasSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax syntax, 
            global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
            global::System.String fullAttributeName)
        {
            return syntax.GetSpecifiedAttribute(semanticModel, fullAttributeName) is not null;
        }
        """;

        public static global::System.String? GetArgumentString(this global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax syntax)
        {
            if (syntax.Expression is not global::Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpressionSyntax) return null;
            if (!literalExpressionSyntax.IsKind(global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression)) return null;
            return literalExpressionSyntax.Token.ValueText;
        }

	    internal const string GetArgumentStringText =
        """
        
        public static global::System.String? GetArgumentString(this global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax syntax)
        {
            if (syntax.Expression is not global::Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpressionSyntax) return null;
            if (!literalExpressionSyntax.IsKind(global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression)) return null;
            return literalExpressionSyntax.Token.ValueText;
        }
        """;

    }
}