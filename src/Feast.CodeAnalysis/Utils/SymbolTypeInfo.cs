using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Utils;

public class SymbolTypeInfo : TypeInfo
{
    private SymbolTypeInfo(ITypeSymbol symbol, string name)
    {
        Name        = name;
        IsInterface = symbol.BaseType == null;
        if (symbol.BaseType != null) BaseClass = new(() => new SymbolTypeInfo(symbol.BaseType));
    }


    public SymbolTypeInfo(INamedTypeSymbol type)
        : this(type, $"{type.ContainingNamespace.ToDisplayString()}.{type.MetadataName}")
    {
        Interfaces = new(() => type.AllInterfaces
            .Select(x => new SymbolTypeInfo(x) as TypeInfo)
            .ToArray());
        if (type.IsGenericType)
            GenericTypes = new(() => type.TypeArguments.Select(x =>
                x switch
                {
                    ITypeParameterSymbol parameterSymbol => new SymbolTypeInfo(parameterSymbol),
                    INamedTypeSymbol namedTypeSymbol     => new SymbolTypeInfo(namedTypeSymbol),
                    _                                    => throw new Exception()
                } as TypeInfo
            ).ToArray());
    }


    public SymbolTypeInfo(ITypeParameterSymbol type) : this(type, type.Name)
    {
        Name        = type.Name;
        IsParameter = true;
        Debugger.Break();
    }

    public override string Name { get; }

    public override bool IsParameter { get; }

    public override bool             IsInterface  { get; }
    public override Lazy<TypeInfo[]> GenericTypes { get; } = new();
    public override Lazy<TypeInfo?>  BaseClass    { get; } = new();
    public override Lazy<TypeInfo[]> Interfaces   { get; } = new();
}