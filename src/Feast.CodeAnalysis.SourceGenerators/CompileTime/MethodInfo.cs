﻿using System;
using System.Linq;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.MethodInfo")]
internal partial class MethodInfo(global::Microsoft.CodeAnalysis.IMethodSymbol method)
    : global::System.Reflection.MethodInfo
{
    public override object[] GetCustomAttributes(bool inherit) =>
        method.GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        method.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        method.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType =>
        new global::Feast.CodeAnalysis.CompileTime.Type(
            (method.ContainingSymbol as global::Microsoft.CodeAnalysis.ITypeSymbol)!);

    public override string Name => method.MetadataName;

    public override global::System.Type ReflectedType =>
        new global::Feast.CodeAnalysis.CompileTime.Type(method.ReturnType);

    public override global::System.Reflection.MethodImplAttributes GetMethodImplementationFlags()
    {
        var ret = global::System.Reflection.MethodImplAttributes.Managed;
        return ret;
    }

    public override global::System.Reflection.ParameterInfo[] GetParameters() =>
        method
            .Parameters
            .Select(static x =>
                (global::System.Reflection.ParameterInfo)
                new global::Feast.CodeAnalysis.CompileTime.ParameterInfo(x))
            .ToArray();

    public override object Invoke(object obj, 
        global::System.Reflection.BindingFlags invokeAttr,
        global::System.Reflection.Binder binder,
        object[] parameters,
        global::System.Globalization.CultureInfo culture) => throw new global::System.NotSupportedException();

    public override global::System.Reflection.MethodAttributes Attributes
    {
        get
        {
            var ret = global::System.Reflection.MethodAttributes.PrivateScope;
            if (method.IsStatic)
                ret |= global::System.Reflection.MethodAttributes.Static;
            if (method.IsVirtual)
                ret |= global::System.Reflection.MethodAttributes.Virtual;
            if (method.IsAbstract)
                ret |= global::System.Reflection.MethodAttributes.Abstract;
            switch (method.DeclaredAccessibility)
            {
                case Microsoft.CodeAnalysis.Accessibility.Public:
                    ret |= global::System.Reflection.MethodAttributes.Public;
                    break;
                case Microsoft.CodeAnalysis.Accessibility.Protected or Microsoft.CodeAnalysis.Accessibility.Private:
                    ret |= global::System.Reflection.MethodAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override global::System.RuntimeMethodHandle MethodHandle => throw new global::System.NotSupportedException();

    public override global::System.Reflection.MethodInfo GetBaseDefinition() =>
        new global::Feast.CodeAnalysis.CompileTime.MethodInfo(method.OriginalDefinition);

    public override global::System.Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes =>
        throw new global::System.NotImplementedException();
}