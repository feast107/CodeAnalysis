using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class SyntaxExtensionsGenerator : AutoTextGenerator
{
    protected override Type[] Types => [typeof(SyntaxExtensions)];
}