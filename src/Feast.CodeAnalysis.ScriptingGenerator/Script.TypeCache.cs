using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.TypeCache.Script")]
partial class Script
{
     internal static readonly Lazy<MethodInfo> Initialize = new(() =>
     {
          var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
               .GetType("Roslyn.Utilities.InterlockedOperations")
               .GetMethods(BindingFlags.Public | BindingFlags.Static)
               .FirstOrDefault(x => x.Name == nameof(Initialize) 
                                    && x.GetParameters().Length == 2
                                    && x.ReturnType.Name.StartsWith(nameof(ImmutableArray)))
               ?.MakeGenericMethod(typeof(Func<object[], Task>));
          
          if (method == null)
          {
               throw new InvalidOperationException(
                    $"Method {nameof(Initialize)} not found in CommonReferenceManager class.");
          }

          return method;
     });

     [field: AllowNull, MaybeNull]
     internal static Func<object, object, object> WithRecursiveAliases
     {
          get
          {
               if (field is not null) return field;
               var method = typeof(MetadataReferenceProperties)
                    .GetMethod(nameof(WithRecursiveAliases), BindingFlags.NonPublic | BindingFlags.Instance);
               if (method == null)
               {
                    throw new InvalidOperationException(
                         $"Method {nameof(WithRecursiveAliases)} not found in {nameof(MetadataReferenceProperties)} class.");
               }

               return field = (arg0, arg1) => method.Invoke(arg0, [arg1]);
          }
     }
     
     internal static readonly Lazy<Func<object,object>> CreateFromAssemblyInternal = new(() =>
     {
          var method = typeof(MetadataReference)
               .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
               .FirstOrDefault(x => x.Name == nameof(CreateFromAssemblyInternal) && x.GetParameters().Length == 1);
          if (method == null)
          {
               throw new InvalidOperationException($"Method {nameof(CreateFromAssemblyInternal)} not found in {nameof(MetadataReference)} class.");
          }

          return arg => method.Invoke(null, [arg]);
     });
     
     internal static readonly Lazy<Func<object,object>> HasMetadata = new(() =>
     {
          var method = typeof(MetadataReference)
               .GetMethod(nameof(HasMetadata),
                    BindingFlags.NonPublic | BindingFlags.Static);
          if (method == null)
          {
               throw new InvalidOperationException($"Method {nameof(HasMetadata)} not found in {nameof(MetadataReference)} class.");
          }

          return arg => method.Invoke(null, [arg]);
     });

     internal static readonly Lazy<Func<object,object,object>> CreateFromAssemblyInternal3 = new(() =>
     {
          var method = typeof(MetadataReference)
               .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
               .FirstOrDefault(x => x.Name == nameof(CreateFromAssemblyInternal) && x.GetParameters().Length == 3);
          if (method == null)
          {
               throw new InvalidOperationException($"Method {nameof(CreateFromAssemblyInternal)} not found in {nameof(MetadataReference)} class.");
          }

          return (arg1,arg2) => method.Invoke(null, [arg1,arg2,null]);
     });

     internal static readonly Lazy<Func<object, object, object, object, object>> CreateDiagnostic = new(() =>
     {
          var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
               .GetType("Microsoft.CodeAnalysis.CommonMessageProvider")
               .GetMethods()
               .FirstOrDefault(x => x.Name == nameof(CreateDiagnostic) && x.GetParameters().Length == 3);
          if (method == null)
          {
               throw new InvalidOperationException(
                    $"Method {nameof(CreateDiagnostic)} not found in CommonMessageProvider class.");
          }

          return (arg0, arg1, arg2, arg3) => method.Invoke(arg0, [arg1, arg2, (object[]) [arg3]]);
     });
     
     internal static readonly Lazy<Func<object,object>> ERR_MetadataFileNotFound = new(() =>
     {
          var method = Global.GetAssembly("Microsoft.CodeAnalysis")?
               .GetType("Microsoft.CodeAnalysis.CommonMessageProvider")
               .GetProperty(nameof(ERR_MetadataFileNotFound));
          if (method == null)
          {
               throw new InvalidOperationException(
                    $"Method {nameof(CreateDiagnostic)} not found in CommonMessageProvider class.");
          }

          return (arg0) => method.GetValue(arg0);
     });
}