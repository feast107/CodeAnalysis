using System;
using Feast.CodeAnalysis.SourceGenerators.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class SyntaxExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(SyntaxExtensions);
    protected override Type   Type      => typeof(SyntaxExtensions);
}