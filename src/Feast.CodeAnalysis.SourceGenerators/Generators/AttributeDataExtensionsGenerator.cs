using System;
using Feast.CodeAnalysis.SourceGenerators.Generators.Base;
using Feast.CodeAnalysis.SourceGenerators.Templates;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.SourceGenerators.Generators;

[Generator]
public class AttributeDataExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(AttributeDataExtensions);
    protected override Type   Type      => typeof(AttributeDataExtensions);
}