using Feast.CodeAnalysis.CompileTime;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;
using AttributeData = Feast.CodeAnalysis.CompileTime.AttributeData;
using CompileTimeExtensions = Feast.CodeAnalysis.CompileTime.CompileTimeExtensions;
using Type = Feast.CodeAnalysis.CompileTime.Type;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class CompileTimeGenerator : AutoTextGenerator
{
    protected override System.Type[] Types =>
    [
        typeof(Assembly),
        typeof(AttributeData),
        typeof(ConstructorInfo),
        typeof(EventInfo),
        typeof(FieldInfo),
#if False
        typeof(MemberInfo), //member info will be removed
#endif
        typeof(MethodInfo),
        typeof(Module),
        typeof(ParameterInfo),
        typeof(PropertyInfo),
        typeof(Type),
        typeof(CompileTimeExtensions),
        typeof(TypeEqualityComparer),
        typeof(AssemblyEqualityComparer),
    ];
}