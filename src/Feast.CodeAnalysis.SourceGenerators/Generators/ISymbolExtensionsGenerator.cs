using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class ISymbolExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(ISymbolExtensions);
    protected override Type   Type      => typeof(ISymbolExtensions);
}