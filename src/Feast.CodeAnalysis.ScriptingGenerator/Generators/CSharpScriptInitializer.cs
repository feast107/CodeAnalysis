using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Feast.CodeAnalysis.Scripting.Generators;

file class ModuleInitializerAttribute : Attribute;

[Literal("Feast.CodeAnalysis.Scripting.CSharpScriptInitializer")]
internal static partial class CSharpScriptInitializer
{
    private static readonly MethodInfo Load = typeof(global:: System.Reflection.Assembly).GetMethods(BindingFlags.Static | BindingFlags.Public)
        .FirstOrDefault(x=>
        {
            if (x.Name != nameof(global:: System.Reflection.Assembly.Load)) return false;
            var parameters = x.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(byte[]);
        })!;
    
    [ModuleInitializer]
    public static void Initialize()
    {
        //Check CSC5
        var          assemblies = global::System.AppDomain.CurrentDomain.GetAssemblies();
        const string fileName   = "Microsoft.CodeAnalysis.Scripting";
        if (assemblies.Any(x => x.GetName().Name == fileName))
            return;
        var ms         = assemblies.FirstOrDefault(x => x.GetName().Name == "Microsoft.CodeAnalysis");
        if (ms?.CustomAttributes
                .FirstOrDefault(x => x.AttributeType == typeof(global::System.Reflection.AssemblyFileVersionAttribute))?
                .ConstructorArguments
                .FirstOrDefault()
                .Value is string v && global::System.Version.TryParse(v, out var version) && version.Major >= 5)
        {
            var file = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, fileName + ".dll"));
            if (file.Exists) return;
            using (var stream = file.Create())
            {
                stream.WriteAsync(Scripting_5_0_0_2_Final, 0, Scripting_5_0_0_2_Final.Length).Wait();
            }
            try
            {
                Assembly.UnsafeLoadFrom(file.FullName);
                //if (Load.Invoke(null, parameters: new object[] { Scripting_4_0_0 }) is not Assembly)
            }
            catch (Exception e)
            {
                Debugger.Launch();
                Debugger.Break();
            }
        }
    }
}

partial class CSharpScriptInitializer
{
    public static byte[] Scripting_5_0_0_2_Final;
}