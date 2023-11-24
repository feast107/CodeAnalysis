using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Feast.CodeAnalysis.TestGenerator;

/// <summary>
/// A sample source generator that creates a custom report based on class properties. The target class should be annotated with the 'Generators.ReportAttribute' attribute.
/// When using the source code as a baseline, an incremental source generator is preferable because it reduces the performance overhead.
/// </summary>
[Generator]
public class SampleIncrementalSourceGenerator : IIncrementalGenerator
{
    private const string Namespace     = "Generators";
    private const string AttributeName = "RelateToAttribute";

    private const string AttributeSourceCode = $$"""
                                                 // <auto-generated/>

                                                 namespace {{Namespace}}
                                                 {
                                                     [System.AttributeUsage(System.AttributeTargets.Class)]
                                                     public class {{AttributeName}} : System.Attribute
                                                     {
                                                         public {{AttributeName}}(Type type){ }
                                                     }
                                                 }
                                                 """;


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{AttributeName}.g.cs",
            SourceText.From(AttributeSourceCode, Encoding.UTF8)));
        // Filter classes annotated with the [Report] attribute. Only filtered Syntax Nodes can trigger code generation.
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, token) =>
                   s is  ClassDeclarationSyntax,
                (ctx,_) => GetClassDeclarationForSourceGen(ctx))
            .Where(t => t.reportAttributeFound)
            .Select((t, _) => t.Item1);

        // Generate the source code.
        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            ((ctx, t) => GenerateCode(ctx, t.Left, t.Right)));
    }

    /// <summary>
    /// Checks whether the Node is annotated with the [Report] attribute and maps syntax context to the specific node type (ClassDeclarationSyntax).
    /// </summary>
    /// <param name="context">Syntax context, based on CreateSyntaxProvider predicate</param>
    /// <returns>The specific cast and whether the attribute was found.</returns>
    private static (ClassDeclarationSyntax, bool reportAttributeFound) GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        
        var attr = classDeclarationSyntax.GetSpecifiedAttribute(context.SemanticModel, $"{Namespace}.{AttributeName}");

        var args = attr?.ArgumentList?.Arguments;

        if (args is null || !args.Value.Any()) return (classDeclarationSyntax, false);
        
        var arg = attr!.ArgumentList!.Arguments.First();

        var exp = arg.Expression as TypeOfExpressionSyntax;

        var type = exp!.Type;

        var symbol = context.SemanticModel.GetSymbolInfo(type).Symbol;
        
        
        return (classDeclarationSyntax, 
            classDeclarationSyntax.HasSpecifiedAttribute(context.SemanticModel, $"{Namespace}.{AttributeName}"));
    }

    /// <summary>
    /// Generate code action.
    /// It will be executed on specific nodes (ClassDeclarationSyntax annotated with the [Report] attribute) changed by the user.
    /// </summary>
    /// <param name="context">Source generation context used to add source files.</param>
    /// <param name="compilation">Compilation used to provide access to the Semantic Model.</param>
    /// <param name="classDeclarations">Nodes annotated with the [Report] attribute that trigger the generate action.</param>
    private void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        // Go through all filtered class declarations.
        foreach (var classDeclarationSyntax in classDeclarations)
        {
            // We need to get semantic model of the class to retrieve metadata.
            var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);

            // Symbols allow us to get the compile-time information.
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
                continue;

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // 'Identifier' means the token of the node. Get class name from the syntax node.
            var className = classDeclarationSyntax.Identifier.Text;

            // Go through all class members with a particular type (property) to generate method lines.
            var methodBody = classSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Select(p =>
                    $$"""        yield return $"{{p.Name}}:{this.{{p.Name}}}";"""); // e.g. yield return $"Id:{this.Id}";

            // Build up the source code
            var code = $$"""
                         // <auto-generated/>

                         using System;
                         using System.Collections.Generic;

                         namespace {{namespaceName}};

                         partial class {{className}}
                         {
                             public IEnumerable<string> Report()
                             {
                         {{string.Join("\n", methodBody)}}
                             }
                         }

                         """;

            // Add the source code to the compilation.
            context.AddSource($"{className}.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }
}