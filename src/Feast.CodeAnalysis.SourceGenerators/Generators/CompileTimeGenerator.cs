using Feast.CodeAnalysis.CompileTime;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;
using AttributeData = Feast.CodeAnalysis.CompileTime.AttributeData;
using CompileTimeExtensions = Feast.CodeAnalysis.CompileTime.CompileTimeExtensions;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class CompileTimeGenerator: AutoTextGenerator
{
    protected override System.Type[] Types =>
    [
        typeof(Assembly),
        typeof(AttributeData),
        typeof(ConstructorInfo),
        typeof(EventInfo),
        typeof(FieldInfo),
        typeof(MemberInfo),
        typeof(MethodInfo),
        typeof(Module),
        typeof(ParameterInfo),
        typeof(PropertyInfo),
        typeof(Type),
        typeof(CompileTimeExtensions),
        typeof(TypeEqualityComparer),
    ];
}