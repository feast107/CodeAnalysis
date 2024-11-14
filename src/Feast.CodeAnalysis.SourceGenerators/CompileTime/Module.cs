using System;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.Module")]
internal partial class Module(global::Microsoft.CodeAnalysis.IModuleSymbol symbol) : global::System.Reflection.Module
{
    internal IModuleSymbol Symbol => symbol;
    
    public override string        Name               => symbol.MetadataName;
    public override string        FullyQualifiedName => symbol.GetFullyQualifiedName();

    public override global::System.Reflection.Assembly Assembly => new Assembly(symbol.ContainingAssembly);
}