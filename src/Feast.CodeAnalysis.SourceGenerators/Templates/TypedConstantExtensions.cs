using System;
using System.Linq;

#nullable enable
namespace Microsoft.CodeAnalysis;

[Literal("Feast.CodeAnalysis.TypedConstantExtensions")]
internal static class TypedConstantExtensions
{
    private static Type GetElementType(this Type type)
    {
        return type.IsArray ? type.GetElementType()! : throw new ArgumentException("type is not an array");
    }


    internal static object GenericList(this Type type, params object[] values)
    {
        var listType = type.MakeGenericType(type.GetGenericArguments());
        var list     = Activator.CreateInstance(listType);
        var add      = listType.GetMethod(nameof(System.Collections.Generic.List<object>.Add))!;
        foreach (var value in values)
        {
            add.Invoke(list, new object[] { value });
        }

        return list;
    }

    private static object ToArray(object genericList)
    {
        var toArray = genericList.GetType().GetMethod(nameof(System.Collections.Generic.List<object>.ToArray))!;
        return toArray.Invoke(genericList, null);
    }


    public static object? GetArgumentValue(this TypedConstant constant) =>
        constant.Kind switch
        {
            TypedConstantKind.Array => constant.Values.Select(x => x.GetArgumentValue()).ToArray(),
            TypedConstantKind.Error => null,
            TypedConstantKind.Type  => (constant.Value as INamedTypeSymbol)?.ToType(),
            _                       => constant.Value
        };

    public static object? GetArgumentValue(this TypedConstant constant,
        Type type)
    {
        var value = constant.GetArgumentValue();
        
        if (!type.IsArray)
            return type.IsEnum
                ? value == null
                    ? null
                    : Enum.ToObject(type, (int)value)
                : value;

        if (value is not object[] arr) throw new ArgumentException("constant is not an array");
        var ret = Array.CreateInstance(type.GetElementType()!, arr.Length);
        Array.Copy(arr, ret, arr.Length);
        return ret;
    }


    public static INamedTypeSymbol? GetArgumentType(this TypedConstant constant) =>
        constant.Kind == TypedConstantKind.Type
            ? constant.Value as INamedTypeSymbol
            : throw new ArgumentException("constant is not a type");


    public static string? GetArgumentString(this TypedConstant constant) => constant.Kind == TypedConstantKind.Primitive
        ? constant.Value as string
        : throw new ArgumentException("constant is not a string");

    public static T? GetArgumentEnum<T>(this TypedConstant constant) where T : Enum =>
        constant.Kind == TypedConstantKind.Enum
            ? (T?)constant.Value
            : throw new ArgumentException("constant is not an enum");

    public static T? GetArgumentPrimitive<T>(this TypedConstant constant) where T : struct =>
        constant.Kind == TypedConstantKind.Primitive
            ? (T?)constant.Value
            : throw new ArgumentException("constant is not a primitive");


    public static T?[] GetArgumentArray<T>(this TypedConstant constant) => constant.Kind == TypedConstantKind.Array
        ? constant.Values.Select(x => (T?)x.Value).ToArray()
        : throw new ArgumentException("constant is not an array");
}