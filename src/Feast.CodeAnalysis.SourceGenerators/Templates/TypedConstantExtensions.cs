﻿using System;
using System.Linq;

#nullable enable
namespace Microsoft.CodeAnalysis;

[Literal("Feast.CodeAnalysis.TypedConstantExtensions")]
internal static class TypedConstantExtensions
{
    private static global::System.Type GetElementType(this global::System.Type type)
    {
        return type.IsArray ? type.GetElementType()! : throw new global::System.ArgumentException("type is not an array");
    }


    internal static object GenericList(this global::System.Type type, params object[] values)
    {
        var listType = type.MakeGenericType(type.GetGenericArguments());
        var list     = global::System.Activator.CreateInstance(listType);
        var add      = listType.GetMethod(nameof(global::System.Collections.Generic.List<object>.Add))!;
        foreach(var value in values)
        {
            add.Invoke(list, new object[] { value });
        }
        return list;
    }
        
    private static object ToArray(object genericList)
    {
        var toArray = genericList.GetType().GetMethod(nameof(global::System.Collections.Generic.List<object>.ToArray))!;
        return toArray.Invoke(genericList, null);
    }


    public static object? GetArgumentValue(this global::Microsoft.CodeAnalysis.TypedConstant constant)
    {
        return constant.Kind switch
        {
            global::Microsoft.CodeAnalysis.TypedConstantKind.Array => constant.Values.Select(x => x.GetArgumentValue()).ToArray(),
            global::Microsoft.CodeAnalysis.TypedConstantKind.Error => null,
            _                                                      => constant.Value
        };
    }

    public static object? GetArgumentValue(this global::Microsoft.CodeAnalysis.TypedConstant constant,
        global::System.Type type)
    {
        var value = constant.GetArgumentValue();
        if (type == typeof(global::System.Type))
        {
            return value == null ? null : (value as global::Microsoft.CodeAnalysis.INamedTypeSymbol)!.ToType();
        }

        if (!type.IsArray)
        {
            if (type.IsEnum)
            {
                return value == null ? null : global::System.Enum.ToObject(type, (int)value);
            }

            return value;
        }

        if (value is not object[] arr) throw new global::System.ArgumentException("constant is not an array");
        var ret = global::System.Array.CreateInstance(
            type.GetInterfaces()
                .First(static x => x.GenericTypeArguments.Length == 1 && !x.GenericTypeArguments[0].IsGenericParameter)
                .GenericTypeArguments[0], arr.Length);
        global::System.Array.Copy(arr, ret, arr.Length);
        return ret;
    }


    public static global::Microsoft.CodeAnalysis.INamedTypeSymbol? GetArgumentType(this global::Microsoft.CodeAnalysis.TypedConstant constant)
    {
        return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Type ? constant.Value as global::Microsoft.CodeAnalysis.INamedTypeSymbol : throw new global::System.ArgumentException("constant is not a type");
    }


    public static string? GetArgumentString(this global::Microsoft.CodeAnalysis.TypedConstant constant)
    {
        return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? constant.Value as string : throw new global::System.ArgumentException("constant is not a string");
    }

    public static T? GetArgumentEnum<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : global::System.Enum
    {
        return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Enum ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not an enum");
    }

    public static T? GetArgumentPrimitive<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant) where T : struct
    {
        return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Primitive ? (T?)constant.Value : throw new global::System.ArgumentException("constant is not a primitive");
    }


    public static T?[] GetArgumentArray<T>(this global::Microsoft.CodeAnalysis.TypedConstant constant)
    {
        return constant.Kind == global::Microsoft.CodeAnalysis.TypedConstantKind.Array ? constant.Values.Select(x => (T?)x.Value).ToArray() : throw new global::System.ArgumentException("constant is not an array");
    }
}