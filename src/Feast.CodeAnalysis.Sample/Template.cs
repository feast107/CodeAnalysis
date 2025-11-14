foreach (var item in RoslynTypes.Report())
{
    Console.WriteLine(item);
}

file class ErrorInitializedClass
{
    private static readonly string Failed = GetError();

    private static string GetError() => throw new TypeInitializationException("", new AbandonedMutexException());
}