#pragma warning disable
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

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
    
    public static Exception? Exception { get; private set; }

    public static           IReadOnlyList<string> Detail => detail;
    private static readonly List<string>          detail = [];
    
    [ModuleInitializer]
    public static void Initialize()
    {
        try
        {
            //Check CSC5
            var          assemblies = global::System.AppDomain.CurrentDomain.GetAssemblies();
            const string fileName   = "Microsoft.CodeAnalysis.Scripting";
            var          scripting  = assemblies.FirstOrDefault(static x => x.GetName().Name == fileName);
            if (scripting != null)
            {
                detail.Add(scripting.ToString());
                return;
            }
            var ms = assemblies.FirstOrDefault(x => x.GetName().Name == "Microsoft.CodeAnalysis");
            if (ms?.CustomAttributes
                    .FirstOrDefault(x => x.AttributeType == typeof(global::System.Reflection.AssemblyFileVersionAttribute))?
                    .ConstructorArguments
                    .FirstOrDefault()
                    .Value is string v && global::System.Version.TryParse(v, out var version) && version.Major >= 5)
            {
                var csc  = Assembly.GetExecutingAssembly().Location;
                var dir = Path.GetDirectoryName(csc) ?? Path.GetDirectoryName(Path.GetTempFileName())!;
                var file = new FileInfo(Path.Combine(dir, fileName + ".dll"));
                if (file.Exists) return;
                using (var stream = file.Create())
                {
                    stream.WriteAsync(Scripting_5_0_0_2_Final, 0, Scripting_5_0_0_2_Final.Length).Wait();
                }
                Assembly.UnsafeLoadFrom(file.FullName);
            }
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }
}

partial class CSharpScriptInitializer
{
    public static byte[] Scripting_5_0_0_2_Final;
}