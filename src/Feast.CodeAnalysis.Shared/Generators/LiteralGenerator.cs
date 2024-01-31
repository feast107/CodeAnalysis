﻿global using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Linq;
using System.Text;
using Feast.CodeAnalysis.LiteralGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Feast.CodeAnalysis.Generators.LiteralGenerator;

[Generator(LanguageNames.CSharp)]
public class LiteralGenerator : IIncrementalGenerator
{
    private const string AttributeName = "System.LiteralAttribute";
    
    private const string LiteralAttribute =
        """
        #nullable enable
        using System;
        namespace System
        {
            [global::System.AttributeUsage(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct | global::System.AttributeTargets.Interface | global::System.AttributeTargets.Enum | global::System.AttributeTargets.Delegate)]
            public class LiteralAttribute : Attribute
            {
                public string? FieldName { get; set; }
            
                public LiteralAttribute(string belongToFullyQualifiedClassName){ }
            }
        }
        """;
    
    internal static SyntaxTriviaList Header =
        TriviaList(
            Comment($"// <auto-generated/> By {nameof(Feast)}.{nameof(CodeAnalysis)}"),
            Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
            Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true)));

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{nameof(LiteralAttribute)}.g.cs", SourceText.From(LiteralAttribute, Encoding.UTF8));
        });
        
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(AttributeName,
                (ctx, t) => ctx is MemberDeclarationSyntax,
                transform: (ctx, t) => ctx);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) =>
            {
                foreach (var syntax in t.Right)
                {
                    var config = syntax.Attributes.First(x =>
                        x.AttributeClass!.ToDisplayString() == AttributeName);
                    if (config.ConstructorArguments[0].Kind  == TypedConstantKind.Error ||
                        config.ConstructorArguments[0].Value == null) continue;
                    var fullClassName = (config.ConstructorArguments[0].Value as string)!.Split('.');
                    if (fullClassName.Length < 2) continue;
                    var fieldName = config.NamedArguments is [{ Key: "FieldName", Value.Value: string }]
                                    && !string.IsNullOrWhiteSpace(config.NamedArguments[0].Value.Value as string)
                        ? (config.NamedArguments[0].Value.Value as string)!
                        : "Text";

                    var typeDeclaration = (syntax.TargetNode as MemberDeclarationSyntax)!;
                    var attrList        = new SyntaxList<AttributeListSyntax>();
                    var classSymbol     = (syntax.TargetSymbol as INamedTypeSymbol)!;
                    var attrSymbols     = classSymbol.GetAttributes();
                    foreach (var (attributeList, index) in typeDeclaration.AttributeLists.Select((x, i) => (x, i)))
                    {
                        var attrs = new SeparatedSyntaxList<AttributeSyntax>();
                        attrs = attributeList.Attributes
                            .Where(_ => attrSymbols[index].AttributeClass!.ToDisplayString() != AttributeName)
                            .Aggregate(attrs, (current, attribute) => current.Add(attribute));

                        if (attrs.Count > 0)
                        {
                            attrList = attrList.Add(AttributeList(attrs));
                        }
                    }

                    var full = typeDeclaration
                        .FullQualifiedMember(syntax.SemanticModel)
                        .WithAttributeLists(attrList)
                        .FullNamespace(classSymbol)
                        .WithUsing(typeDeclaration.SyntaxTree.GetCompilationUnitRoot())
                        .NormalizeWhitespace()
                        .GetText(Encoding.UTF8);
                    var @namespace = string.Join(".", fullClassName.Take(fullClassName.Length - 1));
                    var className  = fullClassName.Last();
                    var content = $"internal static string {fieldName} = \"\"\"\n"
                                  + full.ToString().Replace("\"\"\"","\"^\"\"")
                                  + "\n\"\"\""
                                  + ".Replace(\"\\\"^\\\"\\\"\",\"\\\"\\\"\\\"\");";
                    var code = CompilationUnit()
                        .AddMembers(
                            NamespaceDeclaration(IdentifierName(@namespace))
                                .WithLeadingTrivia(Header)
                                .AddMembers(
                                    ClassDeclaration(className)
                                        .AddModifiers(Token(SyntaxKind.PartialKeyword))
                                        .AddMembers(ParseMemberDeclaration(content)!)
                                ));
                    ctx.AddSource($"{@namespace}.{className}.g.cs", code.NormalizeWhitespace().GetText(Encoding.UTF8));
                }
            });
    }
}