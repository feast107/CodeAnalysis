using System;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.ParameterInfo")]
internal partial class ParameterInfo(global::Microsoft.CodeAnalysis.IParameterSymbol symbol)
    : global::System.Reflection.ParameterInfo
{
    internal IParameterSymbol Symbol => symbol;
    public override   string           Name => symbol.MetadataName;

    public override global::System.Type ParameterType => new Type(symbol.Type);

    public override System.Reflection.ParameterAttributes Attributes
    {
        get
        {
            var ret = System.Reflection.ParameterAttributes.None;
            if (symbol.IsOptional)
                ret |= System.Reflection.ParameterAttributes.Optional;
            switch (symbol.RefKind)
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

    public override object? DefaultValue => symbol.ExplicitDefaultValue;

    public override bool HasDefaultValue => symbol.HasExplicitDefaultValue;

}