using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.TestGenerator;

[Generator]
public class ForAssemblyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(ForAssemblyAttribute).FullName!,
            (c, _) => true,
            (c, _) => c
        );
        context.RegisterImplementationSourceOutput(provider.Collect(), (source, tuple) =>
        {
            if (tuple.Length > 0)
            {
                //Debugger.Launch();
            }
        });
    }
}