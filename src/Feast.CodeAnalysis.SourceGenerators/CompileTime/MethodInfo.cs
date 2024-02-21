using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

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

    public override global::System.Type DeclaringType => new Type((method.ContainingSymbol as ITypeSymbol)!);

    public override string Name => method.MetadataName;

    public override global::System.Type ReflectedType => new Type(method.ReturnType);

    public override System.Type ReturnType => new Type(method.ReturnType);

    public override System.Reflection.Module Module => new Module(method.ContainingModule);

    public override bool IsGenericMethod => method.IsGenericMethod;

    public override MemberTypes MemberType => method.MethodKind switch
    {
        MethodKind.Constructor                         => MemberTypes.Constructor,
        MethodKind.StaticConstructor                   => MemberTypes.Constructor,
        _                                              => MemberTypes.Method
    };

    public override MethodImplAttributes GetMethodImplementationFlags()
    {
        var ret = System.Reflection.MethodImplAttributes.Managed;
        return ret;
    }

    public override global::System.Reflection.ParameterInfo[] GetParameters() =>
        method
            .Parameters
            .Select(static x => (global::System.Reflection.ParameterInfo)new ParameterInfo(x))
            .ToArray();

    public override bool ContainsGenericParameters => method.IsGenericMethod;

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
            if (method.IsStatic)
                ret |= MethodAttributes.Static;
            if (method.IsVirtual)
                ret |= MethodAttributes.Virtual;
            if (method.IsAbstract)
                ret |= MethodAttributes.Abstract;
            switch (method.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    ret |= MethodAttributes.Public;
                    break;
                case Accessibility.Protected or Accessibility.Private:
                    ret |= MethodAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

    public override global::System.Reflection.MethodInfo GetBaseDefinition() => new MethodInfo(method.OriginalDefinition);

    public override ICustomAttributeProvider ReturnTypeCustomAttributes =>
        throw new NotImplementedException();
}