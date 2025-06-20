using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.Global")]
internal static class Global
{
    private static readonly ConcurrentDictionary<string, Assembly?> Assemblies = new();
    private static readonly Lazy<Assembly[]> GetAssemblies = new(() => AppDomain.CurrentDomain.GetAssemblies());

    public static Assembly? GetAssembly(string name) => 
        Assemblies.GetOrAdd(name,
        _ => GetAssemblies
            .Value
            .FirstOrDefault(x => x.GetName().Name == name));
    
}