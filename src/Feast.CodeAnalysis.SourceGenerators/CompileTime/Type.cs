using System.Linq;
using Microsoft.CodeAnalysis;

namespace Feast.CodeAnalysis.CompileTime
{
    [global::System.Diagnostics.DebuggerDisplay("{FullName}")]
    internal partial class Type(global::Microsoft.CodeAnalysis.ITypeSymbol type) : global::System.Type
    {
        private readonly global::Microsoft.CodeAnalysis.ITypeSymbol type = type;
    
        public override object[] GetCustomAttributes(bool inherit) => type
            .GetAttributes()
            .CastArray<object>()
            .ToArray();

        public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
            type.GetAttributes()
                .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
                .Cast<object>()
                .ToArray();

        public override bool IsDefined(global::System.Type attributeType, bool inherit) => type
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

        public override global::System.Reflection.Module Module =>
            new Module(type.ContainingModule);

        public override string Namespace => type.ContainingNamespace.ToDisplayString();
        public override string Name      => type.MetadataName;

        protected override global::System.Reflection.TypeAttributes GetAttributeFlagsImpl()
        {
            var ret = type.TypeKind switch
            {
                global::Microsoft.CodeAnalysis.TypeKind.Interface => global::System.Reflection.TypeAttributes.Interface,
                global::Microsoft.CodeAnalysis.TypeKind.Class     => global::System.Reflection.TypeAttributes.Class,
                _                                                 => global::System.Reflection.TypeAttributes.NotPublic
            };
            if (type.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public)
            {
                ret |= global::System.Reflection.TypeAttributes.Public;
                if (type.ContainingType != null)
                {
                    ret |= global::System.Reflection.TypeAttributes.NestedPublic;
                }
            }
            else if (
                type is { DeclaredAccessibility: Microsoft.CodeAnalysis.Accessibility.Private, ContainingType: not null })
            {
                ret |= global::System.Reflection.TypeAttributes.NestedPrivate;
            }

            if (type.IsAbstract)
            {
                ret |= global::System.Reflection.TypeAttributes.Abstract;
            }

            if (type.IsSealed)
            {
                ret |= global::System.Reflection.TypeAttributes.Sealed;
            }

            return ret;
        }

        protected override global::System.Reflection.ConstructorInfo GetConstructorImpl(
            global::System.Reflection.BindingFlags bindingAttr,
            global::System.Reflection.Binder binder,
            global::System.Reflection.CallingConventions callConvention,
            global::System.Type[] types,
            global::System.Reflection.ParameterModifier[] modifiers)
        {
            throw new global::System.NotImplementedException();
        }

        public override global::System.Reflection.ConstructorInfo[] GetConstructors(
            global::System.Reflection.BindingFlags bindingAttr)
        {
            switch (type.TypeKind)
            {
                case global::Microsoft.CodeAnalysis.TypeKind.Class:
                case global::Microsoft.CodeAnalysis.TypeKind.Array:
                case global::Microsoft.CodeAnalysis.TypeKind.Delegate:
                case global::Microsoft.CodeAnalysis.TypeKind.Struct:
                    return (type as global::Microsoft.CodeAnalysis.INamedTypeSymbol)!.Constructors
                        .Select(static x =>
                            (global::System.Reflection.ConstructorInfo)
                            new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(x))
                        .ToArray();
            }

            return global::System.Array.Empty<global::System.Reflection.ConstructorInfo>();
        }

        public override global::System.Type? GetElementType()
        {
            if (type is not global::Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol) return null;
            if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.TypeParameters.Length != 1) return null;
            return new global::Feast.CodeAnalysis.CompileTime.Type(namedTypeSymbol.TypeParameters[0]);
        }

        public override global::System.Reflection.EventInfo? GetEvent(string name,
            global::System.Reflection.BindingFlags bindingAttr)
        {
            var ret = type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IEventSymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .FirstOrDefault(x => x.Name == name);
            return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.EventInfo(ret);
        }

