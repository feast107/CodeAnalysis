using System;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators.Base;

public abstract class AutoTextGenerator : IIncrementalGenerator
{
    protected abstract string ClassName { get; }
    
    protected abstract Type Type { get; } 
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(GenerateFileName(ClassName),
                Generate(ClassName,
                    GetGenerateTexts(Type)
                )
            );
        });
    }
}