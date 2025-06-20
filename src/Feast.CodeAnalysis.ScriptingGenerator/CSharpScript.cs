using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.CSharpScript")]
public static class CSharpScript
{
    public static Script<T> Create<T>(
        string code,
        ScriptOptions options = null,
        Type globalsType = null,
        InteractiveAssemblyLoader assemblyLoader = null)
    {
        if (code == null)
            throw new ArgumentNullException(nameof(code));
        return Script.CreateInitialScript<T>(CSharpScriptCompiler.Instance,
            SourceText.From(code, options?.FileEncoding), options, globalsType, assemblyLoader);
    }

    public static Script<T> Create<T>(
        Stream code,
        ScriptOptions options = null,
        Type globalsType = null,
        InteractiveAssemblyLoader assemblyLoader = null)
    {
        if (code == null)
            throw new ArgumentNullException(nameof(code));
        return Script.CreateInitialScript<T>(CSharpScriptCompiler.Instance,
            SourceText.From(code, options?.FileEncoding), options, globalsType, assemblyLoader);
    }

    public static Script<object> Create(
        string code,
        ScriptOptions options = null,
        Type globalsType = null,
        InteractiveAssemblyLoader assemblyLoader = null)
    {
        if (code == null)
            throw new ArgumentNullException(nameof(code));
        return Create<object>(code, options, globalsType, assemblyLoader);
    }

    public static Script<object> Create(
        Stream code,
        ScriptOptions options = null,
        Type globalsType = null,
        InteractiveAssemblyLoader assemblyLoader = null)
    {
        if (code == null)
            throw new ArgumentNullException(nameof(code));
        return Create<object>(code, options, globalsType, assemblyLoader);
    }

    public static Task<ScriptState<T>> RunAsync<T>(
        string code,
        ScriptOptions options = null,
        object globals = null,
        Type globalsType = null,
        CancellationToken cancellationToken = default)
    {
        string        code1        = code;
        ScriptOptions options1     = options;
        Type          globalsType1 = globalsType;
        if ((object)globalsType1 == null)
            globalsType1 = globals?.GetType();
        return Create<T>(code1, options1, globalsType1).RunAsync(globals, cancellationToken);
    }

    public static Task<ScriptState<object>> RunAsync(
        string code,
        ScriptOptions options = null,
        object globals = null,
        Type globalsType = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync<object>(code, options, globals, globalsType, cancellationToken);
    }

    public static Task<T> EvaluateAsync<T>(
        string code,
        ScriptOptions options = null,
        object globals = null,
        Type globalsType = null,
        CancellationToken cancellationToken = default)
    {
        return RunAsync<T>(code, options, globals, globalsType, cancellationToken).GetEvaluationResultAsync<T>();
    }

    public static Task<object> EvaluateAsync(
        string code,
        ScriptOptions options = null,
        object globals = null,
        Type globalsType = null,
        CancellationToken cancellationToken = default)
    {
        return EvaluateAsync<object>(code, options, globals, globalsType, cancellationToken);
    }
}