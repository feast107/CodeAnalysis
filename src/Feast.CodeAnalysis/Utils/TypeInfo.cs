using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Utils;

[DebuggerDisplay("{FullName}")]
public abstract class TypeInfo
{
    public string FullName => Namespace != null ? $"{Namespace}.{Name}" : Name;

    /// <summary>
    /// when null if global::
    /// </summary>
    public abstract string? Namespace { get; }

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
    public bool IsGeneric => GenericTypes.Count > 0;

    /// <summary>
    /// 泛型参数
    /// </summary>
    public IReadOnlyList<TypeInfo> GenericTypes => genericTypes.Value;

    protected abstract Lazy<IReadOnlyList<TypeInfo>> genericTypes { get; }

    /// <summary>
    /// 基类，如果没有则为null
    /// </summary>
    public TypeInfo? BaseClass => baseClass.Value;

    protected abstract Lazy<TypeInfo?> baseClass { get; }

    /// <summary>
    /// 实现的所有接口
    /// </summary>
    public IReadOnlyList<TypeInfo> Interfaces => interfaces.Value;

    protected abstract Lazy<IReadOnlyList<TypeInfo>> interfaces { get; }

    /// <summary>
    /// 泛型参数下的约束
    /// </summary>
    public IReadOnlyList<TypeInfo> ConstrainedTypes => constrainedTypes.Value;

    protected abstract Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; }

    /// <summary>
    /// 本类型是否可以从另一个类型赋值
    /// </summary>
    /// <param name="another"></param>
    /// <returns></returns>
    public bool IsAssignableFrom(TypeInfo another) => another.IsAssignableTo(this);

    /// <summary>
    /// 本类型是否可以赋值给另一个类型
    /// </summary>
    /// <param name="another"></param>
    /// <returns></returns>
    public bool IsAssignableTo(TypeInfo another)
    {
        switch (another)
        {
            case { IsParameter: true }:
                return another.ConstrainedTypes.Count == 0 || another.ConstrainedTypes.All(IsAssignableTo);
            case { IsInterface: true } when IsInterface:
                if (FullName != another.FullName) return false;
                if (!IsGeneric && !another.IsGeneric) return true;
                if (GenericTypes.Count != another.GenericTypes.Count) return false;
                return !GenericTypes
                    .Where((t, i) => !t.IsAssignableTo(another.GenericTypes[i]))
                    .Any();
            case { IsInterface: true } when !IsInterface:
                return Interfaces
                    .Where(x => x.FullName == another.FullName)
                    .Any(interfaceInfo => interfaceInfo.IsAssignableTo(another));
            case { IsInterface: false } when !IsInterface:
                return SameAs(another) || IsSubClassOf(another);
        }

        return false;
    }

    /// <summary>
    /// 是否是另一个类型的子类
    /// </summary>
    /// <param name="another"></param>
    /// <returns></returns>
    private bool IsSubClassOf(TypeInfo another)
    {
        if (IsParameter || another.IsParameter ||
            IsInterface || another.IsInterface) return false;
        var parent = BaseClass;
        while (parent != null)
        {
            if (another.SameAs(parent)) return true;
            parent = parent.BaseClass;
        }

        return false;
    }

    /// <summary>
    /// 是否相同
    /// </summary>
    /// <param name="another"></param>
    /// <returns></returns>
    public bool SameAs(TypeInfo another)
    {
        if (FullName    != another.FullName) return false;
        if (IsParameter)
        {
            if (!another.IsParameter) return false;
            if (FullName != another.FullName) return false;
            return !ConstrainedTypes.Where((t, i) => !t.SameAs(another.ConstrainedTypes[i])).Any();
        }
        if (!IsGeneric && !another.IsGeneric) return true;
        if (IsInterface        != another.IsInterface) return false;
        if (GenericTypes.Count != another.GenericTypes.Count) return false;
        return !GenericTypes.Where((t, i) => !t.SameAs(another.GenericTypes[i])).Any();
    }

    public static TypeInfo FromType(Type type) => new RuntimeTypeInfo(type);
    public static TypeInfo FromSymbol(ITypeSymbol symbol) => new SymbolTypeInfo(symbol);
}