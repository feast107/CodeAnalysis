using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.FieldInfo")]
internal partial class FieldInfo(global::Microsoft.CodeAnalysis.IFieldSymbol field) : global::System.Reflection.FieldInfo
{
    internal IFieldSymbol Symbol => field;
    public override object[] GetCustomAttributes(bool inherit) =>
        field.GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        field.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        field.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);


    public override global::System.Type DeclaringType => new Type(field.ContainingType);

    public override string              Name          => field.MetadataName;
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
            var ret                         = System.Reflection.FieldAttributes.PrivateScope;
            if (field.IsStatic) ret         |= System.Reflection.FieldAttributes.Static;
            if (field.IsReadOnly) ret       |= System.Reflection.FieldAttributes.InitOnly;
            if (field.HasConstantValue) ret |= System.Reflection.FieldAttributes.HasDefault;
            if (field.IsConst) ret          |= System.Reflection.FieldAttributes.Literal;

            switch (field.DeclaredAccessibility)
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
    public override global::System.Type FieldType => new Type(field.Type);
}