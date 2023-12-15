using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime
{
    internal partial class Module(global::Microsoft.CodeAnalysis.IModuleSymbol module) : global::System.Reflection.Module
    {
        public override string Name               => module.MetadataName;
        public override string FullyQualifiedName => module.GetFullyQualifiedName();

        public override global::System.Reflection.Assembly Assembly =>
            new global::Feast.CodeAnalysis.CompileTime.Assembly(module.ContainingAssembly);
    }

    partial class Module
    {
        internal const string Text = """
                                     using Microsoft.CodeAnalysis;
                                     #nullable enable
                                     namespace Feast.CodeAnalysis.CompileTime
                                     {
                                         internal partial class Module(global::Microsoft.CodeAnalysis.IModuleSymbol module) : global::System.Reflection.Module
                                         {
                                             public override string Name               => module.MetadataName;
                                             public override string FullyQualifiedName => module.GetFullyQualifiedName();
                                     
                                             public override global::System.Reflection.Assembly Assembly =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Assembly(module.ContainingAssembly);
                                         }
                                     }
                                     """;
    }
}