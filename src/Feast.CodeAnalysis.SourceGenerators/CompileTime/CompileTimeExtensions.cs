using System;
using System.Reflection;

namespace Microsoft.CodeAnalysis;

[Literal("Feast.CodeAnalysis.CompileTime.CompileTimeExtensions")]
public static partial class CompileTimeExtensions
{
    public static Assembly ToAssembly(this IAssemblySymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.Assembly(symbol);

    public static Module ToModule(this IModuleSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.Module(symbol);

    public static Type ToType(this ITypeSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.Type(symbol);

    public static MemberInfo ToMemberInfo(this ISymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.MemberInfo(symbol);

    public static MethodInfo ToMethodInfo(this IMethodSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.MethodInfo(symbol);

    public static FieldInfo ToFieldInfo(this IFieldSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.FieldInfo(symbol);

    public static PropertyInfo ToPropertyInfo(this IPropertySymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(symbol);

    public static ConstructorInfo ToConstructorInfo(this IMethodSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(symbol);

    public static EventInfo ToEventInfo(this IEventSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.EventInfo(symbol);

    public static ParameterInfo ToParameterInfo(this IParameterSymbol symbol) =>
        new global::Feast.CodeAnalysis.CompileTime.ParameterInfo(symbol);

    public static bool IsAssignableTo(this global::System.Type type, global::System.Type another) =>
        another.IsAssignableFrom(type);
}