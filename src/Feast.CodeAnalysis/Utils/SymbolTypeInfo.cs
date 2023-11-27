using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Utils;

internal class SymbolTypeInfo : TypeInfo
{
    private readonly ITypeSymbol type;

    public SymbolTypeInfo(ITypeSymbol type)
    {
        this.type = type;
        Name      = type.MetadataName;
        Namespace = type.ContainingNamespace.MetadataName == string.Empty
            ? null
            : type.ContainingNamespace.ToDisplayString();
        if (type.BaseType != null) baseClass = new(() => new SymbolTypeInfo(type.BaseType));
        switch (type)
        {
            case INamedTypeSymbol namedTypeSymbol:
                IsInterface = type.BaseType == null;
                interfaces = new(() =>
                    type.AllInterfaces
                        .Select(FromSymbol)
                        .ToArray());
                if (namedTypeSymbol.IsGenericType)
                    genericTypes = new(() =>
                        namedTypeSymbol.TypeArguments
                            .Select(FromSymbol)
                            .ToArray());
                break;
            case ITypeParameterSymbol parameterSymbol:
                constrainedTypes = new(() =>
                    parameterSymbol.ConstraintTypes
                        .Select(FromSymbol)
                        .ToArray());
                IsParameter = true;
                break;
        }


    }

    public override string? Namespace   { get; }
    public override string  Name        { get; }
    public override bool    IsParameter { get; }
    public override bool    IsInterface { get; }

    protected override Lazy<TypeInfo?>               baseClass        { get; } = new(() => null);
    protected override Lazy<IReadOnlyList<TypeInfo>> genericTypes     { get; } = new(Array.Empty<TypeInfo>);
    protected override Lazy<IReadOnlyList<TypeInfo>> interfaces       { get; } = new(Array.Empty<TypeInfo>);
    protected override Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; } = new(Array.Empty<TypeInfo>);

    public static bool operator ==(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);
    public static bool operator !=(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);

    protected override bool SameAs(TypeInfo another)
    {
        return another is SymbolTypeInfo symbolTypeInfo
            ? SymbolEqualityComparer.Default.Equals(type, symbolTypeInfo.type)
            : base.SameAs(another);
    }
    public override bool IsSubClassOf(TypeInfo another)
    {
        if(another is not SymbolTypeInfo symbolTypeInfo) return base.IsSubClassOf(another);
        var baseType = type.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, symbolTypeInfo.type)) return true;
            baseType = baseType.BaseType;
        }

        return false;
    }

#pragma warning disable RS1024
    public override int GetHashCode() => type.GetHashCode();
#pragma warning restore RS1024

    public override bool Equals(object? obj) =>
        obj is SymbolTypeInfo symbolInfo
            ? SymbolEqualityComparer.Default.Equals(type, symbolInfo.type)
            : obj is TypeInfo typeInfo
              && SameAs(typeInfo);
}