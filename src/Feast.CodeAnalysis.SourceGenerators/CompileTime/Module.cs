﻿using System;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.Module")]
internal partial class Module(global::Microsoft.CodeAnalysis.IModuleSymbol module) : global::System.Reflection.Module
{
    internal IModuleSymbol Symbol => module;
    
    public override string        Name               => module.MetadataName;
    public override string        FullyQualifiedName => module.GetFullyQualifiedName();

    public override global::System.Reflection.Assembly Assembly => new Assembly(module.ContainingAssembly);
}