using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
#nullable enable
namespace Feast.CodeAnalysis.Utils
{
    internal class RuntimeTypeInfo : TypeInfo
    {
        private readonly Type type;
    
        public RuntimeTypeInfo(Type type)
        {
            this.type = type;
            if (type.BaseType != null)
            {
                baseClass = new(() => new RuntimeTypeInfo(type.BaseType));
            }
            origin = new(() =>
            {
                var def = type.GetGenericTypeDefinition();
                return def == null || def == type ? null : new RuntimeTypeInfo(type);
            });
            interfaces = new(() =>
                type.GetInterfaces()
                    .Select(FromType)
                    .ToArray());
            if (type.IsGenericType)
                genericTypes = new(() =>
                    type.GetGenericArguments()
                        .Select(FromType)
                        .ToArray());
            if (type.IsGenericParameter)
            {
                constrainedTypes = new(() =>
                    type.GetGenericParameterConstraints()
                        .Select(FromType)
                        .ToArray());
            }
        }
    
        public override string? Namespace   => type.Namespace;
        public override string  Name        => type.Name;
        public override bool    IsParameter => type.IsGenericParameter;
        public override bool    IsInterface => type.IsInterface;
    
        protected override Lazy<TypeInfo?>               baseClass        { get; } = new(() => null);
        protected override Lazy<TypeInfo?>               origin           { get; } = new(() => null);
        protected override Lazy<IReadOnlyList<TypeInfo>> genericTypes     { get; } = new(Array.Empty<TypeInfo>);
        protected override Lazy<IReadOnlyList<TypeInfo>> interfaces       { get; } = new(Array.Empty<TypeInfo>);
        protected override Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; } = new(Array.Empty<TypeInfo>);
    
        public static bool operator ==(RuntimeTypeInfo one, RuntimeTypeInfo another) => one.Equals(another);
        public static bool operator !=(RuntimeTypeInfo one, RuntimeTypeInfo another) => one.Equals(another);
    
        public override int GetHashCode() => type.GetHashCode();
    
        public override bool Equals(object? obj) =>
            obj is RuntimeTypeInfo info
                ? info.type == type
                : obj is TypeInfo typeInfo
                  && SameAs(typeInfo);

	    internal const string RuntimeTypeInfoText =
            """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Microsoft.CodeAnalysis;
            #nullable enable 
            namespace Feast.CodeAnalysis.Utils;     
            internal class RuntimeTypeInfo : TypeInfo
            {
                private readonly Type type;
            
                public RuntimeTypeInfo(Type type)
                {
                    this.type = type;
                    if (type.BaseType != null)
                    {
                        baseClass = new(() => new RuntimeTypeInfo(type.BaseType));
                    }
                    origin = new(() =>
                    {
                        var def = type.GetGenericTypeDefinition();
                        return def == null || def == type ? null : new RuntimeTypeInfo(type);
                    });
                    interfaces = new(() =>
                        type.GetInterfaces()
                            .Select(FromType)
                            .ToArray());
                    if (type.IsGenericType)
                        genericTypes = new(() =>
                            type.GetGenericArguments()
                                .Select(FromType)
                                .ToArray());
                    if (type.IsGenericParameter)
                    {
                        constrainedTypes = new(() =>
                            type.GetGenericParameterConstraints()
                                .Select(FromType)
                                .ToArray());
                    }
                }
            
                public override string? Namespace   => type.Namespace;
                public override string  Name        => type.Name;
                public override bool    IsParameter => type.IsGenericParameter;
                public override bool    IsInterface => type.IsInterface;
            
                protected override Lazy<TypeInfo?>               baseClass        { get; } = new(() => null);
                protected override Lazy<TypeInfo?>               origin           { get; } = new(() => null);
                protected override Lazy<IReadOnlyList<TypeInfo>> genericTypes     { get; } = new(Array.Empty<TypeInfo>);
                protected override Lazy<IReadOnlyList<TypeInfo>> interfaces       { get; } = new(Array.Empty<TypeInfo>);
                protected override Lazy<IReadOnlyList<TypeInfo>> constrainedTypes { get; } = new(Array.Empty<TypeInfo>);
            
                public static bool operator ==(RuntimeTypeInfo one, RuntimeTypeInfo another) => one.Equals(another);
                public static bool operator !=(RuntimeTypeInfo one, RuntimeTypeInfo another) => one.Equals(another);
            
                public override int GetHashCode() => type.GetHashCode();
            
                public override bool Equals(object? obj) =>
                    obj is RuntimeTypeInfo info
                        ? info.type == type
                        : obj is TypeInfo typeInfo
                          && SameAs(typeInfo);
            }
            """;
    }
}
