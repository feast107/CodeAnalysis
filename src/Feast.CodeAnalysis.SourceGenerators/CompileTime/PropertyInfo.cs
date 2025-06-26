using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.PropertyInfo")]
internal partial class PropertyInfo(global::Microsoft.CodeAnalysis.IPropertySymbol symbol)
    : global::System.Reflection.PropertyInfo
{
    internal IPropertySymbol Symbol => symbol;
    
    public override object[] GetCustomAttributes(bool inherit)
        => symbol.GetAttributes()
            .Cast<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        symbol.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        symbol
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType => new Type(symbol.ContainingType);
    public override string              Name          => symbol.MetadataName;
    public override global::System.Type ReflectedType => PropertyType;
    
    public override System.Reflection.MethodInfo? GetMethod =>
        symbol.GetMethod == null
            ? null
            : new MethodInfo(symbol.GetMethod);

    public override System.Reflection.MethodInfo? SetMethod =>
        symbol.SetMethod == null
            ? null
            : new MethodInfo(symbol.SetMethod);

    public override global::System.Reflection.MethodInfo[] GetAccessors(bool nonPublic)
    {
        var list      = new global::System.Collections.Generic.List<global::System.Reflection.MethodInfo>();
        var getMethod = GetGetMethod(nonPublic);
        if (getMethod != null) list.Add(getMethod);
        var setMethod = GetSetMethod(nonPublic);
        if (setMethod != null) list.Add(setMethod);
        return list.ToArray();
    }

    public override global::System.Reflection.MethodInfo? GetGetMethod(bool nonPublic) =>
        symbol.GetMethod == null
            ? null
            : symbol.GetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? GetMethod
                : null;

    public override global::System.Reflection.MethodInfo? GetSetMethod(bool nonPublic) =>
        symbol.SetMethod == null
            ? null
            : symbol.SetMethod.DeclaredAccessibility != Accessibility.Public == nonPublic
                ? SetMethod
                : null;

    public override void SetValue(object obj, object value, object[] index) => throw new NotSupportedException();
    public override object GetValue(object obj, object[] index) => throw new NotSupportedException();

    public override global::System.Reflection.ParameterInfo[] GetIndexParameters() =>
        symbol
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

    public override bool CanRead  => !symbol.IsWriteOnly;
    public override bool CanWrite => !symbol.IsReadOnly;
    
    public override MemberTypes MemberType => MemberTypes.Property; 

    public override System.Reflection.Module Module => symbol.ContainingModule.ToModule();

    public override global::System.Type PropertyType => new Type(symbol.Type);

    public bool HasNullableAnnotation => symbol.NullableAnnotation == NullableAnnotation.Annotated;
}