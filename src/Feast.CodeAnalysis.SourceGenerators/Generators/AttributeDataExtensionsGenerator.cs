using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class AttributeDataExtensionsGenerator : AutoTextGenerator
{
    protected override Type[] Types => [typeof(AttributeDataExtensions)];
}