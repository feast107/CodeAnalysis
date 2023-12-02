using System;
using Feast.CodeAnalysis.SourceGenerators.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class ITypeSymbolExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(ITypeSymbolExtensions);
    protected override Type   Type      => typeof(ITypeSymbolExtensions);

}