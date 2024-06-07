using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

#nullable enable
namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.Type")]
internal partial class Type(global::Microsoft.CodeAnalysis.ITypeSymbol symbol) 
    : global::System.Type, IEquatable<global::System.Type>
{
    internal readonly ITypeSymbol Symbol = symbol;
        
    public override object[] GetCustomAttributes(bool inherit) => Symbol
        .GetAttributes()
        .CastArray<object>()
        .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        Symbol.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) => Symbol
        .GetAttributes()
        .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override System.Collections.Generic.IEnumerable<CustomAttributeData>
        CustomAttributes => Symbol.GetAttributes()
        .Select(x => new AttributeData(x));

    
    public override string? Namespace => IsGenericParameter ? null : NotGlobalNamespace(Symbol.ContainingNamespace.ToDisplayString());
    public override string  Name      => Symbol.MetadataName;

    public override string FullName =>
        Symbol.TypeKind switch
        {
            TypeKind.TypeParameter => Name,
            TypeKind.Array         => $"{GetElementType()!.FullName}[]",
            TypeKind.Pointer       => $"{GetElementType()!.FullName}*",
            _ => $"{Namespace}.{Name}{(!IsGenericType
                ? string.Empty
                : '[' + string.Join(",", GenericTypeArguments.Select(x => $"[{x.AssemblyQualifiedName}]")) + ']')}"
        };

    public override string AssemblyQualifiedName => $"{FullName}, {Assembly.FullName}";
        
    public override Guid GUID => throw new NotSupportedException();

    public override MemberTypes MemberType => Symbol.ContainingType is not null
        ? MemberTypes.NestedType
        : MemberTypes.TypeInfo;
    
    public override global::System.Type? BaseType =>
        Symbol.BaseType == null
            ? null
            : new Type(Symbol.BaseType);
        
    public override global::System.Type? ReflectedType =>
        Symbol.ContainingType is null
            ? null
            : new Type(Symbol.ContainingType);
    
    public override global::System.Type? DeclaringType =>
        Symbol.ContainingType is null
            ? null
            : new Type(Symbol.ContainingType);

    public override MethodBase? DeclaringMethod =>
        Symbol.ContainingSymbol is not IMethodSymbol methodSymbol
            ? null
            : new MethodInfo(methodSymbol);

    public override bool IsEnum => Symbol.TypeKind == TypeKind.Enum;
    
    public override global::System.Reflection.Assembly Assembly => new Assembly(Symbol.ContainingAssembly);

    public override global::System.Reflection.Module Module => new Module(Symbol.ContainingModule);

    public override global::System.Type UnderlyingSystemType => new Type(Symbol);
    
    public override bool IsGenericType => Symbol is INamedTypeSymbol
    {
        TypeArguments.Length: > 0
    };

    public override bool ContainsGenericParameters => Symbol is INamedTypeSymbol
    {
        TypeParameters.Length: > 0
    };

    public override bool IsGenericParameter => Symbol.TypeKind == TypeKind.TypeParameter;

    public override bool IsGenericTypeDefinition =>
        Symbol is INamedTypeSymbol namedType &&
        namedType.TypeParameters.Length > namedType.TypeArguments.Length;

    public override bool IsConstructedGenericType =>
        Symbol is INamedTypeSymbol namedType &&
        namedType.TypeArguments.All(x => !x.IsDefinition);

    public override System.Type[] GenericTypeArguments =>
        Symbol is INamedTypeSymbol { TypeArguments.Length: > 0 } typeSymbol
            ? typeSymbol.TypeArguments
                .Select(static x => (global::System.Type)new Type(x))
                .ToArray()
            : [];

    public override GenericParameterAttributes GenericParameterAttributes
    {
        get
        {
            var ret = GenericParameterAttributes.None;
            if (Symbol is not ITypeParameterSymbol parameter) return ret;
            if (parameter.HasReferenceTypeConstraint) ret |= GenericParameterAttributes.ReferenceTypeConstraint;
            if (parameter.HasNotNullConstraint) ret       |= GenericParameterAttributes.NotNullableValueTypeConstraint;
            if (parameter.HasUnmanagedTypeConstraint) ret |= GenericParameterAttributes.NotNullableValueTypeConstraint;
            if (parameter.HasConstructorConstraint) ret   |= GenericParameterAttributes.DefaultConstructorConstraint;
            return ret;
        }
    }

    public override bool IsSerializable =>
        Symbol.GetAttributes()
            .Any(static x => x.AttributeClass?
                .ToType()
                .Equals(typeof(SerializableAttribute)) is true);

    public override System.Type? GetGenericTypeDefinition() =>
        Symbol is INamedTypeSymbol { IsGenericType: true }
            ? new Type(Symbol.OriginalDefinition)
            : null;

    public override System.Type[] GetGenericArguments() =>
        Symbol is INamedTypeSymbol { TypeArguments.Length: > 0 } namedType
            ? namedType.TypeArguments
                .Select(static x => (global::System.Type)new Type(x))
                .ToArray()
            : [];

    public System.Type[] GetGenericParameters() =>
        Symbol is INamedTypeSymbol { TypeParameters.Length: > 0 } namedType
            ? namedType.TypeParameters
                .Select(static x => (global::System.Type)new Type(x))
                .ToArray()
            : [];
    
    public override System.Type[] GetGenericParameterConstraints() =>
        Symbol is not ITypeParameterSymbol typeParameterSymbol
            ? []
            : typeParameterSymbol.ConstraintTypes
                .Select(static x => (global::System.Type)new Type(x))
                .ToArray();

    
    
    protected override TypeAttributes GetAttributeFlagsImpl()
    {
        var ret = Symbol.TypeKind switch
        {
            TypeKind.Interface => TypeAttributes.Interface,
            TypeKind.Class     => TypeAttributes.Class,
            _                  => TypeAttributes.NotPublic
        };
        if (Symbol.DeclaredAccessibility == Accessibility.Public)
        {
            ret |= TypeAttributes.Public;
            if (Symbol.ContainingType != null)
            {
                ret |= TypeAttributes.NestedPublic;
            }
        }
        else if (
            Symbol is
            {
                DeclaredAccessibility: Accessibility.Private, ContainingType: not null
            })
        {
            ret |= TypeAttributes.NestedPrivate;
        }

        if (Symbol.IsAbstract)
        {
            ret |= TypeAttributes.Abstract;
        }

        if (Symbol.IsSealed)
        {
            ret |= TypeAttributes.Sealed;
        }

        return ret;
    }

    protected override global::System.Reflection.ConstructorInfo? GetConstructorImpl(
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callConvention,
        global::System.Type[] types,
        ParameterModifier[] modifiers)
    {
        var ret = Symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(x =>
                Qualified(x, bindingAttr) &&
                !types.Where((c, i) => new Type(x.Parameters[i].Type).Equals(c))
                    .Any());
        return ret == null ? null : new ConstructorInfo(ret);
    }

    public override global::System.Reflection.ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
        switch (Symbol.TypeKind)
        {
            case TypeKind.Class:
            case TypeKind.Array:
            case TypeKind.Delegate:
            case TypeKind.Struct:
                return (Symbol as INamedTypeSymbol)!.Constructors
                    .Select(static x =>
                        (global::System.Reflection.ConstructorInfo)new ConstructorInfo(x))
                    .ToArray();
        }

        return [];
    }


    public override global::System.Reflection.EventInfo? GetEvent(string name,
        BindingFlags bindingAttr)
    {
        var ret = Symbol.GetMembers()
            .OfType<IEventSymbol>()
            .FirstOrDefault(x => Qualified(x, name, bindingAttr));
        return ret == null ? null : new EventInfo(ret);
    }

    public override global::System.Reflection.EventInfo[] GetEvents(
        BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<IEventSymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.EventInfo)new EventInfo(x))
            .ToArray();

    public override global::System.Reflection.FieldInfo? GetField(string name,
        BindingFlags bindingAttr)
    {
        var ret = Symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(x => Qualified(x, name, bindingAttr));
        return ret == null ? null : new FieldInfo(ret);
    }

    public override global::System.Reflection.FieldInfo[] GetFields(
        BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.FieldInfo)new FieldInfo(x))
            .ToArray();
    
    public override global::System.Reflection.MemberInfo[] GetMembers(
        BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<ISymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.MemberInfo)new MemberInfo(x))
            .ToArray();

    protected override global::System.Reflection.MethodInfo? GetMethodImpl(string name,
        BindingFlags bindingAttr,
        Binder binder,
        CallingConventions callConvention,
        global::System.Type[] types,
        ParameterModifier[] modifiers)
    {
        var ret = Symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .FirstOrDefault(x =>
                Qualified(x, name, bindingAttr)
                && x.Parameters.Length == types.Length
                && !x.Parameters
                    .Where((p, i) => types[i] != new Type(p.Type))
                    .Any());
        return ret == null ? null : new MethodInfo(ret);
    }

    public override global::System.Reflection.MethodInfo[] GetMethods(
        BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x =>
                (global::System.Reflection.MethodInfo)new MethodInfo(x))
            .ToArray();

    public override global::System.Reflection.PropertyInfo[] GetProperties(
        BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => Qualified(x, bindingAttr))
            .Select(static x => (global::System.Reflection.PropertyInfo)new PropertyInfo(x))
            .ToArray();
    
    protected override global::System.Reflection.PropertyInfo? GetPropertyImpl(string name,
        BindingFlags bindingAttr,
        Binder binder,
        global::System.Type returnType,
        global::System.Type[] types,
        ParameterModifier[] modifiers)
    {
        var ret = Symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .FirstOrDefault(x =>
                Qualified(x, name, bindingAttr) && new Type(x.Type).Equals(returnType));
        return ret == null ? null : new PropertyInfo(ret);
    }

    public override System.Reflection.EventInfo[] GetEvents() =>
        Symbol.GetMembers()
            .OfType<IEventSymbol>()
            .Select(static x => (global::System.Reflection.EventInfo)new EventInfo(x))
            .ToArray();

    public override System.Reflection.MemberInfo[] GetMember(string name,
        BindingFlags bindingAttr)
        => Symbol.GetMembers()
            .Where(x => Qualified(x, name, bindingAttr))
            .Select(static x => (global::System.Reflection.MemberInfo)new MemberInfo(x))
            .ToArray();

    public override System.Reflection.MemberInfo[] GetMember(string name,
        MemberTypes type,
        BindingFlags bindingAttr)
        => Symbol.GetMembers()
            .Where(x => Qualified(x, name, bindingAttr, type))
            .Select(static x => (global::System.Reflection.MemberInfo)new MemberInfo(x))
            .ToArray();
    
    public override bool IsEnumDefined(object value) =>
        IsEnum && Symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Any(x => x.ConstantValue == value);

    public override object InvokeMember(string name,
        BindingFlags invokeAttr,
        Binder binder,
        object target,
        object[] args,
        ParameterModifier[] modifiers,
        System.Globalization.CultureInfo culture,
        string[] namedParameters) => throw new NotSupportedException();

    protected override bool IsArrayImpl() => Symbol.SpecialType == SpecialType.System_Array;

    protected override bool IsByRefImpl() => Symbol.IsReferenceType || Symbol.IsRefLikeType;

    protected override bool IsCOMObjectImpl() => Symbol is INamedTypeSymbol
    {
        IsComImport: true
    };

    protected override bool IsPointerImpl() => Symbol.Kind == SymbolKind.PointerType;

    protected override bool IsPrimitiveImpl() =>
        Symbol.SpecialType is >= SpecialType.System_Boolean
            and <= SpecialType.System_Double
            or SpecialType.System_Object
            or SpecialType.System_String;

    public override Array GetEnumValues() =>
        IsEnum
            ? Symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Select(x => x.ConstantValue)
                .ToArray()
            : throw new InvalidOperationException();

    protected override bool HasElementTypeImpl() =>
        Symbol.TypeKind is TypeKind.Array or TypeKind.Pointer;

    public override global::System.Type? GetElementType() =>
        Symbol switch
        {
            { TypeKind: TypeKind.Array } => new
                Type(Symbol.Interfaces.First(static x => x.TypeArguments.Length == 1).TypeArguments[0]),
            { TypeKind: TypeKind.Pointer } and IPointerTypeSymbol pointer => new Type(pointer.PointedAtType),
            { IsReferenceType: true }                                     => this,
            _                                                             => null
        };

    protected override bool IsValueTypeImpl() => Symbol.TypeKind is TypeKind.Struct or TypeKind.Structure;
        
    public override global::System.Type? GetNestedType(string name,
        BindingFlags bindingAttr)
    {
        var ret = Symbol.GetMembers()
            .OfType<INamedTypeSymbol>()
            .FirstOrDefault(x => x.Name == name && Qualified(x, bindingAttr));
        return ret == null ? null : new Type(ret);
    }

    public override global::System.Type[] GetNestedTypes(BindingFlags bindingAttr) =>
        Symbol.GetMembers()
            .OfType<INamedTypeSymbol>()
            .Select(x => (global::System.Type)new Type(x))
            .ToArray();

    public override global::System.Type? GetInterface(string name, bool ignoreCase)
    {
        var ret = Symbol.AllInterfaces
            .FirstOrDefault(x => ignoreCase
                ? string.Equals(name, x.Name, StringComparison.OrdinalIgnoreCase)
                : name == x.Name);
        return ret == null ? null : new Type(ret);
    }

    public override global::System.Type[] GetInterfaces() =>
        Symbol.AllInterfaces
            .Select(x => (global::System.Type)new Type(x))
            .ToArray();

    public override bool Equals(global::System.Type? o)
    {
        switch (o)
        {
            case null:
                return false;
            case Type ct:
                return SymbolEqualityComparer.Default.Equals(ct.Symbol, Symbol);
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

    public override string ToString() => FullName;
    
    public static bool operator ==(Type? left, global::System.Type? right) =>
        left?.Equals(right) ?? right is null;
    
    public static bool operator ==(global::System.Type? left, Type? right) =>
        right?.Equals(left) ?? left is null;

    public static bool operator !=(Type? left, global::System.Type? right) =>
        !(left == right);
    
    public static bool operator !=(global::System.Type? left, Type? right) =>
        !(left == right);
    
    private static bool Qualified(ISymbol symbol, BindingFlags flags) =>
        flags.HasFlag(BindingFlags.Instance) && !symbol.IsStatic
        ||
        flags.HasFlag(BindingFlags.Static) && symbol.IsStatic
        ||
        flags.HasFlag(BindingFlags.Public) && symbol.DeclaredAccessibility == Accessibility.Public 
        ||
        flags.HasFlag(BindingFlags.NonPublic) && symbol.DeclaredAccessibility != Accessibility.Public;

    private static bool Qualified(ISymbol symbol, MemberTypes memberTypes) =>
        memberTypes is MemberTypes.All || symbol switch
        {
            IFieldSymbol => memberTypes.HasFlag(MemberTypes.Field),
            IMethodSymbol method => method.MethodKind == MethodKind.Constructor
                ? memberTypes.HasFlag(MemberTypes.Constructor)
                : memberTypes.HasFlag(MemberTypes.Method),
            IPropertySymbol  => memberTypes.HasFlag(MemberTypes.Property),
            IEventSymbol     => memberTypes.HasFlag(MemberTypes.Event),
            INamedTypeSymbol => memberTypes.HasFlag(MemberTypes.NestedType),
            _                => false
        };
    private static bool Qualified(ISymbol symbol, string name, BindingFlags flags) =>
        symbol.MetadataName == name && Qualified(symbol, flags);
    private static bool Qualified(ISymbol symbol, string name, MemberTypes memberTypes) =>
        symbol.MetadataName == name && Qualified(symbol, memberTypes);
    private static bool Qualified(ISymbol symbol, string name, BindingFlags flags, MemberTypes memberTypes) =>
        Qualified(symbol, name, flags) && Qualified(symbol, memberTypes);
    
    private static string? NotGlobalNamespace(string ns) => ns == "<global namespace>" ? null : ns;
}