        public override global::System.Reflection.EventInfo[] GetEvents(global::System.Reflection.BindingFlags bindingAttr)
        {
            return type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IEventSymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .Select(static x =>
                    (global::System.Reflection.EventInfo)new global::Feast.CodeAnalysis.CompileTime.EventInfo(x))
                .ToArray();
        }

        public override global::System.Reflection.FieldInfo? GetField(string name,
            global::System.Reflection.BindingFlags bindingAttr)
        {
            var ret = type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IFieldSymbol>()
                .FirstOrDefault(x => Qualified(x, bindingAttr) && x.Name == name);
            return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.FieldInfo(ret);
        }

        public override global::System.Reflection.FieldInfo[]
            GetFields(global::System.Reflection.BindingFlags bindingAttr) =>
            type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IFieldSymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .Select(static x =>
                    (global::System.Reflection.FieldInfo)new global::Feast.CodeAnalysis.CompileTime.FieldInfo(x))
                .ToArray();

        private static bool Qualified(global::Microsoft.CodeAnalysis.ISymbol symbol,
            global::System.Reflection.BindingFlags flags)
        {
            if (flags.HasFlag(global::System.Reflection.BindingFlags.Instance) && symbol.IsStatic) return false;
            if (flags.HasFlag(global::System.Reflection.BindingFlags.Static)   && !symbol.IsStatic) return false;
            if (flags.HasFlag(global::System.Reflection.BindingFlags.Public) &&
                symbol.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public) return false;
            return !flags.HasFlag(global::System.Reflection.BindingFlags.NonPublic) ||
                   symbol.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public;
        }

        public override global::System.Reflection.MemberInfo[] GetMembers(
            global::System.Reflection.BindingFlags bindingAttr) =>
            type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.ISymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .Select(static x =>
                    (global::System.Reflection.MemberInfo)new global::Feast.CodeAnalysis.CompileTime.MemberInfo(x))
                .ToArray();

        protected override global::System.Reflection.MethodInfo? GetMethodImpl(string name,
            global::System.Reflection.BindingFlags bindingAttr,
            global::System.Reflection.Binder binder,
            global::System.Reflection.CallingConventions callConvention,
            global::System.Type[] types,
            global::System.Reflection.ParameterModifier[] modifiers)
        {
            var ret = type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IMethodSymbol>()
                .FirstOrDefault(x =>
                    x.Name == name && Qualified(x, bindingAttr) && x.Parameters.Length == types.Length
                    && !x.Parameters
                        .Where((p, i) => types[i] != new global::Feast.CodeAnalysis.CompileTime.Type(p.Type))
                        .Any());
            return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.MethodInfo(ret);
        }

        public override global::System.Reflection.MethodInfo[] GetMethods(
            global::System.Reflection.BindingFlags bindingAttr) =>
            type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IMethodSymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .Select(static x =>
                    (global::System.Reflection.MethodInfo)new global::Feast.CodeAnalysis.CompileTime.MethodInfo(x))
                .ToArray();

        public override global::System.Reflection.PropertyInfo[] GetProperties(
            global::System.Reflection.BindingFlags bindingAttr) =>
            type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.IPropertySymbol>()
                .Where(x => Qualified(x, bindingAttr))
                .Select(static x =>
                    (global::System.Reflection.PropertyInfo)
                    new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(x))
                .ToArray();


        public override object InvokeMember(string name,
            global::System.Reflection.BindingFlags invokeAttr,
            global::System.Reflection.Binder binder,
            object target,
            object[] args,
            global::System.Reflection.ParameterModifier[] modifiers,
            global::System.Globalization.CultureInfo culture,
            string[] namedParameters) => throw new global::System.NotSupportedException();

        public override global::System.Type UnderlyingSystemType =>
            new global::Feast.CodeAnalysis.CompileTime.Type(type);

