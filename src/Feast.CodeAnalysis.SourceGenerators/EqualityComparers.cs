using System;
using System.Reflection;

// ReSharper disable CheckForReferenceEqualityInstead.1

namespace Feast.CodeAnalysis;

[Literal("Feast.CodeAnalysis.TypeEqualityComparer")]
public partial class TypeEqualityComparer :  System.Collections.Generic.IEqualityComparer<Type>
{
    public bool Equals(Type x, Type y) =>
        x is CompileTime.Type
            ? x.Equals(y)
            : y is CompileTime.Type
                ? y.Equals(x)
                : x.Equals(y);

    public int GetHashCode(Type obj) =>
        obj is global::Feast.CodeAnalysis.CompileTime.Type type
            ? type.GetHashCode()
            : obj.GetHashCode();

    public static TypeEqualityComparer Default { get; } = new();
}

[Literal("Feast.CodeAnalysis.AssemblyEqualityComparer")]
public partial class AssemblyEqualityComparer :  System.Collections.Generic.IEqualityComparer<Assembly>
{
    public bool Equals(Assembly x, Assembly y) =>
        x is CompileTime.Assembly assemblyX
            ? assemblyX.Equals(y)
            : y is CompileTime.Assembly assemblyY
                ? assemblyY.Equals(x)
                : x.Equals(y);

    public int GetHashCode(Assembly obj) =>
        obj is CompileTime.Assembly assembly
            ? assembly.GetHashCode()
            : obj.GetHashCode();

    public static AssemblyEqualityComparer Default { get; } = new();
}