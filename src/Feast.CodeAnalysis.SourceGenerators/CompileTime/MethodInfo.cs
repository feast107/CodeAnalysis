using System;
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
        new Type((method.ContainingSymbol as global::Microsoft.CodeAnalysis.ITypeSymbol)!);

    public override string Name => method.MetadataName;

    public override global::System.Type ReflectedType => new Type(method.ReturnType);

    public override System.Reflection.MethodImplAttributes GetMethodImplementationFlags()
    {
        var ret = System.Reflection.MethodImplAttributes.Managed;
        return ret;
    }

    public override global::System.Reflection.ParameterInfo[] GetParameters() =>
        method
            .Parameters
            .Select(static x => (global::System.Reflection.ParameterInfo)new ParameterInfo(x))
            .ToArray();

    public override object Invoke(object obj, 
        System.Reflection.BindingFlags invokeAttr,
        System.Reflection.Binder binder,
        object[] parameters,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override System.Reflection.MethodAttributes Attributes
    {
        get
        {
            var ret = System.Reflection.MethodAttributes.PrivateScope;
            if (method.IsStatic)
                ret |= System.Reflection.MethodAttributes.Static;
            if (method.IsVirtual)
                ret |= System.Reflection.MethodAttributes.Virtual;
            if (method.IsAbstract)
                ret |= System.Reflection.MethodAttributes.Abstract;
            switch (method.DeclaredAccessibility)
            {
                case Microsoft.CodeAnalysis.Accessibility.Public:
                    ret |= System.Reflection.MethodAttributes.Public;
                    break;
                case Microsoft.CodeAnalysis.Accessibility.Protected or Microsoft.CodeAnalysis.Accessibility.Private:
                    ret |= System.Reflection.MethodAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override RuntimeMethodHandle MethodHandle => throw new NotSupportedException();

    public override global::System.Reflection.MethodInfo GetBaseDefinition() => new MethodInfo(method.OriginalDefinition);

    public override System.Reflection.ICustomAttributeProvider ReturnTypeCustomAttributes =>
        throw new NotImplementedException();
}