        protected override bool IsArrayImpl() =>
            type.SpecialType == global::Microsoft.CodeAnalysis.SpecialType.System_Array;

        protected override bool IsByRefImpl() => type.IsReferenceType || type.IsRefLikeType;

        protected override bool IsCOMObjectImpl() => type is global::Microsoft.CodeAnalysis.INamedTypeSymbol
        {
            IsComImport: true
        };

        protected override bool IsPointerImpl() => type.Kind == global::Microsoft.CodeAnalysis.SymbolKind.PointerType;

        protected override bool IsPrimitiveImpl() =>
            type.SpecialType is >= global::Microsoft.CodeAnalysis.SpecialType.System_Boolean
                and <= global::Microsoft.CodeAnalysis.SpecialType.System_Double
                or global::Microsoft.CodeAnalysis.SpecialType.System_Object
                or global::Microsoft.CodeAnalysis.SpecialType.System_String;

        public override global::System.Reflection.Assembly Assembly =>
            new global::Feast.CodeAnalysis.CompileTime.Assembly(type.ContainingAssembly);

        public override string              FullName => $"{Namespace}.{Name}";
        public override global::System.Guid GUID     => throw new global::System.NotSupportedException();

        public override string AssemblyQualifiedName =>
            type.ToDisplayString(global::Microsoft.CodeAnalysis.SymbolDisplayFormat.FullyQualifiedFormat);

        public override global::System.Type? BaseType => type.BaseType == null
            ? null
            : new global::Feast.CodeAnalysis.CompileTime.Type(type.BaseType);

        protected override global::System.Reflection.PropertyInfo GetPropertyImpl(string name,
            global::System.Reflection.BindingFlags bindingAttr,
            global::System.Reflection.Binder binder,
            global::System.Type returnType,
            global::System.Type[] types,
            global::System.Reflection.ParameterModifier[] modifiers)
        {
            throw new global::System.NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            throw new global::System.NotImplementedException();
        }

        public override global::System.Type? GetNestedType(string name, global::System.Reflection.BindingFlags bindingAttr)
        {
            var ret = type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.INamedTypeSymbol>()
                .FirstOrDefault(x => x.Name == name && Qualified(x, bindingAttr));
            return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.Type(ret);
        }

        public override global::System.Type[] GetNestedTypes(global::System.Reflection.BindingFlags bindingAttr) =>
            type.GetMembers()
                .OfType<global::Microsoft.CodeAnalysis.INamedTypeSymbol>()
                .Select(x => (global::System.Type)new global::Feast.CodeAnalysis.CompileTime.Type(x))
                .ToArray();

        public override global::System.Type? GetInterface(string name, bool ignoreCase)
        {
            var ret = type.AllInterfaces
                .FirstOrDefault(x => ignoreCase
                    ? string.Equals(name, x.Name, global::System.StringComparison.OrdinalIgnoreCase)
                    : name == x.Name);
            return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.Type(ret);
        }

        public override global::System.Type[] GetInterfaces() =>
            type.AllInterfaces
                .Select(x => (global::System.Type)new global::Feast.CodeAnalysis.CompileTime.Type(x))
                .ToArray();

        public override bool IsAssignableFrom(System.Type c)
        {
            return base.IsAssignableFrom(c);
        }

