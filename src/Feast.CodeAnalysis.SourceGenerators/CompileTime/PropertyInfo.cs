using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.PropertyInfo")]
internal partial class PropertyInfo(global::Microsoft.CodeAnalysis.IPropertySymbol property)
    : global::System.Reflection.PropertyInfo
{
    public override object[] GetCustomAttributes(bool inherit)
        => property.GetAttributes()
            .Cast<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        property.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        property
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType => new Type(property.ContainingType);
    public override string              Name          => property.Name;
    public override global::System.Type ReflectedType => PropertyType;

    public override global::System.Reflection.MethodInfo[] GetAccessors(bool nonPublic)
        => [GetGetMethod(nonPublic), GetSetMethod(nonPublic)];

    public override global::System.Reflection.MethodInfo? GetGetMethod(bool nonPublic) =>
        property.GetMethod == null
            ? null
            : property.GetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? new MethodInfo(property.GetMethod)
                : null;

    public override global::System.Reflection.ParameterInfo[] GetIndexParameters() =>
        property
            .Parameters
            .Select(x => (global::System.Reflection.ParameterInfo)new ParameterInfo(x))
            .ToArray();

    public override global::System.Reflection.MethodInfo? GetSetMethod(bool nonPublic) =>
        property.SetMethod == null
            ? null
            : property.SetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? new MethodInfo(property.SetMethod)
                : null;

    public override object GetValue(object obj,
        System.Reflection.BindingFlags invokeAttr,
        System.Reflection.Binder binder,
        object[] index,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override void SetValue(object obj,
        object value,
        System.Reflection.BindingFlags invokeAttr,
        System.Reflection.Binder binder,
        object[] index,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override System.Reflection.PropertyAttributes Attributes =>
        System.Reflection.PropertyAttributes.SpecialName;

    public override bool CanRead  => !property.IsWriteOnly;
    public override bool CanWrite => !property.IsReadOnly;

    public override System.Reflection.Module Module => property.ContainingModule.ToModule();

    public override global::System.Type PropertyType => new Type(property.Type);
}