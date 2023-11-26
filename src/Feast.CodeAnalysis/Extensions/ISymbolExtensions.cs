using Microsoft.CodeAnalysis;
using TypeInfo = Feast.CodeAnalysis.Utils.TypeInfo;

namespace Feast.CodeAnalysis.Extensions;

public static class ISymbolExtensions
{
    public static bool IsAssignableTo(this ITypeSymbol symbol, Type type) =>
        symbol.ToTypeInfo().IsAssignableTo(type);

    public static bool IsAssignableTo(this Type type, ITypeSymbol symbol) =>
        symbol.ToTypeInfo().IsAssignableFrom(type);

    public static bool IsAssignableFrom(this Type type, ITypeSymbol symbol) =>
        symbol.ToTypeInfo().IsAssignableTo(type);

    public static bool IsAssignableFrom(this ITypeSymbol symbol, Type type) =>
        symbol.ToTypeInfo().IsAssignableTo(type);
    
    public static TypeInfo ToTypeInfo(this ITypeSymbol symbol) => TypeInfo.FromSymbol(symbol);
}