using System.Diagnostics;
using System.Reflection;

foreach (var item in RoslynAssemblies.Report())
{
    Console.WriteLine(item);
}
Console.WriteLine();

Console.WriteLine(ScriptGeneratedClass.Report());

file class ErrorInitializedClass
{
    private static readonly string Failed = GetError();

    private static string GetError() => throw new TypeInitializationException("", new AbandonedMutexException());
}