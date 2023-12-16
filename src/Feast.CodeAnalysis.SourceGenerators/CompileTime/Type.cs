using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

#nullable enable
namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.Type")]
internal partial class Type(global::Microsoft.CodeAnalysis.ITypeSymbol type) 
    : global::System.Type, global::System.IEquatable<global::System.Type>
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

    public override global::System.Collections.Generic.IEnumerable<global::System.Reflection.CustomAttributeData>
        CustomAttributes => type.GetAttributes()
        .Select(x => new global::Feast.CodeAnalysis.CompileTime.AttributeData(x));

    
    public override string Namespace => type.ContainingNamespace.ToDisplayString();
    public override string Name      => type.MetadataName;

    public override string FullName =>
        $"{Namespace}.{Name}{(!IsGenericType
            ? string.Empty
            : '[' + string.Join(",", GenericTypeArguments.Select(x => $"[{x.AssemblyQualifiedName}]")) + ']')}";

    public override string AssemblyQualifiedName => $"{FullName}, {Assembly.FullName}";
        
    public override global::System.Guid GUID => throw new global::System.NotSupportedException();

    public override global::System.Reflection.MemberTypes MemberType => type.ContainingType is not null
        ? global::System.Reflection.MemberTypes.NestedType
        : global::System.Reflection.MemberTypes.TypeInfo;
    
    public override global::System.Type? BaseType =>
        type.BaseType == null
            ? null
            : new global::Feast.CodeAnalysis.CompileTime.Type(type.BaseType);
        
    public override global::System.Type? ReflectedType =>
        type.ContainingType is null
            ? null
            : new global::Feast.CodeAnalysis.CompileTime.Type(type.ContainingType);
    
    public override global::System.Type? DeclaringType =>
        type.ContainingType is null
            ? null
            : new global::Feast.CodeAnalysis.CompileTime.Type(type.ContainingType);

    public override global::System.Reflection.MethodBase? DeclaringMethod =>
        type.ContainingSymbol is not global::Microsoft.CodeAnalysis.IMethodSymbol methodSymbol
            ? null
            : new global::Feast.CodeAnalysis.CompileTime.MethodInfo(methodSymbol);

    public override bool IsEnum => type.TypeKind == global::Microsoft.CodeAnalysis.TypeKind.Enum;
    
    public override global::System.Reflection.Assembly Assembly =>
        new global::Feast.CodeAnalysis.CompileTime.Assembly(type.ContainingAssembly);

    public override global::System.Reflection.Module Module =>
        new global::Feast.CodeAnalysis.CompileTime.Module(type.ContainingModule);

    public override global::System.Type UnderlyingSystemType =>
        new global::Feast.CodeAnalysis.CompileTime.Type(type);


    public override bool IsGenericType => type is global::Microsoft.CodeAnalysis.INamedTypeSymbol
    {
        TypeArguments.Length: > 0
    };

    public override bool ContainsGenericParameters => type is global::Microsoft.CodeAnalysis.INamedTypeSymbol
    {
        TypeParameters.Length: > 0
    };

    public override bool IsGenericParameter =>
        type.TypeKind == global::Microsoft.CodeAnalysis.TypeKind.TypeParameter;

    public override bool IsGenericTypeDefinition =>
        type is global::Microsoft.CodeAnalysis.INamedTypeSymbol namedType &&
        namedType.TypeParameters.Length > namedType.TypeArguments.Length;

    public override bool IsConstructedGenericType =>
        type is global::Microsoft.CodeAnalysis.INamedTypeSymbol namedType &&
        namedType.TypeParameters.Length == namedType.TypeArguments.Length;

    public override System.Type[] GenericTypeArguments =>
        type is global::Microsoft.CodeAnalysis.INamedTypeSymbol { TypeArguments.Length: > 0 } typeSymbol
            ? typeSymbol.TypeArguments
                .Select(static x => (global::System.Type)new Type(x))
                .ToArray()
            : Array.Empty<System.Type>();

    public override System.Type[] GetGenericParameterConstraints()
    {
        if (type is not global::Microsoft.CodeAnalysis.ITypeParameterSymbol typeParameterSymbol)
            return global::System.Array.Empty<System.Type>();
        return typeParameterSymbol.ConstraintTypes
            .Select(static x => (global::System.Type)new Type(x))
            .ToArray();
    }

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
            type is
            {
                DeclaredAccessibility: Microsoft.CodeAnalysis.Accessibility.Private, ContainingType: not null
            })
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

    protected override global::System.Reflection.ConstructorInfo? GetConstructorImpl(
        global::System.Reflection.BindingFlags bindingAttr,
        global::System.Reflection.Binder binder,
        global::System.Reflection.CallingConventions callConvention,
        global::System.Type[] types,
        global::System.Reflection.ParameterModifier[] modifiers)
    {
        var ret = type.GetMembers()
            .OfType<global::Microsoft.CodeAnalysis.IMethodSymbol>()
            .FirstOrDefault(x =>
                Qualified(x, bindingAttr) &&
                !types.Where((c, i) => new global::Feast.CodeAnalysis.CompileTime.Type(x.Parameters[i].Type).Equals(c))
                    .Any());
        return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.ConstructorInfo(ret);
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
            .FirstOrDefault(x => x.Name == name && Qualified(x, bindingAttr));
        return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.EventInfo(ret);
    }

    public override global::System.Reflection.EventInfo[] GetEvents(
        global::System.Reflection.BindingFlags bindingAttr)
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

    public override global::System.Reflection.FieldInfo[] GetFields(
        global::System.Reflection.BindingFlags bindingAttr) =>
        type.GetMembers()
            .OfType<global::Microsoft.CodeAnalysis.IFieldSymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.FieldInfo)new global::Feast.CodeAnalysis.CompileTime.FieldInfo(x))
            .ToArray();

    private static bool Qualified(global::Microsoft.CodeAnalysis.ISymbol symbol,
        global::System.Reflection.BindingFlags flags) =>
        (!flags.HasFlag(global::System.Reflection.BindingFlags.Instance) || !symbol.IsStatic) &&
        (!flags.HasFlag(global::System.Reflection.BindingFlags.Static)   || symbol.IsStatic)  &&
        (!flags.HasFlag(global::System.Reflection.BindingFlags.Public) ||
         symbol.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public) &&
        (!flags.HasFlag(global::System.Reflection.BindingFlags.NonPublic) ||
         symbol.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public);

    private static bool Qualified(global::Microsoft.CodeAnalysis.ISymbol symbol,
        global::System.Reflection.MemberTypes memberTypes) =>
        memberTypes is MemberTypes.All || symbol switch
        {
            global::Microsoft.CodeAnalysis.IFieldSymbol =>
                memberTypes.HasFlag(global::System.Reflection.MemberTypes.Field),
            global::Microsoft.CodeAnalysis.IMethodSymbol method =>
                method.MethodKind == MethodKind.Constructor
                    ? memberTypes.HasFlag(global::System.Reflection.MemberTypes.Constructor)
                    : memberTypes.HasFlag(global::System.Reflection.MemberTypes.Method),
            global::Microsoft.CodeAnalysis.IPropertySymbol =>
                memberTypes.HasFlag(global::System.Reflection.MemberTypes.Property),
            global::Microsoft.CodeAnalysis.IEventSymbol =>
                memberTypes.HasFlag(global::System.Reflection.MemberTypes.Event),
            global::Microsoft.CodeAnalysis.INamedTypeSymbol =>
                memberTypes.HasFlag(global::System.Reflection.MemberTypes.NestedType),
            _ => false
        };
    
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

    public override System.Reflection.EventInfo[] GetEvents() =>
        type.GetMembers()
            .OfType<global::Microsoft.CodeAnalysis.IEventSymbol>()
            .Select(static x =>
                (global::System.Reflection.EventInfo)
                new global::Feast.CodeAnalysis.CompileTime.EventInfo(x))
            .ToArray();

    public override bool IsSerializable => type.GetAttributes().Any(static x =>
        x.AttributeClass?.ToType().Equals(typeof(global::System.SerializableAttribute)) is true);

    public override System.Reflection.MemberInfo[] GetMember(string name,
        global::System.Reflection.BindingFlags bindingAttr)
        => type.GetMembers()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.MemberInfo)
                new global::Feast.CodeAnalysis.CompileTime.MemberInfo(x))
            .ToArray();

    public override System.Reflection.MemberInfo[] GetMember(string name,
        global::System.Reflection.MemberTypes type,
        global::System.Reflection.BindingFlags bindingAttr)
        => this.type.GetMembers()
            .Where(x => x.Name == name && Qualified(x, bindingAttr) && Qualified(x, type))
            .Select(static x =>
                (global::System.Reflection.MemberInfo)
                new global::Feast.CodeAnalysis.CompileTime.MemberInfo(x))
            .ToArray();
    public override bool IsEnumDefined(object value) =>
        IsEnum && type.GetMembers()
            .OfType<IFieldSymbol>()
            .Any(x => x.ConstantValue == value);

    public override object InvokeMember(string name,
        global::System.Reflection.BindingFlags invokeAttr,
        global::System.Reflection.Binder binder,
        object target,
        object[] args,
        global::System.Reflection.ParameterModifier[] modifiers,
        global::System.Globalization.CultureInfo culture,
        string[] namedParameters) => throw new global::System.NotSupportedException();

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

    protected override global::System.Reflection.PropertyInfo? GetPropertyImpl(string name,
        global::System.Reflection.BindingFlags bindingAttr,
        global::System.Reflection.Binder binder,
        global::System.Type returnType,
        global::System.Type[] types,
        global::System.Reflection.ParameterModifier[] modifiers)
    {
        var ret = type.GetMembers()
            .OfType<global::Microsoft.CodeAnalysis.IPropertySymbol>()
            .FirstOrDefault(x =>
                Qualified(x, bindingAttr) && new Type(x.Type).Equals(returnType));
       return ret == null ? null : new global::Feast.CodeAnalysis.CompileTime.PropertyInfo(ret);
    }

    protected override bool HasElementTypeImpl() => type.IsReferenceType;
    protected override bool IsValueTypeImpl() => type.TypeKind == global::Microsoft.CodeAnalysis.TypeKind.Struct;
        
    public override global::System.Type? GetNestedType(string name,
        global::System.Reflection.BindingFlags bindingAttr)
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

    public override bool Equals(global::System.Type? o)
    {
        switch (o)
        {
            case null:
                return false;
            case global::Feast.CodeAnalysis.CompileTime.Type ct:
                return global::Microsoft.CodeAnalysis.SymbolEqualityComparer.Default.Equals(ct.type, type);
        }

        if (o.FullName != FullName) return false;
        return !IsGenericParameter
               || DeclaringType?.Equals(o.DeclaringType) is true
               || DeclaringMethod?.Equals(o.DeclaringMethod) is true;
    }
    
    public override bool IsAssignableFrom(global::System.Type? c)
    {
        if (c is null) return false;
        if (Equals(c)) return true;
        switch (this)
        {
            case { IsClass: true } when c.IsClass:
                return c.IsSubclassOf(this);
            case { IsInterface: true } when c.IsInterface || c.IsClass:
                return c.GetInterfaces().Any(Equals);
            case { IsGenericParameter: true }:
                return true;
        }
        return false;
    }

    public override string ToString() =>
        $"{Namespace}.{Name}{(!IsGenericType
            ? string.Empty
            : '[' + string.Join(",", GenericTypeArguments.Select(x => x.FullName)) + ']')}";
}