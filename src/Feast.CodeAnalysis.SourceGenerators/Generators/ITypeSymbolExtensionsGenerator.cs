using System;
using Feast.CodeAnalysis.Generators.Base;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Generators;

[Generator]
public class ITypeSymbolExtensionsGenerator : AutoTextGenerator
{
    protected override string ClassName => nameof(ITypeSymbolExtensions);
    protected override Type   Type      => typeof(ITypeSymbolExtensions);

}