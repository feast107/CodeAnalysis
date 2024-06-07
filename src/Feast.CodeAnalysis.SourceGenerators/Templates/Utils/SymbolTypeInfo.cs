using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
#nullable enable
namespace Feast.CodeAnalysis.Utils
{
    internal class SymbolTypeInfo : TypeInfo
    {
        private readonly ITypeSymbol type;
    
        public SymbolTypeInfo(ITypeSymbol type)
        {
            this.type = type;
            var ns = type.ContainingNamespace.ToDisplayString();
            Namespace = ns == "<global namespace>"
                ? null
                : ns;
            origin = new(() => SymbolEqualityComparer.Default.Equals(type, type.OriginalDefinition)
                ? null
                : new SymbolTypeInfo(type.OriginalDefinition));
            if (type.TypeKind == TypeKind.Class) baseClass = new(() => new SymbolTypeInfo(type.BaseType));
            switch (type)
            {
                case INamedTypeSymbol namedTypeSymbol:
                    interfaces = new(() =>
                        type.AllInterfaces
                            .Select(FromSymbol)
                            .ToArray());
                    if (namedTypeSymbol.IsGenericType)
                        genericTypes = new(() =>
                            namedTypeSymbol.TypeArguments
                                .Select(FromSymbol)
                                .ToArray());
                    break;
                case ITypeParameterSymbol parameterSymbol:
                    constrainedTypes = new(() =>
                        parameterSymbol.ConstraintTypes
                            .Select(FromSymbol)
                            .ToArray());
                    break;
            }
    
    
        }
    
        public override string? Namespace   { get; }
        public override string  Name        => type.MetadataName;
        public override bool    IsClass     => type.TypeKind == TypeKind.Class;
        public override bool    IsParameter => type.TypeKind == TypeKind.TypeParameter;
        public override bool    IsInterface => type.TypeKind == TypeKind.Interface;
        public override bool    IsEnum      => type.TypeKind == TypeKind.Enum;
    
        protected override Lazy<TypeInfo?>               baseClass        { get; } = new(() => null);
        protected override Lazy<TypeInfo?>               origin           { get; } = new(() => null);
        protected override Lazy<IReadOnlyList<TypeInfo>> genericTypes     { get; } = new(Array.Empty<TypeInfo>);
        protected override Lazy<IReadOnlyList<TypeInfo>> interfaces       { get; } = new(Array.Empty<TypeInfo>);
        protected override Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; } = new(Array.Empty<TypeInfo>);
        
    
        public static bool operator ==(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);
        public static bool operator !=(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);
    
        
    #pragma warning disable RS1024
        public override int GetHashCode() => type.GetHashCode();
    #pragma warning restore RS1024
    
        public override bool Equals(object? obj) =>
            obj is SymbolTypeInfo symbolInfo
                ? SymbolEqualityComparer.Default.Equals(type, symbolInfo.type)
                : obj is TypeInfo typeInfo
                  && SameAs(typeInfo);

        #region Text
	    internal const string SymbolTypeInfoText =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Microsoft.CodeAnalysis;
            #nullable enable 
            namespace Feast.CodeAnalysis.Utils;     
            internal class SymbolTypeInfo : TypeInfo
            {
                private readonly ITypeSymbol type;
            
                public SymbolTypeInfo(ITypeSymbol type)
                {
                    this.type = type;
                    Namespace = type.ContainingNamespace.MetadataName == string.Empty
                        ? null
                        : type.ContainingNamespace.ToDisplayString();
                    origin = new(() => SymbolEqualityComparer.Default.Equals(type, type.OriginalDefinition)
                        ? null
                        : new SymbolTypeInfo(type.OriginalDefinition));
                    if (type.TypeKind == TypeKind.Class) baseClass = new(() => new SymbolTypeInfo(type.BaseType));
                    switch (type)
                    {
                        case INamedTypeSymbol namedTypeSymbol:
                            interfaces = new(() =>
                                type.AllInterfaces
                                    .Select(FromSymbol)
                                    .ToArray());
                            if (namedTypeSymbol.IsGenericType)
                                genericTypes = new(() =>
                                    namedTypeSymbol.TypeArguments
                                        .Select(FromSymbol)
                                        .ToArray());
                            break;
                        case ITypeParameterSymbol parameterSymbol:
                            constrainedTypes = new(() =>
                                parameterSymbol.ConstraintTypes
                                    .Select(FromSymbol)
                                    .ToArray());
                            break;
                    }
            
            
                }
            
                public override string? Namespace   { get; }
                public override string  Name        => type.MetadataName;
                public override bool    IsClass     => type.TypeKind == TypeKind.Class;
                public override bool    IsParameter => type.TypeKind == TypeKind.TypeParameter;
                public override bool    IsInterface => type.TypeKind == TypeKind.Interface;
                public override bool    IsEnum      => type.TypeKind == TypeKind.Enum;
            
                protected override Lazy<TypeInfo?>               baseClass        { get; } = new(() => null);
                protected override Lazy<TypeInfo?>               origin           { get; } = new(() => null);
                protected override Lazy<IReadOnlyList<TypeInfo>> genericTypes     { get; } = new(Array.Empty<TypeInfo>);
                protected override Lazy<IReadOnlyList<TypeInfo>> interfaces       { get; } = new(Array.Empty<TypeInfo>);
                protected override Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; } = new(Array.Empty<TypeInfo>);
                
            
                public static bool operator ==(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);
                public static bool operator !=(SymbolTypeInfo one, SymbolTypeInfo another) => one.Equals(another);
            
                
            #pragma warning disable RS1024
                public override int GetHashCode() => type.GetHashCode();
            #pragma warning restore RS1024
            
                public override bool Equals(object? obj) =>
                    obj is SymbolTypeInfo symbolInfo
                        ? SymbolEqualityComparer.Default.Equals(type, symbolInfo.type)
                        : obj is TypeInfo typeInfo
                          && SameAs(typeInfo);
            }
            """;
        #endregion

    }
}

