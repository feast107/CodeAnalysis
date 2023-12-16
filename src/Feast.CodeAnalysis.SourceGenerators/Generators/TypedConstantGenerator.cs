using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class TypedConstantGenerator : AutoTextGenerator
{
    protected override Type[] Types => [typeof(TypedConstantExtensions)];
}