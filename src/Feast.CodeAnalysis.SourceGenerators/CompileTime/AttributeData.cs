using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.AttributeData")]
public partial class AttributeData(global::Microsoft.CodeAnalysis.AttributeData attributeData)
    : CustomAttributeData
{
    public override System.Reflection.ConstructorInfo Constructor =>
        new ConstructorInfo(attributeData.AttributeConstructor!);

    public override IList<CustomAttributeTypedArgument> ConstructorArguments =>
        attributeData.ConstructorArguments.Select((x,i) =>
        {
            var value = x.GetArgumentValue();
            return new CustomAttributeTypedArgument(
                attributeData.AttributeConstructor!.Parameters[i].Type.ToType(), value);
        }).ToList();

    public override IList<CustomAttributeNamedArgument> NamedArguments =>
        attributeData.NamedArguments.Select(x =>
            {
                var value = x.Value.GetArgumentValue();
                if (value is null) return (CustomAttributeNamedArgument?)null;
                return new CustomAttributeNamedArgument(
                    attributeData.AttributeClass!.GetMembers()
                        .First(p => p.Name == x.Key).ToMemberInfo(), value);
            })
            .Where(static x => x != null)
            .Cast<CustomAttributeNamedArgument>()
            .ToList();
}