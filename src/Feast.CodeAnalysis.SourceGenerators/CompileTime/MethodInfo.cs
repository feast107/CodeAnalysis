﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.MethodInfo")]
internal partial class MethodInfo(global::Microsoft.CodeAnalysis.IMethodSymbol symbol)
    : global::System.Reflection.MethodInfo
{
    internal IMethodSymbol Symbol => symbol;
    public override object[] GetCustomAttributes(bool inherit) =>
        symbol.GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        symbol.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        symbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType => new Type(symbol.ContainingType);
    
    public override string Name => symbol.MetadataName;

    public override global::System.Type ReflectedType => new Type(symbol.ContainingType);

    public override System.Type ReturnType => new Type(symbol.ReturnType);

    public override System.Reflection.Module Module => new Module(symbol.ContainingModule);

    public override bool IsGenericMethod => symbol.IsGenericMethod;

    public override MemberTypes MemberType => symbol.MethodKind switch
    {
        MethodKind.Constructor       => MemberTypes.Constructor,
        MethodKind.StaticConstructor => MemberTypes.Constructor,
        _                            => MemberTypes.Method
    };
    

    public override MethodImplAttributes GetMethodImplementationFlags()
    {
        var ret = System.Reflection.MethodImplAttributes.Managed;
        return ret;
    }

    public override global::System.Reflection.ParameterInfo[] GetParameters() =>
        symbol
            .Parameters
            .Select(static x => (global::System.Reflection.ParameterInfo)new ParameterInfo(x))
            .ToArray();

    public override bool ContainsGenericParameters => symbol.IsGenericMethod;

    public override object Invoke(object obj, 
        BindingFlags invokeAttr,
        Binder binder,
        object[] parameters,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override MethodAttributes Attributes
    {
        get
        {
            var ret = MethodAttributes.PrivateScope;
            if (symbol.IsStatic)
                ret |= MethodAttributes.Static;
            if (symbol.IsVirtual)
                ret |= MethodAttributes.Virtual;
            if (symbol.IsAbstract)
                ret |= MethodAttributes.Abstract;
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    ret |= MethodAttributes.Public;
                    break;
                default:
                    ret |= MethodAttributes.Private;
                    break;
            }

            if (symbol.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet or MethodKind.Constructor)
            {
                ret |= MethodAttributes.SpecialName;
                ret |= MethodAttributes.HideBySig;

                if (symbol.MethodKind == MethodKind.Constructor)
                {
                    ret |= MethodAttributes.RTSpecialName;
                }
            }
            return ret;
        }
    }

    public override MethodBody? GetMethodBody() => throw new NotSupportedException();

    public override Delegate CreateDelegate(System.Type delegateType) => throw new NotSupportedException();

    public override Delegate CreateDelegate(System.Type delegateType, object target) =>
        throw new NotSupportedException();

    public override System.Reflection.MethodInfo GetGenericMethodDefinition() =>
        new MethodInfo(symbol.OriginalDefinition);

    public override bool IsGenericMethodDefinition => symbol is { IsDefinition: true, IsGenericMethod: true };

    public override System.Reflection.MethodInfo MakeGenericMethod(params System.Type[] typeArguments) =>
        throw new NotSupportedException();

    public override System.Type[] GetGenericArguments() =>
        symbol.TypeArguments.Select(x => new Type(x) as System.Type).ToArray();

    public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

    public override global::System.Reflection.MethodInfo GetBaseDefinition() => new MethodInfo(symbol.OriginalDefinition);

    public override ICustomAttributeProvider ReturnTypeCustomAttributes => new CustomAttributeProvider(symbol.ReturnType);
    
    private class CustomAttributeProvider(ISymbol symbol) : ICustomAttributeProvider
    {
        public object[] GetCustomAttributes(bool inherit) =>
            symbol.GetAttributes()
                .CastArray<object>()
                .ToArray();

        public object[] GetCustomAttributes(System.Type attributeType, bool inherit) =>
            symbol.GetAttributes()
                .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
                .Cast<object>()
                .ToArray();

        public bool IsDefined(System.Type attributeType, bool inherit) =>
            symbol.GetAttributes()
                .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);
    }
}