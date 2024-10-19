using System;
using System.Linq;
using Microsoft.CodeAnalysis;

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

    public override System.Collections.Generic.IEnumerable<global::System.Type> ExportedTypes => Symbol
        .GetForwardedTypes()
        .Where(static x => x.DeclaredAccessibility == Accessibility.Public)
        .Select(static x => new Type(x));

    public bool Equals(global::System.Reflection.Assembly? other)
    {
        if (other is Assembly compileTime)
            return SymbolEqualityComparer.Default.Equals(Symbol, compileTime.Symbol);
        return other is not null && FullName == other.FullName;
    }

    public override int GetHashCode() => Symbol.GetHashCode();

    public override bool Equals(object? o) => o is Assembly assembly && Equals(assembly);
}