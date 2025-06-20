using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#nullable disable
namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.CSharpScriptCompiler")]
internal sealed partial class CSharpScriptCompiler : ScriptCompiler
{
    public static readonly ScriptCompiler Instance = new CSharpScriptCompiler();

    internal static readonly CSharpParseOptions DefaultParseOptions =
        new(LanguageVersion.Latest, kind: SourceCodeKind.Script);

    private CSharpScriptCompiler()
    {
    }

    public override DiagnosticFormatter DiagnosticFormatter => CSharpDiagnosticFormatter.Instance;

    public override StringComparer IdentifierComparer => StringComparer.Ordinal;

    public override bool IsCompleteSubmission(SyntaxTree tree)
    {
        return SyntaxFactory.IsCompleteSubmission(tree);
    }

    public override SyntaxTree ParseSubmission(
        SourceText text,
        ParseOptions parseOptions,
        CancellationToken cancellationToken)
    {
        var text1   = text;
        var options = parseOptions;
        if ((object)options == null)
            options = DefaultParseOptions;
        var cancellationToken1 = cancellationToken;
        return SyntaxFactory.ParseSyntaxTree(text1, options, cancellationToken: cancellationToken1);
    }

    public override Compilation CreateSubmission(Script script)
    {
        var csharpCompilation = (CSharpCompilation)null;
        if (script.Previous != null)
            csharpCompilation = (CSharpCompilation)script.Previous.GetCompilation();
        var instance = DiagnosticBag.GetInstance();
        ImmutableArray<MetadataReference> referencesForCompilation =
            script.GetReferencesForCompilation(MessageProviderInstance, instance);
        instance.Free();
        var sourceText = script.SourceText;
        var options1   = ParseOptions.Value(script.Options) as ParseOptions;
        if ((object)options1 == null)
            options1 = DefaultParseOptions;
        var    filePath          = script.Options.FilePath;
        var    cancellationToken = CancellationToken.None;
        var    syntaxTree1       = SyntaxFactory.ParseSyntaxTree(sourceText, options1, filePath, cancellationToken);
        script.Builder.GenerateSubmissionId(out var assemblyName1, out var typeName);
        var assemblyName2 = assemblyName1;
        var syntaxTree2   = syntaxTree1;
        // ISSUE: variable of a boxed type
        var references      = (ValueType)referencesForCompilation;
        var                                        scriptClassName = typeName;
        // ISSUE: variable of a boxed type
        var imports = (ValueType)script.Options.Imports;
        var optimizationLevel = (int)script.Options.OptimizationLevel;
        var num1 = script.Options.CheckOverflow ? 1 : 0;
        var num2 = script.Options.AllowUnsafe ? 1 : 0;
        var warningLevel1 = script.Options.WarningLevel;
        var sourceResolver = script.Options.SourceResolver;
        var metadataResolver = script.Options.MetadataResolver;
        var identityComparer = (AssemblyIdentityComparer)DesktopAssemblyIdentityComparer.Default;
        var cryptoPublicKey = new ImmutableArray<byte>();
        bool? delaySign = null;
        var warningLevel2 = warningLevel1;
        var sourceReferenceResolver = sourceResolver;
        var metadataReferenceResolver = metadataResolver;
        var assemblyIdentityComparer = identityComparer;
        var options2 = WithTopLevelBinderFlags(new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary, scriptClassName: scriptClassName, usings: (IEnumerable<string>)imports,
            optimizationLevel: (OptimizationLevel)optimizationLevel, checkOverflow: num1 != 0, allowUnsafe: num2 != 0,
            cryptoPublicKey: cryptoPublicKey, delaySign: delaySign, warningLevel: warningLevel2,
            sourceReferenceResolver: sourceReferenceResolver, metadataReferenceResolver: metadataReferenceResolver,
            assemblyIdentityComparer: assemblyIdentityComparer));
        var previousScriptCompilation = csharpCompilation;
        var returnType                = script.ReturnType;
        var globalsType               = script.GlobalsType;
        return CSharpCompilation.CreateScriptCompilation(assemblyName2, syntaxTree2,
            (IEnumerable<MetadataReference>)references, options2, previousScriptCompilation, returnType, globalsType);
    }

    internal static CSharpCompilationOptions WithTopLevelBinderFlags(CSharpCompilationOptions options)
    {
        return Option_WithTopLevelBinderFlags.Value(options, IgnoreCorLibraryDuplicatedTypes) as CSharpCompilationOptions;
    }
    
    
}