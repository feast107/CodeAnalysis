using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Feast.CodeAnalysis.LiteralGenerator;

public static partial class General
{
    public static string FullName(this INamespaceSymbol symbol)
    {
        var name   = symbol.Name;
        var parent = symbol.ContainingNamespace;
        while (parent is { IsGlobalNamespace: false })
        {
            name   = parent.Name + '.' + name;
            parent = parent.ContainingNamespace;
        }

        return "global::" + name;
    }

    public static string FullName(this ITypeSymbol symbol) => symbol.ContainingSymbol switch
    {
        ITypeSymbol typeSymbol => typeSymbol.FullName() + '.' + symbol.Name,
        INamespaceSymbol namespaceSymbol => (namespaceSymbol.IsGlobalNamespace
                                                ? "global::"
                                                : namespaceSymbol.FullName() + '.')
                                            + symbol.Name,
        _ => symbol.Name
    };

    public static string FullName(this TypeSyntax? syntax, SemanticModel semanticModel)
    {
        if (syntax == null) return string.Empty;
        if (syntax.IsVar) return "var ";
        if (syntax is IdentifierNameSyntax { Identifier.ValueText: "nameof" })
            return "nameof";
        if (syntax is not GenericNameSyntax generic)
            return semanticModel
                .GetSymbolInfo(syntax)
                .Symbol?
                .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) + ' ';
        var symbol = semanticModel.GetTypeInfo(generic).Type;
        return (symbol is not null
                ? symbol.OriginalDefinition.FullName()
                : generic.Identifier.ValueText) + $"<{string.Join(",", generic.TypeArgumentList
                    .Arguments
                    .Select(x => x.FullName(semanticModel)))}>";
    }

    public static TypeSyntax ParseTypeName(this string name) => SyntaxFactory.ParseTypeName(name);

    public static MemberDeclarationSyntax CleanNamespace(this BaseTypeDeclarationSyntax type)
    {
        var ret = type.Parent;
        if (ret is ClassDeclarationSyntax) throw new ArgumentException("Should Not be on Nested Class");
        while (ret is BaseNamespaceDeclarationSyntax @namespace)
        {
            var tmp = NamespaceDeclaration(@namespace.Name);
            ret = tmp.AddMembers([@namespace]);
        }
        return (ret as MemberDeclarationSyntax)!;
    }

    public static MemberDeclarationSyntax FullNamespace(this MemberDeclarationSyntax type, ITypeSymbol symbol)
    {
        if (symbol.ContainingNamespace == null) return type;
        return symbol.ContainingNamespace.IsGlobalNamespace
            ? type
            : NamespaceDeclaration(IdentifierName(symbol.ContainingNamespace.FullName().Remove(0, 8)))
                .AddMembers([type]);
    }

    public static MemberDeclarationSyntax FullQualifiedMember(this MemberDeclarationSyntax member,
        SemanticModel semanticModel) => member switch
    {
        DelegateDeclarationSyntax delegateDeclaration =>
            delegateDeclaration
                .WithReturnType(delegateDeclaration.ReturnType.FullName(semanticModel).ParseTypeName())
                .WithParameterList(delegateDeclaration.ParameterList.FullQualifiedParameterList(semanticModel)),
        BaseTypeDeclarationSyntax baseTypeDeclaration => baseTypeDeclaration.FullQualifiedType(semanticModel),
        _                                             => member
    };
    
    public static BaseTypeDeclarationSyntax FullQualifiedType(this BaseTypeDeclarationSyntax type,
        SemanticModel semanticModel) => type switch
    {
        TypeDeclarationSyntax typeDeclaration => typeDeclaration.FullQualifiedClass(semanticModel),
        _                                     => type,
    };

    public static TypeDeclarationSyntax FullQualifiedClass(this TypeDeclarationSyntax type,
        SemanticModel semanticModel) =>
        type
            .WithBaseList(
                type.BaseList?.WithTypes(
                    type.BaseList.Types.Aggregate(new SeparatedSyntaxList<BaseTypeSyntax>(), (s, x) =>
                        s.Add(x.WithType(x.Type.FullName(semanticModel).ParseTypeName())))))
            .WithMembers(
                type.Members.Aggregate(new SyntaxList<MemberDeclarationSyntax>(), (s, m) =>
                    s.Add(m switch
                    {
                        FieldDeclarationSyntax field             => field.FullQualifiedField(semanticModel),
                        PropertyDeclarationSyntax property       => property.FullQualifiedProperty(semanticModel),
                        MethodDeclarationSyntax method           => method.FullQualifiedMethod(semanticModel),
                        ConstructorDeclarationSyntax constructor => constructor.FullQualifiedBaseMethod(semanticModel),
                        BaseMethodDeclarationSyntax method       => method.FullQualifiedBaseMethod(semanticModel),
                        BaseTypeDeclarationSyntax baseType       => baseType.FullQualifiedType(semanticModel),
                        _                                        => m
                    })));

    public static FieldDeclarationSyntax FullQualifiedField(this FieldDeclarationSyntax syntax,
        SemanticModel semanticModel)
    {
        return syntax
            .WithDeclaration(syntax.Declaration.FullQualifiedVariableDeclaration(semanticModel)
                .WithType(syntax.Declaration.Type.FullName(semanticModel).ParseTypeName())
                .WithVariables(syntax.Declaration.Variables.Aggregate(
                    new SeparatedSyntaxList<VariableDeclaratorSyntax>(),
                    (s, v) =>
                        s.Add(v.FullQualifiedVariable(semanticModel)))));
    }

    public static PropertyDeclarationSyntax FullQualifiedProperty(this PropertyDeclarationSyntax syntax,
        SemanticModel semanticModel) =>
        syntax
            .WithType(syntax.Type.FullName(semanticModel).ParseTypeName())
            .WithAccessorList(
                syntax.AccessorList?.WithAccessors(syntax.AccessorList.Accessors.Aggregate(
                    new SyntaxList<AccessorDeclarationSyntax>(),
                    (s, x) =>
                        s.Add(x
                            .WithBody(x.Body?
                                .WithStatements(x.Body.Statements.Aggregate(new SyntaxList<StatementSyntax>(),
                                    (l, ss) =>
                                        l.Add(ss.FullQualifiedStatement(semanticModel)))))
                            .WithExpressionBody(syntax.ExpressionBody?
                                .WithExpression(
                                    syntax.ExpressionBody.Expression.FullQualifiedExpression(semanticModel)))))));

    public static BaseMethodDeclarationSyntax FullQualifiedBaseMethod(this BaseMethodDeclarationSyntax method,
        SemanticModel semanticModel) =>
        method
            .WithParameterList(
                method.ParameterList.FullQualifiedParameterList(semanticModel))
            .WithExpressionBody(
                method.ExpressionBody?.WithExpression(
                    method.ExpressionBody.Expression.FullQualifiedExpression(semanticModel)))
            .WithBody(
                method.Body?.WithStatements(
                    method.Body.Statements.Aggregate(new SyntaxList<StatementSyntax>(), (l, s) =>
                        l.Add(s.FullQualifiedStatement(semanticModel)))));

    public static MethodDeclarationSyntax FullQualifiedMethod(this MethodDeclarationSyntax method,
        SemanticModel semanticModel) =>
        (method.FullQualifiedBaseMethod(semanticModel) as MethodDeclarationSyntax)!
        .WithReturnType(method.ReturnType.FullName(semanticModel).ParseTypeName());

    public static ConstructorDeclarationSyntax FullQualifiedConstructor(this ConstructorDeclarationSyntax constructor,
        SemanticModel semanticModel) =>
        (constructor.FullQualifiedBaseMethod(semanticModel) as ConstructorDeclarationSyntax)!;

    public static T FullQualifiedStatement<T>(this T syntax, SemanticModel semanticModel)
        where T : StatementSyntax
    {
        return (T)(syntax switch
        {
            BlockSyntax block =>
                (object)Block(block.Statements.Aggregate(new SyntaxList<StatementSyntax>(),
                    (s, x) => s.Add(x.FullQualifiedStatement(semanticModel)))),
            ExpressionStatementSyntax expressionStatement =>
                expressionStatement
                    .WithExpression(expressionStatement.Expression.FullQualifiedExpression(semanticModel)),
            IfStatementSyntax ifStatement =>
                ifStatement
                    .WithCondition(ifStatement.Condition.FullQualifiedExpression(semanticModel))
                    .WithStatement(ifStatement.Statement.FullQualifiedStatement(semanticModel)),
            LocalDeclarationStatementSyntax declaration =>
                declaration
                    .WithDeclaration(declaration.Declaration.FullQualifiedVariableDeclaration(semanticModel)
                        .WithType(declaration.Declaration.Type.FullName(semanticModel).ParseTypeName())
                        .WithVariables(declaration.Declaration.Variables.Aggregate(
                            new SeparatedSyntaxList<VariableDeclaratorSyntax>(),
                            (s, v) =>
                                s.Add(v.FullQualifiedVariable(semanticModel))))),
            ReturnStatementSyntax returnStatement =>
                returnStatement
                    .WithExpression(returnStatement.Expression?.FullQualifiedExpression(semanticModel)),
            SwitchStatementSyntax switchStatement =>
                switchStatement
                    .WithExpression(switchStatement.Expression.FullQualifiedExpression(semanticModel))
                    .WithSections(
                        switchStatement.Sections.Aggregate(new SyntaxList<SwitchSectionSyntax>(), (l, s) =>
                            l.Add(s.WithStatements(s.Statements.Aggregate(new SyntaxList<StatementSyntax>(), (ss, x) =>
                                    ss.Add(x.FullQualifiedStatement(semanticModel))
                                ))
                                .WithLabels(s.Labels.Aggregate(new SyntaxList<SwitchLabelSyntax>(), (ls, ll) =>
                                    ls.Add(ll switch
                                    {
                                        CaseSwitchLabelSyntax caseSwitchLabel =>
                                            caseSwitchLabel.WithValue(
                                                caseSwitchLabel.Value.FullQualifiedExpression(semanticModel)),
                                        _ => ll
                                    })))))),
            ThrowStatementSyntax throwStatement =>
                throwStatement
                    .WithExpression(throwStatement.Expression?.FullQualifiedExpression(semanticModel)),
            _ => syntax
        });
    }

    public static VariableDeclaratorSyntax FullQualifiedVariable(this VariableDeclaratorSyntax syntax,
        SemanticModel semanticModel)
    {
        if (syntax.Initializer is not null)
        {
            return syntax.WithInitializer(
                syntax.Initializer.WithValue(
                    syntax.Initializer.Value
                        .FullQualifiedExpression(semanticModel)));
        }

        return syntax;
    }

    public static ExpressionSyntax FullQualifiedExpression(this ExpressionSyntax syntax,
        SemanticModel semanticModel) =>
        syntax switch
        {
            ArrayCreationExpressionSyntax arrayCreationExpression =>
                arrayCreationExpression
                    .WithType(ArrayType(arrayCreationExpression.Type.FullName(semanticModel).ParseTypeName())),
            ArrayTypeSyntax arrayType =>
                arrayType
                    .WithElementType(arrayType.ElementType.FullName(semanticModel).ParseTypeName()),
            AssignmentExpressionSyntax assignmentExpression =>
                assignmentExpression
                    .WithLeft(assignmentExpression.Left.FullQualifiedExpression(semanticModel))
                    .WithRight(assignmentExpression.Right.FullQualifiedExpression(semanticModel)),
            BinaryExpressionSyntax binaryExpression =>
                binaryExpression
                    .WithLeft(binaryExpression.Left.FullQualifiedExpression(semanticModel))
                    .WithRight(binaryExpression.Right.FullQualifiedExpression(semanticModel)),
            CastExpressionSyntax castExpression =>
                castExpression
                    .WithType(castExpression.Type.FullName(semanticModel).ParseTypeName())
                    .WithExpression(castExpression.Expression.FullQualifiedExpression(semanticModel)),
            ConditionalExpressionSyntax conditionalExpression =>
                conditionalExpression
                    .WithCondition(conditionalExpression.Condition.FullQualifiedExpression(semanticModel))
                    .WithWhenTrue(conditionalExpression.WhenTrue.FullQualifiedExpression(semanticModel))
                    .WithWhenFalse(conditionalExpression.WhenFalse.FullQualifiedExpression(semanticModel)),
            GenericNameSyntax genericName =>
                genericName.WithTypeArgumentList(
                    genericName.TypeArgumentList.WithArguments(
                        genericName.TypeArgumentList.Arguments.Aggregate(new SeparatedSyntaxList<TypeSyntax>(),
                            (s, x) => s.Add(x.FullName(semanticModel).ParseTypeName())))),
            IdentifierNameSyntax identifierName =>
                identifierName.FullName(semanticModel).ParseTypeName(),
            InvocationExpressionSyntax invocation =>
                invocation
                    .WithExpression(invocation.Expression.FullQualifiedExpression(semanticModel))
                    .WithArgumentList(invocation.ArgumentList.FullQualifiedArgumentList(semanticModel)),
            MemberAccessExpressionSyntax memberAccess =>
                memberAccess
                    .WithName(memberAccess.Name is not GenericNameSyntax genericName
                        ? memberAccess.Name
                        : genericName.WithTypeArgumentList(
                            genericName.TypeArgumentList.WithArguments(
                                genericName.TypeArgumentList.Arguments.Aggregate(
                                    new SeparatedSyntaxList<TypeSyntax>(), (s, x) =>
                                        s.Add(x.FullName(semanticModel).ParseTypeName())))))
                    .WithExpression(memberAccess.Expression.FullQualifiedExpression(semanticModel)),
            ObjectCreationExpressionSyntax objectCreation =>
                objectCreation
                    .WithType(objectCreation.Type.FullName(semanticModel).ParseTypeName())
                    .WithArgumentList(objectCreation.ArgumentList?.FullQualifiedArgumentList(semanticModel)),
            ParenthesizedExpressionSyntax parenthesizedExpression =>
                parenthesizedExpression.WithExpression(
                    parenthesizedExpression.Expression.FullQualifiedExpression(semanticModel)),
            ParenthesizedLambdaExpressionSyntax lambdaExpression =>
                lambdaExpression
                    .WithExpressionBody(lambdaExpression.ExpressionBody?.FullQualifiedExpression(semanticModel)),
            SimpleLambdaExpressionSyntax simpleLambda =>
                simpleLambda
                    .WithBlock(simpleLambda.Block?.FullQualifiedStatement(semanticModel))
                    .WithExpressionBody(simpleLambda.ExpressionBody?.FullQualifiedExpression(semanticModel)),
            ThrowExpressionSyntax throwExpression =>
                throwExpression
                    .WithExpression(throwExpression.Expression.FullQualifiedExpression(semanticModel)),
            _ => syntax
        };


    public static ParameterListSyntax FullQualifiedParameterList(this ParameterListSyntax syntax,
        SemanticModel semanticModel) =>
        ParameterList(
            syntax.Parameters.Aggregate(new SeparatedSyntaxList<ParameterSyntax>(),
                (l, x) => l.Add(
                    x.WithType(x.Type.FullName(semanticModel).ParseTypeName())))
        );
 
    
    public static ArgumentListSyntax FullQualifiedArgumentList(this ArgumentListSyntax syntax,
        SemanticModel semanticModel) =>
        syntax
            .WithArguments(syntax.Arguments.Aggregate(
                new SeparatedSyntaxList<ArgumentSyntax>(),
                (s, x) =>
                    s.Add(x.WithExpression(x.Expression.FullQualifiedExpression(semanticModel)))));

    public static VariableDeclarationSyntax FullQualifiedVariableDeclaration(this VariableDeclarationSyntax syntax,
        SemanticModel semanticModel) =>
        syntax
            .WithType(syntax.Type.FullName(semanticModel).ParseTypeName())
            .WithVariables(syntax.Variables.Aggregate(
                new SeparatedSyntaxList<VariableDeclaratorSyntax>(),
                (s, v) =>
                    s.Add(v.FullQualifiedVariable(semanticModel))));
}