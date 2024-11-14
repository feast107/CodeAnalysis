using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.FieldInfo")]
internal partial class FieldInfo(global::Microsoft.CodeAnalysis.IFieldSymbol symbol) : global::System.Reflection.FieldInfo
{
    internal IFieldSymbol Symbol => symbol;
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

    public override string              Name          => symbol.MetadataName;
    public override global::System.Type ReflectedType => FieldType;

    public override object GetValue(object obj) => throw new NotSupportedException();

    public override void SetValue(object obj,
        object value,
        System.Reflection.BindingFlags invokeAttr,
        System.Reflection.Binder binder,
        System.Globalization.CultureInfo culture) => throw new NotSupportedException();

    public override System.Reflection.FieldAttributes Attributes
    {
        get
        {
            var ret                          = System.Reflection.FieldAttributes.PrivateScope;
            if (symbol.IsStatic) ret         |= System.Reflection.FieldAttributes.Static;
            if (symbol.IsReadOnly) ret       |= System.Reflection.FieldAttributes.InitOnly;
            if (symbol.HasConstantValue) ret |= System.Reflection.FieldAttributes.HasDefault;
            if (symbol.IsConst) ret          |= System.Reflection.FieldAttributes.Literal;

            switch (symbol.DeclaredAccessibility)
            {
                case Microsoft.CodeAnalysis.Accessibility.Public:
                    ret |= System.Reflection.FieldAttributes.Public;
                    break;
                default:
                    ret |= System.Reflection.FieldAttributes.Private;
                    break;
            }

            return ret;
        }
    }

    public override RuntimeFieldHandle FieldHandle => throw new NotSupportedException();
    public override global::System.Type FieldType => new Type(symbol.Type);
}