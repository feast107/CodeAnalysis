using System;
using Feast.CodeAnalysis.SourceGenerators.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class ISymbolExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(ISymbolExtensions);
    protected override Type   Type      => typeof(ISymbolExtensions);
}