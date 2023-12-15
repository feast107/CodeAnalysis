using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class SyntaxExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(SyntaxExtensions);
    protected override Type   Type      => typeof(SyntaxExtensions);
}