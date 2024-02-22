using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.PropertyInfo")]
internal partial class PropertyInfo(global::Microsoft.CodeAnalysis.IPropertySymbol property)
    : global::System.Reflection.PropertyInfo
{
    public IPropertySymbol Symbol => property;
    
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
    public override string              Name          => property.MetadataName;
    public override global::System.Type ReflectedType => PropertyType;
    
    public override System.Reflection.MethodInfo? GetMethod =>
        property.GetMethod == null
            ? null
            : new MethodInfo(property.GetMethod);

    public override System.Reflection.MethodInfo? SetMethod =>
        property.SetMethod == null
            ? null
            : new MethodInfo(property.SetMethod);

    public override global::System.Reflection.MethodInfo[] GetAccessors(bool nonPublic)
        => [GetGetMethod(nonPublic), GetSetMethod(nonPublic)];

    public override global::System.Reflection.MethodInfo? GetGetMethod(bool nonPublic) =>
        property.GetMethod == null
            ? null
            : property.GetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? GetMethod
                : null;

    public override global::System.Reflection.MethodInfo? GetSetMethod(bool nonPublic) =>
        property.SetMethod == null
            ? null
            : property.SetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? SetMethod
                : null;

    public override void SetValue(object obj, object value, object[] index) => throw new NotSupportedException();
    public override object GetValue(object obj, object[] index) => throw new NotSupportedException();

    public override global::System.Reflection.ParameterInfo[] GetIndexParameters() =>
        property
            .Parameters
            .Select(x => (global::System.Reflection.ParameterInfo)new ParameterInfo(x))
            .ToArray();



    public override object GetValue(object obj,
        BindingFlags invokeAttr,
        Binder binder,
        object[] index,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override void SetValue(object obj,
        object value,
        BindingFlags invokeAttr,
        Binder binder,
        object[] index,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override PropertyAttributes Attributes => PropertyAttributes.SpecialName;

    public override bool CanRead  => !property.IsWriteOnly;
    public override bool CanWrite => !property.IsReadOnly;
    
    public override MemberTypes MemberType => MemberTypes.Property; 

    public override System.Reflection.Module Module => property.ContainingModule.ToModule();

    public override global::System.Type PropertyType => new Type(property.Type);

    public bool HasNullableAnnotation => property.NullableAnnotation == NullableAnnotation.Annotated;
}