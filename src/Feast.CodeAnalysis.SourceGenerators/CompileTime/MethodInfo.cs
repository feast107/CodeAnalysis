using System;
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

    public override global::System.Type DeclaringType => new Type((symbol.ContainingSymbol as ITypeSymbol)!);

    public override string Name => symbol.MetadataName;

    public override global::System.Type ReflectedType => new Type(symbol.ReturnType);

    public override System.Type ReturnType => new Type(symbol.ReturnType);

    public override System.Reflection.Module Module => new Module(symbol.ContainingModule);

    public override bool IsGenericMethod => symbol.IsGenericMethod;

    public override MemberTypes MemberType => symbol.MethodKind switch
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
                case Accessibility.Protected or Accessibility.Private:
                    ret |= MethodAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

    public override global::System.Reflection.MethodInfo GetBaseDefinition() => new MethodInfo(symbol.OriginalDefinition);

    public override ICustomAttributeProvider ReturnTypeCustomAttributes =>
        throw new NotImplementedException();
}