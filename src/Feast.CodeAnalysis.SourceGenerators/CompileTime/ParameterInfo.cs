using System;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime
{
    [Literal("Feast.CodeAnalysis.CompileTime.ParameterInfo")]
    internal partial class ParameterInfo(global::Microsoft.CodeAnalysis.IParameterSymbol parameter)
        : global::System.Reflection.ParameterInfo
    {
        public override string Name => parameter.MetadataName;

        public override global::System.Type ParameterType =>
            new global::Feast.CodeAnalysis.CompileTime.Type(parameter.Type);

        public override global::System.Reflection.ParameterAttributes Attributes
        {
            get
            {
                var ret = global::System.Reflection.ParameterAttributes.None;
                if (parameter.IsOptional)
                    ret |= global::System.Reflection.ParameterAttributes.Optional;
                switch (parameter.RefKind)
                {
                    case RefKind.Out:
                        ret |= global::System.Reflection.ParameterAttributes.Out;
                        break;
                    case RefKind.In:
                        ret |= global::System.Reflection.ParameterAttributes.In;
                        break;
                }

                return ret;
            }
        }

        public override object? DefaultValue => parameter.ExplicitDefaultValue;
    }
}