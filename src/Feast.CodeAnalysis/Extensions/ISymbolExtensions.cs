using Microsoft.CodeAnalysis;
using TypeInfo = Feast.CodeAnalysis.Utils.TypeInfo;

namespace Feast.CodeAnalysis.Extensions;

public static class ISymbolExtensions
{
    public static bool IsAssignableTo(this ITypeSymbol symbol, Type type) =>
        TypeInfo.FromSymbol(symbol).IsAssignableTo(TypeInfo.FromType(type));

    public static bool IsAssignableTo(this Type type, ITypeSymbol symbol) =>
        TypeInfo.FromType(type).IsAssignableTo(TypeInfo.FromSymbol(symbol));

    public static bool IsAssignableFrom(this Type type, ITypeSymbol symbol) =>
        TypeInfo.FromType(type).IsAssignableFrom(TypeInfo.FromSymbol(symbol));

    public static bool IsAssignableFrom(this ITypeSymbol symbol, Type type) =>
        TypeInfo.FromSymbol(symbol).IsAssignableTo(TypeInfo.FromType(type));
}