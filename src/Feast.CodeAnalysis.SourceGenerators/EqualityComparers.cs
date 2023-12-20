using System;
// ReSharper disable CheckForReferenceEqualityInstead.1

namespace Feast.CodeAnalysis;

[Literal("Feast.CodeAnalysis.TypeEqualityComparer")]
public partial class TypeEqualityComparer :  global::System.Collections.Generic.IEqualityComparer<global::System.Type>
{
    public bool Equals(global::System.Type x, global::System.Type y) =>
        x is global::Feast.CodeAnalysis.CompileTime.Type
            ? x.Equals(y)
            : y is global::Feast.CodeAnalysis.CompileTime.Type
                ? y.Equals(x)
                : x.Equals(y);

    public int GetHashCode(global::System.Type obj) =>
        obj is global::Feast.CodeAnalysis.CompileTime.Type type
            ? type.GetHashCode()
            : obj.GetHashCode();

    public static global::Feast.CodeAnalysis.TypeEqualityComparer Default { get; } = new();
}


public partial class AssemblyEqualityComparer :  global::System.Collections.Generic.IEqualityComparer<global::System.Reflection.Assembly>
{
    public bool Equals(global::System.Reflection.Assembly x, global::System.Reflection.Assembly y) =>
        x is global::Feast.CodeAnalysis.CompileTime.Assembly assemblyX
            ? assemblyX.Equals(y)
            : y is global::Feast.CodeAnalysis.CompileTime.Assembly assemblyY
                ? assemblyY.Equals(x)
                : x.Equals(y);

    public int GetHashCode(global::System.Reflection.Assembly obj) =>
        obj is global::Feast.CodeAnalysis.CompileTime.Assembly assembly
            ? assembly.GetHashCode()
            : obj.GetHashCode();

    public static global::Feast.CodeAnalysis.AssemblyEqualityComparer Default { get; } = new();
}