using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.EventInfo")]
internal partial class EventInfo(global::Microsoft.CodeAnalysis.IEventSymbol symbol) : global::System.Reflection.EventInfo
{
    internal IEventSymbol Symbol => symbol;
    
    public override object[] GetCustomAttributes(bool inherit)
        => symbol.GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        symbol.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        symbol
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType => new Type((symbol.ContainingSymbol as global::Microsoft.CodeAnalysis.ITypeSymbol)!);

    public override string              Name          => symbol.MetadataName;
    public override global::System.Type ReflectedType => DeclaringType;

    public override global::System.Reflection.MethodInfo? GetAddMethod(bool nonPublic) =>
        symbol.AddMethod == null
            ? null
            : symbol.AddMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(symbol.AddMethod)
                : null;

    public override global::System.Reflection.MethodInfo? GetRaiseMethod(bool nonPublic)
        => symbol.RaiseMethod == null
            ? null
            : symbol.RaiseMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(symbol.RaiseMethod)
                : null;

    public override global::System.Reflection.MethodInfo? GetRemoveMethod(bool nonPublic) =>
        symbol.RemoveMethod == null
            ? null
            : symbol.RemoveMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(symbol.RemoveMethod)
                : null;

    public override System.Reflection.EventAttributes Attributes =>
        System.Reflection.EventAttributes.None;
}