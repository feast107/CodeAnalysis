using System;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.ParameterInfo")]
internal partial class ParameterInfo(global::Microsoft.CodeAnalysis.IParameterSymbol parameter)
    : global::System.Reflection.ParameterInfo
{
    public override string Name => parameter.MetadataName;

    public override global::System.Type ParameterType => new Type(parameter.Type);

    public override System.Reflection.ParameterAttributes Attributes
    {
        get
        {
            var ret = System.Reflection.ParameterAttributes.None;
            if (parameter.IsOptional)
                ret |= System.Reflection.ParameterAttributes.Optional;
            switch (parameter.RefKind)
            {
                case Microsoft.CodeAnalysis.RefKind.Out:
                    ret |= System.Reflection.ParameterAttributes.Out;
                    break;
                case Microsoft.CodeAnalysis.RefKind.In:
                    ret |= System.Reflection.ParameterAttributes.In;
                    break;
            }

            return ret;
        }
    }

    public override object? DefaultValue => parameter.ExplicitDefaultValue;
}