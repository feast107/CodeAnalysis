
using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators.Base;

public abstract class AutoTextGenerator : IIncrementalGenerator
{
    protected abstract Type[] Types { get; } 
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            foreach (var type in Types)
            {
                ctx.AddSource(GenerateFileName(type.Name),
                    "#pragma warning disable\n" +GetGenerateTexts(type).First()
                );
            }
        });
    }
}