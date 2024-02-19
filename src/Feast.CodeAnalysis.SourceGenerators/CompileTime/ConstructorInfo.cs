using System;
using System.Linq;
using System.Reflection;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.ConstructorInfo")]
internal partial class ConstructorInfo(global::Microsoft.CodeAnalysis.IMethodSymbol constructor)
    : global::System.Reflection.ConstructorInfo
{
    public override object[] GetCustomAttributes(bool inherit) =>
        constructor
            .GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        constructor
            .GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        constructor
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType =>
        new Type((constructor.ContainingSymbol as global::Microsoft.CodeAnalysis.ITypeSymbol)!);

    public override string Name => constructor.MetadataName;

    public override global::System.Type ReflectedType =>
        new Type(constructor.ReturnType);

    public override MethodImplAttributes GetMethodImplementationFlags() =>
        throw new NotSupportedException();

    public override global::System.Reflection.ParameterInfo[] GetParameters() =>
        constructor
            .Parameters
            .Select(static x =>
                (global::System.Reflection.ParameterInfo)
                new ParameterInfo(x))
            .ToArray();

    public override object Invoke(object obj,
        BindingFlags invokeAttr,
        Binder binder,
        object[] parameters,
        System.Globalization.CultureInfo culture) =>
        throw new NotSupportedException();

    public override MethodAttributes Attributes
    {
        get
        {
            var ret = MethodAttributes.PrivateScope;
            if (constructor.IsStatic)
                ret |= MethodAttributes.Static;
            if (constructor.IsVirtual)
                ret |= MethodAttributes.Virtual;
            if (constructor.IsAbstract)
                ret |= MethodAttributes.Abstract;
            switch (constructor.DeclaredAccessibility)
            {
                case Microsoft.CodeAnalysis.Accessibility.Public:
                    ret |= MethodAttributes.Public;
                    break;
                case Microsoft.CodeAnalysis.Accessibility.Protected or Microsoft.CodeAnalysis.Accessibility.Private:
                    ret |= MethodAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

    public override object Invoke(BindingFlags invokeAttr,
        Binder binder,
        object[] parameters,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();
}