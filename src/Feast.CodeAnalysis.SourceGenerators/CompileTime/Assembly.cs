using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using TypeInfo = System.Reflection.TypeInfo;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.Assembly")]
internal partial class Assembly(global::Microsoft.CodeAnalysis.IAssemblySymbol symbol)
    : global::System.Reflection.Assembly, IEquatable<global::System.Reflection.Assembly>
{
    internal IAssemblySymbol Symbol => symbol;

    public override string          FullName => Symbol.GetFullyQualifiedName();
    public override string          Location => Symbol.Locations.FirstOrDefault()?.GetLineSpan().Path ?? string.Empty;
    public override bool            ReflectionOnly => !Symbol.CanBeReferencedByName;

    public override System.Collections.Generic.IEnumerable<global::System.Reflection.Module> Modules => Symbol
        .Modules
        .Select(static x => new Module(x));

    public override System.Collections.Generic.IEnumerable<global::System.Type> ExportedTypes =>
        GetTypesInternal()
            .Where(static x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(static x => new Type(x) as System.Type);

    public bool Equals(global::System.Reflection.Assembly? other)
    {
        if (other is Assembly compileTime)
            return SymbolEqualityComparer.Default.Equals(Symbol, compileTime.Symbol);
        return other is not null && FullName == other.FullName;
    }

    public override IEnumerable<TypeInfo> DefinedTypes => GetTypesInternal().Select(x => new Type(x).GetTypeInfo());

    private List<INamedTypeSymbol> GetTypesInternal()
    {
        if (types is not null) return types;
        var collector = new TypeCollector();
        Symbol.GlobalNamespace.Accept(collector);
        return types = collector.Types;
    }

    private List<INamedTypeSymbol>? types;

    public override System.Type[] GetTypes() => GetTypesInternal()
        .Select(x => new Type(x) as System.Type)
        .ToArray();

    public override object? CreateInstance(string typeName, 
                                           bool ignoreCase, 
                                           BindingFlags bindingAttr, 
                                           Binder binder, 
                                           object[] args,
                                           CultureInfo culture, 
                                           object[] activationAttributes) =>
        throw new NotSupportedException();

    public override System.Type? GetType(string name) => 
        GetTypes().FirstOrDefault(x => x.FullName == name);

    public override System.Type? GetType(string name, bool throwOnError)
    {
        var type = GetTypes().FirstOrDefault(x => x.FullName == name);
        if (type is null && throwOnError)
            throw new TypeLoadException($"Type '{name}' not found in assembly '{FullName}'.");
        return type;
    }

    public override System.Type? GetType(string name, bool throwOnError, bool ignoreCase)
    {
        var type = GetTypes().FirstOrDefault(x => 
            string.Equals(x.FullName, name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        if (type is null && throwOnError)
            throw new TypeLoadException($"Type '{name}' not found in assembly '{FullName}'.");
        return type;
    }

    public override bool IsDynamic => false;

    public override System.Reflection.Module[] GetModules(bool getResourceModules) =>
        Symbol
            .Modules
            .Select(static x => new Module(x) as System.Reflection.Module)
            .ToArray();

    public override System.Reflection.Module? GetModule(string name) => 
        Symbol.Modules.FirstOrDefault(x => x.Name == name)?.ToModule();
    
    public override System.Type[] GetExportedTypes() => ExportedTypes.ToArray();

    public override int GetHashCode() => Symbol.GetHashCode();

    public override bool Equals(object? o) => o is Assembly assembly && Equals(assembly);

    private class TypeCollector : SymbolVisitor
    {
        public List<INamedTypeSymbol> Types { get; } = [];

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            Types.Add(symbol);
            foreach (var member in symbol.GetMembers())
            {
                member.Accept(this);
            }
        }
    }
}