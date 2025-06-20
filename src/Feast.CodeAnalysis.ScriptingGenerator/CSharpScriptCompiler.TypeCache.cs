using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

namespace Feast.CodeAnalysis.Scripting;
[Literal("Feast.CodeAnalysis.Scripting.Generates.TypeCache.CSharpScriptCompiler")]

partial class CSharpScriptCompiler
{
    internal static readonly Lazy<Func<object, object, object>> Option_WithTopLevelBinderFlags = new(() =>
    {
        var method = typeof(CSharpCompilationOptions)
            .GetMethod(nameof(WithTopLevelBinderFlags), BindingFlags.NonPublic | BindingFlags.Instance);
        if (method == null)
        {
            throw new InvalidOperationException(
                $"Method {nameof(WithTopLevelBinderFlags)} not found in {nameof(CSharpCompilationOptions)} class.");
        }

        return (arg0, arg1) => method.Invoke(arg0, [arg1]);
    });

    [field: AllowNull, MaybeNull]
    internal static object IgnoreCorLibraryDuplicatedTypes
    {
        get
        {
            if (field is not null) return field;
            var f = Global.GetAssembly("Microsoft.CodeAnalysis.CSharp")?
                .GetType("Microsoft.CodeAnalysis.CSharp.BinderFlags")
                .GetField("IgnoreCorLibraryDuplicatedTypes");
            if (f == null)
            {
                throw new InvalidOperationException(
                    $"Method {nameof(IgnoreCorLibraryDuplicatedTypes)} not found in BinderFlags class.");
            }

            return field = f.GetValue(null);
        }
    }

    [field: AllowNull, MaybeNull]
    internal static object MessageProviderInstance
    {
        get
        {
            if (field is not null) return field;
            var f = Global.GetAssembly("Microsoft.CodeAnalysis.CSharp")?
                .GetType("Microsoft.CodeAnalysis.CSharp.MessageProvider")
                .GetField(nameof(Instance));
            if (f == null)
            {
                throw new InvalidOperationException(
                    $"Method {nameof(Instance)} not found in MessageProvider class.");
            }

            return field = f;
        }
    }
    
    internal static readonly Lazy<Func<object,object>> ParseOptions = new(() =>
    {
        var method = typeof(ScriptOptions)
            .GetProperty(nameof(ParseOptions), BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (method == null)
        {
            throw new InvalidOperationException(
                $"Property {nameof(ParseOptions)} not found in {nameof(ScriptOptions)} class.");
        }

        return arg0 => method.GetValue(arg0);
    });
}