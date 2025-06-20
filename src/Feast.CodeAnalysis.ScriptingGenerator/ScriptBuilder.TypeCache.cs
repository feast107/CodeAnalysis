using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting.Hosting;

namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.TypeCache.ScriptBuilder")]
partial class ScriptBuilder
{
    private static readonly Lazy<Func<object, object>> GetBoundReferenceManager = new(() =>
    {
        var method = typeof(Compilation).GetMethod(nameof(GetBoundReferenceManager),
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
        {
            throw new InvalidOperationException($"Method {nameof(GetBoundReferenceManager)} not found in Compilation class.");
        }
        return arg => method.Invoke(arg, []);
    });
    
    private static readonly Lazy<Func<object, object>> GetReferencedAssemblies = new(() =>
    {
        var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
            .GetType("Microsoft.CodeAnalysis.CommonReferenceManager")
            .GetMethod(nameof(GetReferencedAssemblies),
                BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
        {
            throw new InvalidOperationException($"Method {nameof(GetReferencedAssemblies)} not found in CommonReferenceManager class.");
        }
        return arg => method.Invoke(arg, []);
    });

    private static PropertyInfo? getKeyValuePairKey;
    private static PropertyInfo? getKeyValuePairValue;

    [MemberNotNullWhen(true, nameof(getKeyValuePairKey), nameof(getKeyValuePairValue))]
    private static bool MapProperties(object keyValuePair)
    {
        getKeyValuePairKey   ??= keyValuePair.GetType().GetProperty(nameof(Key));
        getKeyValuePairValue ??= keyValuePair.GetType().GetProperty(nameof(Value));
        if (getKeyValuePairKey == null || getKeyValuePairValue == null)
        {
            throw new InvalidOperationException("Key or Value property not found in KeyValuePair class.");
        }

        return true;
    }
    private static object Key(object keyValuePair)
    {
        if(MapProperties(keyValuePair))
            return getKeyValuePairKey.GetValue(keyValuePair);
        throw new InvalidOperationException("Key property not found in KeyValuePair class.");
    }
    
    private static object Value(object keyValuePair)
    {
        if(MapProperties(keyValuePair))
            return getKeyValuePairValue.GetValue(keyValuePair);
        throw new InvalidOperationException("Value property not found in KeyValuePair class.");
    }

    private static readonly Lazy<Func<object, object>> Identity = new(() =>
    {
        var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
            .GetType("Microsoft.CodeAnalysis.Symbols.IAssemblySymbolInternal")
            .GetProperty(nameof(Identity));

        if (method == null)
        {
            throw new InvalidOperationException($"Method {nameof(Identity)} not found in IAssemblySymbolInternal class.");
        }
        return arg => method.GetValue(arg);
    });
    
    private static readonly Lazy<Func<object,Stream,Stream, object>> LoadAssemblyFromStream = new(() =>
    {
       
        var method =  typeof(InteractiveAssemblyLoader)
            .GetMethod(nameof(LoadAssemblyFromStream),
                BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
        {
            throw new InvalidOperationException($"Method {nameof(LoadAssemblyFromStream)} not found in InteractiveAssemblyLoader class.");
        }

        return (arg0, arg1, arg2) => method.Invoke(arg0, [arg1, arg2]);
    });
    
    private static readonly Lazy<object> EmitOptionsDefault = new(() =>
    {

        var field = typeof(EmitOptions)
            .GetField("Default", BindingFlags.NonPublic | BindingFlags.Static);

        if (field == null)
        {
            throw new InvalidOperationException("Field Default not found in EmitOptions class.");
        }

        return field.GetValue(null);
    });
    
    private static readonly Lazy<Func<object,object,object>> BuildQualifiedName = new(() =>
    {

        var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
            .GetType("Microsoft.CodeAnalysis.MetadataHelpers")
            .GetMethod(nameof(BuildQualifiedName),
                BindingFlags.NonPublic | BindingFlags.Static);
        if (method == null)
        {
            throw new InvalidOperationException($"Method {nameof(BuildQualifiedName)} not found in MetadataHelpers class.");
        }
        
        return (arg1, arg2) => method.Invoke(null, [arg1, arg2]);
    });
}