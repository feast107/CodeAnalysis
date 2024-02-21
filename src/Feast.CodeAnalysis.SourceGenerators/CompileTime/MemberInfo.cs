using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.MemberInfo")]
internal partial class MemberInfo(global::Microsoft.CodeAnalysis.ISymbol symbol) : global::System.Reflection.MemberInfo
{
    internal ISymbol Symbol => symbol;
    
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
                ITypeSymbol type => type.ContainingType != null
                    ? System.Reflection.MemberTypes.NestedType
                    : System.Reflection.MemberTypes.TypeInfo,
                IPropertySymbol => System.Reflection.MemberTypes.Property,
                IFieldSymbol    => System.Reflection.MemberTypes.Field,
                IMethodSymbol method => method.ContainingType.Constructors.Contains(
                    method)
                    ? System.Reflection.MemberTypes.Constructor
                    : System.Reflection.MemberTypes.Method,
                IEventSymbol => System.Reflection.MemberTypes.Event,
                _            => System.Reflection.MemberTypes.Custom
            };
        }
    }

    public override string Name => symbol.MetadataName;

    public override global::System.Type ReflectedType => new Type(
        symbol switch
        {
            ITypeSymbol type         => type,
            IPropertySymbol property => property.Type,
            IFieldSymbol field       => field.Type,
            IMethodSymbol method     => method.ReturnType,
            _                        => throw new ArgumentOutOfRangeException()
        });
}