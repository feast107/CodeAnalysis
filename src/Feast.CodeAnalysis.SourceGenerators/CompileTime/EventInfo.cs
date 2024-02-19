using System;
using System.Linq;

namespace Feast.CodeAnalysis.CompileTime;

[Literal("Feast.CodeAnalysis.CompileTime.EventInfo")]
internal partial class EventInfo(global::Microsoft.CodeAnalysis.IEventSymbol @event) : global::System.Reflection.EventInfo
{
    public override object[] GetCustomAttributes(bool inherit)
        => @event.GetAttributes()
            .CastArray<object>()
            .ToArray();

    public override object[] GetCustomAttributes(global::System.Type attributeType, bool inherit) =>
        @event.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName)
            .Cast<object>()
            .ToArray();

    public override bool IsDefined(global::System.Type attributeType, bool inherit) =>
        @event
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == attributeType.FullName);

    public override global::System.Type DeclaringType => new Type((@event.ContainingSymbol as global::Microsoft.CodeAnalysis.ITypeSymbol)!);

    public override string              Name          => @event.MetadataName;
    public override global::System.Type ReflectedType => DeclaringType;

    public override global::System.Reflection.MethodInfo? GetAddMethod(bool nonPublic) =>
        @event.AddMethod == null
            ? null
            : @event.AddMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(@event.AddMethod)
                : null;

    public override global::System.Reflection.MethodInfo? GetRaiseMethod(bool nonPublic)
        => @event.RaiseMethod == null
            ? null
            : @event.RaiseMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(@event.RaiseMethod)
                : null;

    public override global::System.Reflection.MethodInfo? GetRemoveMethod(bool nonPublic) =>
        @event.RemoveMethod == null
            ? null
            : @event.RemoveMethod.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.Public == nonPublic
                ? new MethodInfo(@event.RemoveMethod)
                : null;

    public override System.Reflection.EventAttributes Attributes =>
        System.Reflection.EventAttributes.None;
}