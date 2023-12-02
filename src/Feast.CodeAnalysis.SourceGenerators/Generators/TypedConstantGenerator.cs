using System;
using Feast.CodeAnalysis.SourceGenerators.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class TypedConstantGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(TypedConstantExtensions);
    protected override Type   Type      => typeof(TypedConstantExtensions);
}