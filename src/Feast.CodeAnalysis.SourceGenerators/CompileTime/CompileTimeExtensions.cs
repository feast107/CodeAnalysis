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
        symbol switch
        {
            ITypeSymbol type => type.ToType(),
            IMethodSymbol method => method.MethodKind
                is MethodKind.Constructor or MethodKind.StaticConstructor
                ? method.ToConstructorInfo()
                : method.ToMethodInfo(),
            IFieldSymbol field       => field.ToFieldInfo(),
            IPropertySymbol property => property.ToPropertyInfo(),
            IEventSymbol @event      => @event.ToEventInfo(),
            _                        => throw new ArgumentException(nameof(symbol))
        };

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

    public static bool IsAssignableTo(this Type type, Type another) =>
        another.IsAssignableFrom(type);

    public static int GenericParameterCount(this Type type) => !type.ContainsGenericParameters
        ? 0
        : int.Parse(type.Name.Split('`')[1]);
}