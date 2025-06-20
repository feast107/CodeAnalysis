﻿using System;
using System.Linq;
using Feast.CodeAnalysis;

#nullable enable
namespace Microsoft.CodeAnalysis;

[Literal("Feast.CodeAnalysis.SyntaxExtensions")]
internal static partial class SyntaxExtensions
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

    public static global::System.Boolean HasSpecifiedAttribute(this global::Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax syntax, 
        global::Microsoft.CodeAnalysis.SemanticModel semanticModel, 
        global::System.String fullAttributeName)
    {
        return syntax.GetSpecifiedAttribute(semanticModel, fullAttributeName) is not null;
    }


    public static global::System.String? GetArgumentString(this global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax syntax)
    {
        if (syntax.Expression is not global::Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax literalExpressionSyntax) return null;
        if (!literalExpressionSyntax.IsKind(global::Microsoft.CodeAnalysis.CSharp.SyntaxKind.StringLiteralExpression)) return null;
        return literalExpressionSyntax.Token.ValueText;
    }


    public static global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax? GetArgumentType(this global::Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax syntax)
    {
        if (syntax.Expression is not global::Microsoft.CodeAnalysis.CSharp.Syntax.TypeOfExpressionSyntax typeOfExpression) return null;
        return typeOfExpression.Type;
    }


    public static Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax ToNameSyntax(this string text, int offset = 0, bool consumeFullText = true)
    {
        return global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(text, offset, consumeFullText);
    }

    public static Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax ToNamespaceDeclaration(this Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax syntax)
    {
        return global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NamespaceDeclaration(syntax);
    }


    public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax ToClassDeclaration(this string identifier)
    {
        return global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ClassDeclaration(identifier);
    }


    public static global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax AddMembers(this global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax syntax, params global::System.String[] members)
    {
        return syntax.AddMembers(members.Select(x => global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseMemberDeclaration(x) ?? throw new global::System.Exception($"Text : {x} , Parse failed")).ToArray());
    }
        
    public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax AddMembers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax syntax, params global::System.String[] members)
    {
        return syntax.AddMembers(members.Select(x => global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseMemberDeclaration(x) ?? throw new global::System.Exception($"Text : {x} , Parse failed")).ToArray());
    }


    public static global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax AddUsings(this global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax syntax, params global::System.String[] usings)
    {
        return syntax.AddUsings(usings.Select(x => global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.UsingDirective(global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(x))).ToArray());
    }
 
    public static global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax AddNamespace(this global::Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax syntax, global::System.String @namespace)
    {
        return syntax.AddMembers(global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.NamespaceDeclaration(global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.ParseName(@namespace)));
    }


    public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax AddModifiers(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax syntax, params global::Microsoft.CodeAnalysis.CSharp.SyntaxKind[] items)
    {
        return syntax.AddModifiers(items.Select(x => global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.Token(x)).ToArray());
    }

    public static Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax AddBaseListTypes(this Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax syntax, params string[] identifiers)
    {
        return syntax.AddBaseListTypes(identifiers.Select(x => global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.SimpleBaseType(global::Microsoft.CodeAnalysis.CSharp.SyntaxFactory.IdentifierName(x))).ToArray());
    }
}