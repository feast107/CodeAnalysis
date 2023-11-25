using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Utils;

[DebuggerDisplay("{Name}")]
public abstract class TypeInfo
{
    public abstract string Name { get; }

    /// <summary>
    /// 是否是泛型参数
    /// </summary>
    public abstract bool IsParameter { get; }

    /// <summary>
    /// 是否是接口
    /// </summary>
    public abstract bool IsInterface { get; }

    /// <summary>
    /// 是否是泛型
    /// </summary>
    public bool IsGeneric => GenericTypes.Value.Count > 0;

    /// <summary>
    /// 泛型参数
    /// </summary>
    public abstract Lazy<IReadOnlyList<TypeInfo>> GenericTypes { get; }

    /// <summary>
    /// 基类
    /// </summary>
    public abstract Lazy<TypeInfo?> BaseClass { get; }

    /// <summary>
    /// 实现的所有接口
    /// </summary>
    public abstract Lazy<IReadOnlyList<TypeInfo>> Interfaces { get; }

    /// <summary>
    /// 本类型是否可以赋值给另一个类型
    /// </summary>
    /// <param name="another"></param>
    /// <returns></returns>
    public bool IsAssignableTo(TypeInfo another)
    {
        if (another.IsParameter) return true;
        switch (this)
        {
            case { IsInterface: true } when another.IsInterface:
                if (Name != another.Name) return false;
                if (!IsGeneric && !another.IsGeneric) return true;
                if (GenericTypes.Value.Count != another.GenericTypes.Value.Count) return false;
                return !GenericTypes.Value
                    .Where((t, i) => !t.IsAssignableTo(another.GenericTypes.Value[i]))
                    .Any();
            case { IsInterface: false } when another.IsInterface:
                return Interfaces.Value
                    .Where(x => x.Name == another.Name)
                    .Any(interfaceInfo => interfaceInfo.IsAssignableTo(another));
            case { IsInterface: false } when !another.IsInterface:
                return Equals(another) || IsSubClassOf(another);
        }

        return false;
    }

    private bool IsSubClassOf(TypeInfo another)
    {
        var parent = BaseClass.Value;
        while (parent != null)
        {
            if (another.Equals(parent)) return true;
            parent = parent.BaseClass.Value;
        }

        return false;
    }


    public override int GetHashCode() => Name.GetHashCode() | GenericTypes.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not TypeInfo another) return false;
        if (Name        != another.Name) return false;
        if (IsParameter != another.IsParameter) return false;
        if (!IsGeneric && !another.IsGeneric) return true;
        if (IsInterface               != another.IsInterface) return false;
        if (GenericTypes.Value.Count != another.GenericTypes.Value.Count) return false;
        return !GenericTypes.Value.Where((t, i) => !t.Equals(another.GenericTypes.Value[i])).Any();
    }

    public static TypeInfo FromType(Type type) => new RuntimeTypeInfo(type);
    public static TypeInfo FromSymbol(INamedTypeSymbol symbol) => new SymbolTypeInfo(symbol);
}