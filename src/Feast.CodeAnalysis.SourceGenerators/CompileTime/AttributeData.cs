﻿using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.AttributeData")]
public partial class AttributeData(global::Microsoft.CodeAnalysis.AttributeData attributeData)
    : global::System.Reflection.CustomAttributeData
{
    public override System.Reflection.ConstructorInfo Constructor =>
        new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(attributeData.AttributeConstructor!);

    public override global::System.Collections.Generic.IList<global::System.Reflection.CustomAttributeTypedArgument> ConstructorArguments =>
        attributeData.ConstructorArguments.Select((x,i) =>
        {
            var value = x.GetArgumentValue();
            return new global::System.Reflection.CustomAttributeTypedArgument(
                attributeData.AttributeConstructor!.Parameters[i].Type.ToType(), value);
        }).ToList();

    public override global::System.Collections.Generic.IList<global::System.Reflection.CustomAttributeNamedArgument> NamedArguments =>
        attributeData.NamedArguments.Select(x =>
            {
                var value = x.Value.GetArgumentValue();
                if (value is null) return (global::System.Reflection.CustomAttributeNamedArgument?)null;
                return new global::System.Reflection.CustomAttributeNamedArgument(
                    attributeData.AttributeClass!.GetMembers()
                        .First(p => p.Name == x.Key).ToMemberInfo(),
                    value);
            }).Where(static x => x != null)
            .Cast<global::System.Reflection.CustomAttributeNamedArgument>()
            .ToList();
}