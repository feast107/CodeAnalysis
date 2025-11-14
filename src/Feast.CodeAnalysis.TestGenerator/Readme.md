## .NET10 SDK 类型引用不对称问题
+ 编译入口程序集 
  + VBCSCompiler
  + Version=5.0.0.0 
  + PublicKeyToken=31bf3856ad364e35
+ 异常 `System.TypeInitializationException`
  + 触发者 `Microsoft.CodeAnalysis.Scripting.Hosting.RuntimeMetadataReferenceResolver`
  + 所属程序集 `Microsoft.CodeAnalysis.Scripting`
  + 行 114
  + 内部异常 `System.MissingFieldException`
    + 触发者 `Roslyn.Utilities.PathUtilities.DirectorySeparatorChar`
    + 所属程序集 `Microsoft.CodeAnalysis.Workspaces`
    + 行 28