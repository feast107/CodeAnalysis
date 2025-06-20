using System;
using System.Diagnostics.CodeAnalysis;

namespace Feast.CodeAnalysis.Scripting;

[Literal("Feast.CodeAnalysis.Scripting.Generates.ScriptingResources")]
public static class ScriptingResources
{
    private static readonly Type ResourceType =
        Global.GetAssembly("Microsoft.CodeAnalysis.Scripting")?
            .GetType("Microsoft.CodeAnalysis.Scripting.ScriptingResources")
        ?? throw new InvalidOperationException(
            "Could not find ScriptingResources type in Microsoft.CodeAnalysis.Scripting assembly.");

    [field: AllowNull, MaybeNull]
    public static string CannotSetReadOnlyVariable =>
        field ??= ResourceType.GetProperty(nameof(CannotSetReadOnlyVariable))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(CannotSetReadOnlyVariable)} property in {nameof(ScriptingResources)} type.");

    [field: AllowNull, MaybeNull]
    public static string CannotSetConstantVariable =>
        field ??= ResourceType.GetProperty(nameof(CannotSetConstantVariable))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(CannotSetConstantVariable)} property in {nameof(ScriptingResources)} type.");

    [field: AllowNull, MaybeNull]
    public static string ScriptRequiresGlobalVariables =>
        field ??= ResourceType.GetProperty(nameof(ScriptRequiresGlobalVariables))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(ScriptRequiresGlobalVariables)} property in {nameof(ScriptingResources)} type.");

    [field: AllowNull, MaybeNull]
    public static string GlobalsNotAssignable =>
        field ??= ResourceType.GetProperty(nameof(GlobalsNotAssignable))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(GlobalsNotAssignable)} property in {nameof(ScriptingResources)} type.");
    
    [field: AllowNull, MaybeNull]
    public static string GlobalVariablesWithoutGlobalType =>
        field ??= ResourceType.GetProperty(nameof(GlobalVariablesWithoutGlobalType))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(GlobalVariablesWithoutGlobalType)} property in {nameof(ScriptingResources)} type.");
    
    [field: AllowNull, MaybeNull]
    public static string StartingStateIncompatible =>
        field ??= ResourceType.GetProperty(nameof(StartingStateIncompatible))?.GetValue(null) as string
                  ?? throw new InvalidOperationException(
                      $"Could not find {nameof(StartingStateIncompatible)} property in {nameof(ScriptingResources)} type.");

}