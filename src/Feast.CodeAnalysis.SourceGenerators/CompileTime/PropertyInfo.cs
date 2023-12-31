﻿using System;
using System.Linq;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.PropertyInfo")]
internal partial class PropertyInfo(global::Microsoft.CodeAnalysis.IPropertySymbol property)
    : global::System.Reflection.PropertyInfo
{
    public override object[] GetCustomAttributes(bool inherit)
        => property.GetAttributes()
            .CastArray<object>()
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

    public override global::System.Type DeclaringType =>
        new global::Feast.CodeAnalysis.CompileTime.Type(property.ContainingType);

    public override string              Name          => property.Name;
    public override global::System.Type ReflectedType => PropertyType;

    public override global::System.Reflection.MethodInfo[] GetAccessors(bool nonPublic)
        => [GetGetMethod(nonPublic), GetSetMethod(nonPublic)];

    public override global::System.Reflection.MethodInfo? GetGetMethod(bool nonPublic) =>
        property.GetMethod == null
            ? null
            : property.GetMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new global::Feast.CodeAnalysis.CompileTime.MethodInfo(property.GetMethod)
                : null;

    public override global::System.Reflection.ParameterInfo[] GetIndexParameters() =>
        property
            .Parameters
            .Select(x =>
                (global::System.Reflection.ParameterInfo)
                new global::Feast.CodeAnalysis.CompileTime.ParameterInfo(x))
            .ToArray();

    public override global::System.Reflection.MethodInfo? GetSetMethod(bool nonPublic) =>
        property.SetMethod == null
            ? null
            : property.SetMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new global::Feast.CodeAnalysis.CompileTime.MethodInfo(property.SetMethod)
                : null;

    public override object GetValue(object obj,
        global::System.Reflection.BindingFlags invokeAttr,
        global::System.Reflection.Binder binder,
        object[] index,
        global::System.Globalization.CultureInfo culture) => throw new global::System.NotSupportedException();

    public override void SetValue(object obj,
        object value,
        global::System.Reflection.BindingFlags invokeAttr,
        global::System.Reflection.Binder binder,
        object[] index,
        global::System.Globalization.CultureInfo culture) => throw new global::System.NotSupportedException();

    public override global::System.Reflection.PropertyAttributes Attributes =>
        global::System.Reflection.PropertyAttributes.SpecialName;

    public override bool CanRead  => !property.IsWriteOnly;
    public override bool CanWrite => !property.IsReadOnly;

    public override global::System.Type PropertyType =>
        new global::Feast.CodeAnalysis.CompileTime.Type(property.Type);
}