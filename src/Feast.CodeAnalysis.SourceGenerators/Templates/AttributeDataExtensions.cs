﻿using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.CodeAnalysis;

[Literal($"Feast.CodeAnalysis.{nameof(AttributeDataExtensions)}")]
public static class AttributeDataExtensions
{
    internal static T ToAttribute<T>(this AttributeData attributeData)
        where T : Attribute
    {
        if (attributeData.AttributeConstructor == null)
            throw new ArgumentException("Attribute constructor not found");
        var attrType = typeof(T);
        var ctor = attrType.GetConstructors().FirstOrDefault(x =>
        {
            var param = x.GetParameters();
            if (param.Length != attributeData.AttributeConstructor.Parameters.Length) return false;
            return !param.Where((t, i) =>
                    attributeData.AttributeConstructor.Parameters[i].Type.ToType().FullName != t.ParameterType.FullName)
                .Any();
        });
        if (ctor == null) throw new MissingMethodException("Cannot find best match ctor for attribute");
        var param = ctor.GetParameters();
        var args = attributeData.ConstructorArguments
            .Select((x, i) => x.GetArgumentValue(param[i].ParameterType))
            .ToArray();

        var attribute = (T)Activator.CreateInstance(typeof(T), args);
        var publicProps = attrType
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(static x => x.CanWrite)
            .ToDictionary(static x => x.Name, static x => x);
        foreach (var argument in attributeData.NamedArguments)
        {
            if (!publicProps.TryGetValue(argument.Key, out var prop)) continue;
            prop.SetValue(attribute, argument.Value.GetArgumentValue(prop.PropertyType));
        }

        return attribute;
    }
}