using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Utils;

internal class SymbolTypeInfo : TypeInfo
{
    public SymbolTypeInfo(ITypeSymbol type)
    {
        Name = type.MetadataName;
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
}