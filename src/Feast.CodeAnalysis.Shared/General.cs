using System;
using System.Collections.Generic;
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
        switch (syntax)
        {
            case ArrayTypeSyntax arrayType:
                return arrayType.ElementType.FullName(semanticModel)
                       + string.Concat(arrayType.RankSpecifiers.Select(
                           x => x.WithSizes(x.Sizes
                               .Select(e => e.FullQualifiedExpression(semanticModel))
                               .ToSeparatedSyntaxList())));
            case GenericNameSyntax genericName:
                var generic = semanticModel.GetTypeInfo(genericName).Type;
                return (generic is not null
                        ? generic.OriginalDefinition.FullName()
                        : genericName.Identifier.ValueText) + $"<{string.Join(",", genericName.TypeArgumentList
                            .Arguments
                            .Select(x => x.FullName(semanticModel)))}>";
        }

        var symbol = semanticModel.GetSymbolInfo(syntax).Symbol;
        var name = symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (symbol is IFieldSymbol && symbol.ContainingType.TypeKind == TypeKind.Enum)
            name = symbol.ContainingType.FullName() + '.' + name;
        if (syntax is NullableTypeSyntax && name?.EndsWith("?") is false) name += "?";
        return name ?? string.Empty;
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

    public static CompilationUnitSyntax WithUsing(this MemberDeclarationSyntax member, CompilationUnitSyntax syntax) =>
        syntax
            .WithMembers([])
            .AddMembers(member);

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

    public static SyntaxList<TypeParameterConstraintClauseSyntax> FullQualifiedConstraintClauses(
        this SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses,
        SemanticModel semanticModel) =>
        constraintClauses
            .Select(c =>
                c.WithConstraints(c.Constraints
                    .Select(x => x switch
                    {
                        TypeConstraintSyntax typeConstraint => typeConstraint
                            .WithType(typeConstraint.Type.FullName(semanticModel).ParseTypeName()),
                        _ => x
                    })
                    .ToSeparatedSyntaxList()))
            .ToSyntaxList();

    public static TypeDeclarationSyntax FullQualifiedClass(this TypeDeclarationSyntax type,
        SemanticModel semanticModel) =>
        type
            .WithConstraintClauses(
                type.ConstraintClauses.FullQualifiedConstraintClauses(semanticModel))
            .WithBaseList(
                type.BaseList?.WithTypes(
                    type.BaseList.Types
                        .Select(x => x.WithType(x.Type.FullName(semanticModel).ParseTypeName()))
                        .ToSeparatedSyntaxList()))
            .WithMembers(
                type.Members
                    .Select(m => m switch
                    {
                        FieldDeclarationSyntax field        => field.FullQualifiedField(semanticModel),
                        PropertyDeclarationSyntax property  => property.FullQualifiedProperty(semanticModel),
                        MethodDeclarationSyntax method      => method.FullQualifiedMethod(semanticModel),
                        OperatorDeclarationSyntax @operator => @operator.FullQualifiedOperator(semanticModel),
                        ConversionOperatorDeclarationSyntax conversion => conversion.FullQualifiedOperator(
                            semanticModel),
                        ConstructorDeclarationSyntax constructor => constructor.FullQualifiedBaseMethod(semanticModel),
                        BaseMethodDeclarationSyntax method       => method.FullQualifiedBaseMethod(semanticModel),
                        BaseTypeDeclarationSyntax baseType       => baseType.FullQualifiedType(semanticModel),
                        _                                        => m
                    })
                    .ToSyntaxList());

    public static FieldDeclarationSyntax FullQualifiedField(this FieldDeclarationSyntax field,
        SemanticModel semanticModel) =>
        field
            .WithDeclaration(field.Declaration.FullQualifiedVariableDeclaration(semanticModel)
                .WithType(field.Declaration.Type.FullName(semanticModel).ParseTypeName())
                .WithVariables(field.Declaration.Variables
                    .Select(x => x.FullQualifiedVariable(semanticModel))
                    .ToSeparatedSyntaxList()));

    public static PropertyDeclarationSyntax FullQualifiedProperty(this PropertyDeclarationSyntax property,
        SemanticModel semanticModel) =>
        property
            .WithType(property.Type.FullName(semanticModel).ParseTypeName())
            .WithExpressionBody(
                property.ExpressionBody?.WithExpression(
                    property.ExpressionBody.Expression.FullQualifiedExpression(semanticModel)))
            .WithAccessorList(
                property.AccessorList?.WithAccessors(property.AccessorList.Accessors
                    .Select(x => x
                        .WithBody(x.Body?
                            .WithStatements(x.Body.Statements
                                .Select(ss => ss.FullQualifiedStatement(semanticModel))
                                .ToSyntaxList()))
                        .WithExpressionBody(x.ExpressionBody?
                            .WithExpression(
                                x.ExpressionBody.Expression.FullQualifiedExpression(semanticModel))))
                    .ToSyntaxList()));

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
                    method.Body.Statements
                        .Select(x => x.FullQualifiedStatement(semanticModel))
                        .ToSyntaxList()));

    public static ConversionOperatorDeclarationSyntax FullQualifiedOperator(
        this ConversionOperatorDeclarationSyntax @operator,
        SemanticModel semanticModel) =>
        (@operator.FullQualifiedBaseMethod(semanticModel) as ConversionOperatorDeclarationSyntax)!
        .WithType(@operator.Type.FullName(semanticModel).ParseTypeName());

    public static OperatorDeclarationSyntax FullQualifiedOperator(this OperatorDeclarationSyntax @operator,
        SemanticModel semanticModel) =>
        (@operator.FullQualifiedBaseMethod(semanticModel) as OperatorDeclarationSyntax)!
        .WithReturnType(@operator.ReturnType.FullName(semanticModel).ParseTypeName());

    public static MethodDeclarationSyntax FullQualifiedMethod(this MethodDeclarationSyntax method,
        SemanticModel semanticModel) =>
        (method.FullQualifiedBaseMethod(semanticModel) as MethodDeclarationSyntax)!
        .WithConstraintClauses(method.ConstraintClauses.FullQualifiedConstraintClauses(semanticModel))
        .WithReturnType(method.ReturnType.FullName(semanticModel).ParseTypeName());

    public static ConstructorDeclarationSyntax FullQualifiedConstructor(this ConstructorDeclarationSyntax constructor,
        SemanticModel semanticModel) =>
        (constructor.FullQualifiedBaseMethod(semanticModel) as ConstructorDeclarationSyntax)!;

    public static T FullQualifiedStatement<T>(this T syntax, SemanticModel semanticModel)
        where T : StatementSyntax =>
        (T)(syntax switch
        {
            BlockSyntax block =>
                (object)Block(block.Statements
                    .Select(x => x.FullQualifiedStatement(semanticModel))
                    .ToSyntaxList()),
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
                        .WithVariables(declaration.Declaration.Variables
                            .Select(x => x.FullQualifiedVariable(semanticModel))
                            .ToSeparatedSyntaxList())),
            ReturnStatementSyntax returnStatement =>
                returnStatement
                    .WithExpression(returnStatement.Expression?.FullQualifiedExpression(semanticModel)),
            SwitchStatementSyntax switchStatement =>
                switchStatement
                    .WithExpression(switchStatement.Expression.FullQualifiedExpression(semanticModel))
                    .WithSections(
                        switchStatement.Sections
                            .Select(s => s
                                .WithStatements(s.Statements
                                    .Select(ss => ss.FullQualifiedStatement(semanticModel))
                                    .ToSyntaxList())
                                .WithLabels(s.Labels
                                    .Select(ll => ll switch
                                    {
                                        CaseSwitchLabelSyntax caseSwitchLabel =>
                                            caseSwitchLabel.WithValue(
                                                caseSwitchLabel.Value.FullQualifiedExpression(semanticModel)),
                                        _ => ll
                                    }).ToSyntaxList()))
                            .ToSyntaxList()),
            ThrowStatementSyntax throwStatement =>
                throwStatement
                    .WithExpression(throwStatement.Expression?.FullQualifiedExpression(semanticModel)),
            _ => syntax
        });

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

    public static T FullQualifiedExpression<T>(this T syntax, SemanticModel semanticModel)
        where T : ExpressionSyntax =>
        (T)(object)(syntax switch
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
            ConditionalAccessExpressionSyntax conditionalAccessExpression =>
                conditionalAccessExpression
                    .WithExpression(conditionalAccessExpression.Expression.FullQualifiedExpression(semanticModel))
                    .WithWhenNotNull(conditionalAccessExpression.WhenNotNull.FullQualifiedExpression(semanticModel)),
            ConditionalExpressionSyntax conditionalExpression =>
                conditionalExpression
                    .WithCondition(conditionalExpression.Condition.FullQualifiedExpression(semanticModel))
                    .WithWhenTrue(conditionalExpression.WhenTrue.FullQualifiedExpression(semanticModel))
                    .WithWhenFalse(conditionalExpression.WhenFalse.FullQualifiedExpression(semanticModel)),

            ImplicitObjectCreationExpressionSyntax implicitObjectCreation =>
                implicitObjectCreation
                    .WithArgumentList(implicitObjectCreation.ArgumentList.FullQualifiedArgumentList(semanticModel))
                    .WithInitializer(implicitObjectCreation.Initializer?.FullQualifiedExpression(semanticModel)),
            InitializerExpressionSyntax initializer =>
                initializer.WithExpressions(
                    initializer.Expressions
                        .Select(x => x.FullQualifiedExpression(semanticModel))
                        .ToSeparatedSyntaxList()),
            InvocationExpressionSyntax invocation =>
                invocation
                    .WithExpression(invocation.Expression.FullQualifiedExpression(semanticModel))
                    .WithArgumentList(invocation.ArgumentList.FullQualifiedArgumentList(semanticModel)),
            IsPatternExpressionSyntax isPatternExpression =>
                isPatternExpression
                    .WithExpression(isPatternExpression.Expression.FullQualifiedExpression(semanticModel))
                    .WithPattern(isPatternExpression.Pattern.FullQualifiedPattern(semanticModel)),
            MemberAccessExpressionSyntax memberAccess =>
                memberAccess
                    .WithName(memberAccess.Name is not GenericNameSyntax genericName
                        ? memberAccess.Name
                        : genericName.WithTypeArgumentList(
                            genericName.TypeArgumentList.WithArguments(
                                genericName.TypeArgumentList.Arguments
                                    .Select(x => x.FullName(semanticModel).ParseTypeName())
                                    .ToSeparatedSyntaxList())))
                    .WithExpression(memberAccess.Expression.FullQualifiedExpression(semanticModel)),
            NameSyntax name => name.FullName(semanticModel).ParseTypeName(),
            ObjectCreationExpressionSyntax objectCreation =>
                objectCreation
                    .WithType(objectCreation.Type.FullName(semanticModel).ParseTypeName())
                    .WithArgumentList(objectCreation.ArgumentList?.FullQualifiedArgumentList(semanticModel))
                    .WithInitializer(objectCreation.Initializer?.WithExpressions(
                        objectCreation.Initializer.Expressions
                            .Select(x => x.FullQualifiedExpression(semanticModel))
                            .ToSeparatedSyntaxList())),
            ParenthesizedExpressionSyntax parenthesizedExpression =>
                parenthesizedExpression
                    .WithExpression(parenthesizedExpression.Expression.FullQualifiedExpression(semanticModel)),
            ParenthesizedLambdaExpressionSyntax lambdaExpression =>
                lambdaExpression
                    .WithExpressionBody(lambdaExpression.ExpressionBody?.FullQualifiedExpression(semanticModel)),
            PostfixUnaryExpressionSyntax unaryExpression =>
                unaryExpression
                    .WithOperand(unaryExpression.Operand.FullQualifiedExpression(semanticModel)),
            PrefixUnaryExpressionSyntax prefixUnaryExpression =>
                prefixUnaryExpression
                    .WithOperand(prefixUnaryExpression.Operand.FullQualifiedExpression(semanticModel)),
            SimpleLambdaExpressionSyntax simpleLambda =>
                simpleLambda
                    .WithBlock(simpleLambda.Block?.FullQualifiedStatement(semanticModel))
                    .WithExpressionBody(simpleLambda.ExpressionBody?.FullQualifiedExpression(semanticModel)),
            SwitchExpressionSyntax switchExpression =>
                switchExpression
                    .WithGoverningExpression(
                        switchExpression.GoverningExpression.FullQualifiedExpression(semanticModel))
                    .WithArms(switchExpression.Arms
                        .Select(x =>
                            x.WithWhenClause(
                                    x.WhenClause?.WithCondition(
                                        x.WhenClause.Condition.FullQualifiedExpression(semanticModel)))
                                .WithPattern(x.Pattern.FullQualifiedPattern(semanticModel))
                                .WithExpression(x.Expression.FullQualifiedExpression(semanticModel)))
                        .ToSeparatedSyntaxList()),
            ThrowExpressionSyntax throwExpression =>
                throwExpression
                    .WithExpression(throwExpression.Expression.FullQualifiedExpression(semanticModel)),
            TypeOfExpressionSyntax typeOfExpression =>
                typeOfExpression
                    .WithType(typeOfExpression.Type.FullName(semanticModel).ParseTypeName()),
            _ => syntax
        });

    public static PatternSyntax FullQualifiedPattern(this PatternSyntax syntax, SemanticModel semanticModel) =>
        syntax switch
        {
            BinaryPatternSyntax binaryPattern =>
                binaryPattern
                    .WithLeft(binaryPattern.Left.FullQualifiedPattern(semanticModel))
                    .WithRight(binaryPattern.Right.FullQualifiedPattern(semanticModel)),
            ConstantPatternSyntax constantPattern =>
                constantPattern
                    .WithExpression(constantPattern.Expression.FullQualifiedExpression(semanticModel)),
            DeclarationPatternSyntax declarationPattern =>
                declarationPattern
                    .WithType(declarationPattern.Type.FullName(semanticModel).ParseTypeName()),
            RecursivePatternSyntax recursivePattern =>
                recursivePattern
                    .WithType(recursivePattern.Type.FullName(semanticModel).ParseTypeName())
                    .WithPropertyPatternClause(recursivePattern.PropertyPatternClause?
                        .WithSubpatterns(recursivePattern.PropertyPatternClause.Subpatterns
                            .Select(x => x.WithPattern(x.Pattern.FullQualifiedPattern(semanticModel)))
                            .ToSeparatedSyntaxList())),
            RelationalPatternSyntax relationalPattern =>
                relationalPattern
                    .WithExpression(relationalPattern.Expression.FullQualifiedExpression(semanticModel)),
            TypePatternSyntax typePatternSyntax =>
                typePatternSyntax
                    .WithType(typePatternSyntax.Type.FullName(semanticModel).ParseTypeName()),
            _ => syntax
        };
    

    public static ParameterListSyntax FullQualifiedParameterList(this ParameterListSyntax syntax,
        SemanticModel semanticModel) =>
        ParameterList(syntax.Parameters
            .Select(x => x.WithType(x.Type.FullName(semanticModel).ParseTypeName()))
            .ToSeparatedSyntaxList());


    public static ArgumentListSyntax FullQualifiedArgumentList(this ArgumentListSyntax syntax,
        SemanticModel semanticModel) =>
        syntax
            .WithArguments(syntax.Arguments
                .Select(x => x.WithExpression(x.Expression.FullQualifiedExpression(semanticModel)))
                .ToSeparatedSyntaxList());

    public static VariableDeclarationSyntax FullQualifiedVariableDeclaration(this VariableDeclarationSyntax syntax,
        SemanticModel semanticModel) =>
        syntax
            .WithType(syntax.Type.FullName(semanticModel).ParseTypeName())
            .WithVariables(syntax.Variables
                .Select(x => x.FullQualifiedVariable(semanticModel))
                .ToSeparatedSyntaxList());


    public static SeparatedSyntaxList<T> ToSeparatedSyntaxList<T>(this IEnumerable<T> enumerable)
        where T : SyntaxNode =>
        enumerable.Aggregate(new SeparatedSyntaxList<T>(), (s, t) => s.Add(t));

    public static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> enumerable)
        where T : SyntaxNode =>
        enumerable.Aggregate(new SyntaxList<T>(), (s, t) => s.Add(t));
}