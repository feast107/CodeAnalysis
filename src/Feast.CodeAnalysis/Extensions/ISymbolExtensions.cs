using Microsoft.CodeAnalysis;
using TypeInfo = Feast.CodeAnalysis.Utils.TypeInfo;

namespace Feast.CodeAnalysis.Extensions;

public static class ISymbolExtensions
{
    public static bool IsAssignableTo(this INamedTypeSymbol symbol, Type type) => TypeInfo.FromSymbol(symbol).IsAssignableTo(TypeInfo.FromType(type));
    
    public static bool IsAssignableTo(this Type type, INamedTypeSymbol symbol) => TypeInfo.FromType(type).IsAssignableTo(TypeInfo.FromSymbol(symbol));
    
    public static bool IsAssignableFrom(this Type type, INamedTypeSymbol symbol) => TypeInfo.FromSymbol(symbol).IsAssignableTo(TypeInfo.FromType(type));
    
    public static bool IsAssignableFrom(this INamedTypeSymbol symbol, Type type) =>TypeInfo.FromType(type).IsAssignableTo( TypeInfo.FromSymbol(symbol));
}