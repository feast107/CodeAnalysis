using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Feast.CodeAnalysis.LiteralGenerator;

public static class General
{
    public static TypeDeclarationSyntax FullQualifiedName(this TypeDeclarationSyntax syntax,
        SemanticModel semanticModel)
    {
        return syntax;
    }

    public static MethodDeclarationSyntax FullQualifiedTypeNameMethod(this MethodDeclarationSyntax method,
        SemanticModel semanticModel)
    {
        return method.WithReturnType(
            ParseTypeName(
                semanticModel.GetTypeInfo(method.ReturnType).Type!.ToDisplayString(SymbolDisplayFormat
                    .FullyQualifiedFormat)));
    }
}