        public override bool Equals(System.Type o)
        {
            if (o is global::Feast.CodeAnalysis.CompileTime.Type compileTimeType)
            {
                return global::Microsoft.CodeAnalysis.SymbolEqualityComparer.Default.Equals(
                    this.type,
                    compileTimeType.type);
            }

            return false;
        }
    }

    public static class TypeExtension
    {
        public static global::System.Reflection.Assembly ToAssembly(this IAssemblySymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.Assembly(symbol);
    
        public static global::System.Reflection.Module ToModule(this IModuleSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.Module(symbol);
    
        public static global::System.Type ToType(this ITypeSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.Type(symbol);

        public static global::System.Reflection.MemberInfo ToMemberInfo(this ISymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.MemberInfo(symbol);
    
        public static global::System.Reflection.MethodInfo ToMethodInfo(this IMethodSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.MethodInfo(symbol);

        public static global::System.Reflection.FieldInfo ToFieldInfo(this IFieldSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.FieldInfo(symbol);

        public static global::System.Reflection.PropertyInfo ToPropertyInfo(this IPropertySymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(symbol);

        public static global::System.Reflection.ConstructorInfo ToConstructorInfo(this IMethodSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(symbol);
    
        public static global::System.Reflection.EventInfo ToEventInfo(this IEventSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.EventInfo(symbol);
    
        public static global::System.Reflection.ParameterInfo ToParameterInfo(this IParameterSymbol symbol) =>
            new global::Feast.CodeAnalysis.CompileTime.ParameterInfo(symbol);
    }

    partial class Type
    {
        internal const string Text = """
                                     using System.Linq;
                                     using Microsoft.CodeAnalysis;
                                     
                                     namespace Feast.CodeAnalysis.CompileTime
                                     {
                                         [global::System.Diagnostics.DebuggerDisplay("{FullName}")]
                                         internal partial class Type(global::Microsoft.CodeAnalysis.ITypeSymbol type) : global::System.Type
                                         {
                                             private readonly global::Microsoft.CodeAnalysis.ITypeSymbol type = type;
                                         
                                             public override object[] GetCustomAttributes(bool inherit) => type
                                                 .GetAttributes()
                                                 .CastArray<object>()
                                                 .ToArray();
                                     
                                             public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
                                                 type.GetAttributes()
                                                     .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
                                                     .Cast<object>()
                                                     .ToArray();
                                     
                                             public override bool IsDefined(global::System.Type attributeType, bool inherit) => type
                                                 .GetAttributes()
                                                 .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);
                                     
                                             public override global::System.Reflection.Module Module =>
                                                 new Module(type.ContainingModule);
                                     
                                             public override string Namespace => type.ContainingNamespace.ToDisplayString();
                                             public override string Name      => type.MetadataName;
                                     
                                             protected override global::System.Reflection.TypeAttributes GetAttributeFlagsImpl()
                                             {
                                                 var ret = type.TypeKind switch
                                                 {
                                                     global::Microsoft.CodeAnalysis.TypeKind.Interface => global::System.Reflection.TypeAttributes.Interface,
                                                     global::Microsoft.CodeAnalysis.TypeKind.Class     => global::System.Reflection.TypeAttributes.Class,
                                                     _                                                 => global::System.Reflection.TypeAttributes.NotPublic
                                                 };
                                                 if (type.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public)
                                                 {
                                                     ret |= global::System.Reflection.TypeAttributes.Public;
                                                     if (type.ContainingType != null)
                                                     {
                                                         ret |= global::System.Reflection.TypeAttributes.NestedPublic;
                                                     }
                                                 }
                                                 else if (
                                                     type is { DeclaredAccessibility: Microsoft.CodeAnalysis.Accessibility.Private, ContainingType: not null })
                                                 {
                                                     ret |= global::System.Reflection.TypeAttributes.NestedPrivate;
                                                 }
                                     
                                                 if (type.IsAbstract)
                                                 {
                                                     ret |= global::System.Reflection.TypeAttributes.Abstract;
                                                 }
                                     
                                                 if (type.IsSealed)
                                                 {
                                                     ret |= global::System.Reflection.TypeAttributes.Sealed;
                                                 }
                                     
                                                 return ret;
                                             }
                                     
                                             protected override global::System.Reflection.ConstructorInfo GetConstructorImpl(
                                                 global::System.Reflection.BindingFlags bindingAttr,
                                                 global::System.Reflection.Binder binder,
                                                 global::System.Reflection.CallingConventions callConvention,
                                                 global::System.Type[] types,
                                                 global::System.Reflection.ParameterModifier[] modifiers)
                                             {
                                                 throw new global::System.NotImplementedException();
                                             }
                                     
                                             public override global::System.Reflection.ConstructorInfo[] GetConstructors(
                                                 global::System.Reflection.BindingFlags bindingAttr)
                                             {
                                                 switch (type.TypeKind)
                                                 {
                                                     case global::Microsoft.CodeAnalysis.TypeKind.Class:
                                                     case global::Microsoft.CodeAnalysis.TypeKind.Array:
                                                     case global::Microsoft.CodeAnalysis.TypeKind.Delegate:
                                                     case global::Microsoft.CodeAnalysis.TypeKind.Struct:
                                                         return (type as global::Microsoft.CodeAnalysis.INamedTypeSymbol)!.Constructors
                                                             .Select(static x =>
                                                                 (global::System.Reflection.ConstructorInfo)
                                                                 new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(x))
                                                             .ToArray();
                                                 }
                                     
                                                 return global::System.Array.Empty<global::System.Reflection.ConstructorInfo>();
                                             }
                                     
                                             public override global::System.Type? GetElementType()
                                             {
                                                 if (type is not global::Microsoft.CodeAnalysis.INamedTypeSymbol namedTypeSymbol) return null;
                                                 if (!namedTypeSymbol.IsGenericType || namedTypeSymbol.TypeParameters.Length != 1) return null;
                                                 return new global::Feast.CodeAnalysis.CompileTime.Type(namedTypeSymbol.TypeParameters[0]);
                                             }
                                     
                                             public override global::System.Reflection.EventInfo? GetEvent(string name,
                                                 global::System.Reflection.BindingFlags bindingAttr)
                                             {
                                                 var ret = type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IEventSymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .FirstOrDefault(x => x.Name == name);
                                                 return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.EventInfo(ret);
                                             }
                                     
                                             public override global::System.Reflection.EventInfo[] GetEvents(global::System.Reflection.BindingFlags bindingAttr)
                                             {
                                                 return type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IEventSymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .Select(static x =>
                                                         (global::System.Reflection.EventInfo)new global::Feast.CodeAnalysis.CompileTime.EventInfo(x))
                                                     .ToArray();
                                             }
                                     
                                             public override global::System.Reflection.FieldInfo? GetField(string name,
                                                 global::System.Reflection.BindingFlags bindingAttr)
                                             {
                                                 var ret = type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IFieldSymbol>()
                                                     .FirstOrDefault(x => Qualified(x, bindingAttr) && x.Name == name);
                                                 return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.FieldInfo(ret);
                                             }
                                     
                                             public override global::System.Reflection.FieldInfo[]
                                                 GetFields(global::System.Reflection.BindingFlags bindingAttr) =>
                                                 type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IFieldSymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .Select(static x =>
                                                         (global::System.Reflection.FieldInfo)new global::Feast.CodeAnalysis.CompileTime.FieldInfo(x))
                                                     .ToArray();
                                     
                                             private static bool Qualified(global::Microsoft.CodeAnalysis.ISymbol symbol,
                                                 global::System.Reflection.BindingFlags flags)
                                             {
                                                 if (flags.HasFlag(global::System.Reflection.BindingFlags.Instance) && symbol.IsStatic) return false;
                                                 if (flags.HasFlag(global::System.Reflection.BindingFlags.Static)   && !symbol.IsStatic) return false;
                                                 if (flags.HasFlag(global::System.Reflection.BindingFlags.Public) &&
                                                     symbol.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public) return false;
                                                 return !flags.HasFlag(global::System.Reflection.BindingFlags.NonPublic) ||
                                                        symbol.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public;
                                             }
                                     
                                             public override global::System.Reflection.MemberInfo[] GetMembers(
                                                 global::System.Reflection.BindingFlags bindingAttr) =>
                                                 type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.ISymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .Select(static x =>
                                                         (global::System.Reflection.MemberInfo)new global::Feast.CodeAnalysis.CompileTime.MemberInfo(x))
                                                     .ToArray();
                                     
                                             protected override global::System.Reflection.MethodInfo? GetMethodImpl(string name,
                                                 global::System.Reflection.BindingFlags bindingAttr,
                                                 global::System.Reflection.Binder binder,
                                                 global::System.Reflection.CallingConventions callConvention,
                                                 global::System.Type[] types,
                                                 global::System.Reflection.ParameterModifier[] modifiers)
                                             {
                                                 var ret = type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IMethodSymbol>()
                                                     .FirstOrDefault(x =>
                                                         x.Name == name && Qualified(x, bindingAttr) && x.Parameters.Length == types.Length
                                                         && !x.Parameters
                                                             .Where((p, i) => types[i] != new global::Feast.CodeAnalysis.CompileTime.Type(p.Type))
                                                             .Any());
                                                 return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.MethodInfo(ret);
                                             }
                                     
                                             public override global::System.Reflection.MethodInfo[] GetMethods(
                                                 global::System.Reflection.BindingFlags bindingAttr) =>
                                                 type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IMethodSymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .Select(static x =>
                                                         (global::System.Reflection.MethodInfo)new global::Feast.CodeAnalysis.CompileTime.MethodInfo(x))
                                                     .ToArray();
                                     
                                             public override global::System.Reflection.PropertyInfo[] GetProperties(
                                                 global::System.Reflection.BindingFlags bindingAttr) =>
                                                 type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.IPropertySymbol>()
                                                     .Where(x => Qualified(x, bindingAttr))
                                                     .Select(static x =>
                                                         (global::System.Reflection.PropertyInfo)
                                                         new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(x))
                                                     .ToArray();
                                     
                                     
                                             public override object InvokeMember(string name,
                                                 global::System.Reflection.BindingFlags invokeAttr,
                                                 global::System.Reflection.Binder binder,
                                                 object target,
                                                 object[] args,
                                                 global::System.Reflection.ParameterModifier[] modifiers,
                                                 global::System.Globalization.CultureInfo culture,
                                                 string[] namedParameters) => throw new global::System.NotSupportedException();
                                     
                                             public override global::System.Type UnderlyingSystemType =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Type(type);
                                     
                                             protected override bool IsArrayImpl() =>
                                                 type.SpecialType == global::Microsoft.CodeAnalysis.SpecialType.System_Array;
                                     
                                             protected override bool IsByRefImpl() => type.IsReferenceType || type.IsRefLikeType;
                                     
                                             protected override bool IsCOMObjectImpl() => type is global::Microsoft.CodeAnalysis.INamedTypeSymbol
                                             {
                                                 IsComImport: true
                                             };
                                     
                                             protected override bool IsPointerImpl() => type.Kind == global::Microsoft.CodeAnalysis.SymbolKind.PointerType;
                                     
                                             protected override bool IsPrimitiveImpl() =>
                                                 type.SpecialType is >= global::Microsoft.CodeAnalysis.SpecialType.System_Boolean
                                                     and <= global::Microsoft.CodeAnalysis.SpecialType.System_Double
                                                     or global::Microsoft.CodeAnalysis.SpecialType.System_Object
                                                     or global::Microsoft.CodeAnalysis.SpecialType.System_String;
                                     
                                             public override global::System.Reflection.Assembly Assembly =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Assembly(type.ContainingAssembly);
                                     
                                             public override string              FullName => $"{Namespace}.{Name}";
                                             public override global::System.Guid GUID     => throw new global::System.NotSupportedException();
                                     
                                             public override string AssemblyQualifiedName =>
                                                 type.ToDisplayString(global::Microsoft.CodeAnalysis.SymbolDisplayFormat.FullyQualifiedFormat);
                                     
                                             public override global::System.Type? BaseType => type.BaseType == null
                                                 ? null
                                                 : new global::Feast.CodeAnalysis.CompileTime.Type(type.BaseType);
                                     
                                             protected override global::System.Reflection.PropertyInfo GetPropertyImpl(string name,
                                                 global::System.Reflection.BindingFlags bindingAttr,
                                                 global::System.Reflection.Binder binder,
                                                 global::System.Type returnType,
                                                 global::System.Type[] types,
                                                 global::System.Reflection.ParameterModifier[] modifiers)
                                             {
                                                 throw new global::System.NotImplementedException();
                                             }
                                     
                                             protected override bool HasElementTypeImpl()
                                             {
                                                 throw new global::System.NotImplementedException();
                                             }
                                     
                                             public override global::System.Type? GetNestedType(string name, global::System.Reflection.BindingFlags bindingAttr)
                                             {
                                                 var ret = type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.INamedTypeSymbol>()
                                                     .FirstOrDefault(x => x.Name == name && Qualified(x, bindingAttr));
                                                 return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.Type(ret);
                                             }
                                     
                                             public override global::System.Type[] GetNestedTypes(global::System.Reflection.BindingFlags bindingAttr) =>
                                                 type.GetMembers()
                                                     .OfType<global::Microsoft.CodeAnalysis.INamedTypeSymbol>()
                                                     .Select(x => (global::System.Type)new global::Feast.CodeAnalysis.CompileTime.Type(x))
                                                     .ToArray();
                                     
                                             public override global::System.Type? GetInterface(string name, bool ignoreCase)
                                             {
                                                 var ret = type.AllInterfaces
                                                     .FirstOrDefault(x => ignoreCase
                                                         ? string.Equals(name, x.Name, global::System.StringComparison.OrdinalIgnoreCase)
                                                         : name == x.Name);
                                                 return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.Type(ret);
                                             }
                                     
                                             public override global::System.Type[] GetInterfaces() =>
                                                 type.AllInterfaces
                                                     .Select(x => (global::System.Type)new global::Feast.CodeAnalysis.CompileTime.Type(x))
                                                     .ToArray();
                                     
                                             public override bool IsAssignableFrom(System.Type c)
                                             {
                                                 return base.IsAssignableFrom(c);
                                             }
                                     
                                             public override bool Equals(System.Type o)
                                             {
                                                 if (o is global::Feast.CodeAnalysis.CompileTime.Type compileTimeType)
                                                 {
                                                     return global::Microsoft.CodeAnalysis.SymbolEqualityComparer.Default.Equals(
                                                         this.type,
                                                         compileTimeType.type);
                                                 }
                                     
                                                 return false;
                                             }
                                         }
                                     
                                         public static class TypeExtension
                                         {
                                             public static global::System.Reflection.Assembly ToAssembly(this IAssemblySymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Assembly(symbol);
                                         
                                             public static global::System.Reflection.Module ToModule(this IModuleSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Module(symbol);
                                         
                                             public static global::System.Type ToType(this ITypeSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.Type(symbol);
                                     
                                             public static global::System.Reflection.MemberInfo ToMemberInfo(this ISymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.MemberInfo(symbol);
                                         
                                             public static global::System.Reflection.MethodInfo ToMethodInfo(this IMethodSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.MethodInfo(symbol);
                                     
                                             public static global::System.Reflection.FieldInfo ToFieldInfo(this IFieldSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.FieldInfo(symbol);
                                     
                                             public static global::System.Reflection.PropertyInfo ToPropertyInfo(this IPropertySymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(symbol);
                                     
                                             public static global::System.Reflection.ConstructorInfo ToConstructorInfo(this IMethodSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(symbol);
                                         
                                             public static global::System.Reflection.EventInfo ToEventInfo(this IEventSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.EventInfo(symbol);
                                         
                                             public static global::System.Reflection.ParameterInfo ToParameterInfo(this IParameterSymbol symbol) =>
                                                 new global::Feast.CodeAnalysis.CompileTime.ParameterInfo(symbol);
                                         }
                                     }
                                     """;
    }
}