using System;
using System.Linq;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.MemberInfo")]
internal partial class MemberInfo(global::Microsoft.CodeAnalysis.ISymbol symbol) : global::System.Reflection.MemberInfo
{
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

    public override global::System.Reflection.MemberTypes MemberType
    {
        get
        {
            return symbol switch
            {
                Microsoft.CodeAnalysis.ITypeSymbol type => type.ContainingType != null
                    ? System.Reflection.MemberTypes.NestedType
                    : System.Reflection.MemberTypes.TypeInfo,
                Microsoft.CodeAnalysis.IPropertySymbol => System.Reflection.MemberTypes.Property,
                Microsoft.CodeAnalysis.IFieldSymbol    => System.Reflection.MemberTypes.Field,
                Microsoft.CodeAnalysis.IMethodSymbol method => method.ContainingType.Constructors.Contains(
                    method)
                    ? System.Reflection.MemberTypes.Constructor
                    : System.Reflection.MemberTypes.Method,
                Microsoft.CodeAnalysis.IEventSymbol => System.Reflection.MemberTypes.Event,
                _                                   => System.Reflection.MemberTypes.Custom
            };
        }
    }

    public override string Name => symbol.MetadataName;

    public override global::System.Type ReflectedType => new Type(
        symbol switch
        {
            global::Microsoft.CodeAnalysis.ITypeSymbol type => type,
            global::Microsoft.CodeAnalysis.IPropertySymbol property => property.Type,
            global::Microsoft.CodeAnalysis.IFieldSymbol field => field.Type,
            global::Microsoft.CodeAnalysis.IMethodSymbol method => method.ReturnType,
            _ => throw new ArgumentOutOfRangeException()
        });
}