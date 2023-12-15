using Feast.CodeAnalysis.CompileTime;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class CompileTimeGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(nameof(Assembly)), Assembly.Text);
            ctx.AddSource(GenerateFileName(nameof(ConstructorInfo)), ConstructorInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(EventInfo)), EventInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(FieldInfo)), FieldInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(MemberInfo)), MemberInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(MethodInfo)), MethodInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(Module)), Module.Text);
            ctx.AddSource(GenerateFileName(nameof(ParameterInfo)), ParameterInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(PropertyInfo)), PropertyInfo.Text);
            ctx.AddSource(GenerateFileName(nameof(Type)), Type.Text);
        });
    }
}