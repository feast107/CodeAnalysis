using System;
// ReSharper disable CheckForReferenceEqualityInstead.1

namespace Feast.CodeAnalysis;

[Literal("Feast.CodeAnalysis.TypeEqualityComparer")]
public partial class TypeEqualityComparer :  global::System.Collections.Generic.IEqualityComparer<global::System.Type>
{
    public bool Equals(Type x, Type y) =>
        x is global::Feast.CodeAnalysis.CompileTime.Type
            ? x.Equals(y)
            : y is global::Feast.CodeAnalysis.CompileTime.Type
                ? y.Equals(x)
                : x.Equals(y);

    public int GetHashCode(Type obj) =>
        obj is global::Feast.CodeAnalysis.CompileTime.Type type
            ? type.GetHashCode()
            : obj.GetHashCode();

    public static global::Feast.CodeAnalysis.TypeEqualityComparer Default { get; } = new();
}