using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime
{
    internal partial class Assembly(global::Microsoft.CodeAnalysis.IAssemblySymbol assembly)
        : global::System.Reflection.Assembly
    {
        public override string FullName => assembly.GetFullyQualifiedName();
        public override string Location => assembly.Locations.FirstOrDefault()?.GetLineSpan().Path ?? string.Empty;

        public override global::System.Collections.Generic.IEnumerable<global::System.Reflection.Module> Modules => assembly
            .Modules
            .Select(static x => new global::Feast.CodeAnalysis.CompileTime.Module(x));

        public override global::System.Collections.Generic.IEnumerable<global::System.Type> ExportedTypes => assembly
            .GetForwardedTypes()
            .Where(static x => x.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public)
            .Select(static x => new global::Feast.CodeAnalysis.CompileTime.Type(x));
    }


    internal partial class Assembly
    {
        internal const string Text = """
                                     using System.Linq;
                                     using Microsoft.CodeAnalysis;
                                     #nullable enable
                                     namespace Feast.CodeAnalysis.CompileTime
                                     {
                                         internal partial class Assembly(global::Microsoft.CodeAnalysis.IAssemblySymbol assembly)
                                             : global::System.Reflection.Assembly
                                         {
                                             public override string FullName => assembly.GetFullyQualifiedName();
                                             public override string Location => assembly.Locations.FirstOrDefault()?.GetLineSpan().Path ?? string.Empty;
                                     
                                             public override global::System.Collections.Generic.IEnumerable<global::System.Reflection.Module> Modules => assembly
                                                 .Modules
                                                 .Select(static x => new global::Feast.CodeAnalysis.CompileTime.Module(x));
                                     
                                             public override global::System.Collections.Generic.IEnumerable<global::System.Type> ExportedTypes => assembly
                                                 .GetForwardedTypes()
                                                 .Where(static x => x.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public)
                                                 .Select(static x => new global::Feast.CodeAnalysis.CompileTime.Type(x));
                                         }
                                     }
                                     """;
    }
}