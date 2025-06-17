using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Feast.CodeAnalysis.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    public string Current([CallerFilePath] string path = "") => path;
    public string Dir([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;

    private const string Attribute =
        """

        namespace Feast.CodeAnalysis.TestGenerator
        {
            [AttributeUsage(AttributeTargets.Class)]
            public class SampleAttribute(params Type[] types) : global::System.Attribute
            {
            }
        }
        """;

    [Test]
    public void Test()
    {
        var file = Path.Combine(Dir(), "AnotherClass.cs");
        // Create an instance of the source generator.
        var generator =
            new Generators.LiteralGenerator.LiteralGenerator();
            //new TestIncrementalGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(Tests),
            [
                CSharpSyntaxTree.ParseText(Attribute),
                CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            ],
            [
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(StringBuilder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Array).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location)
            ]);

        // Run generators and retrieve all results.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // All generated files can be found in 'RunResults.GeneratedTrees'.
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("Vector3.g.cs"));
    }


    public const string CodeText =
        """
        using System;
        
        namespace Feast.CodeAnalysis.TestGenerator
        {
            [AttributeUsage(AttributeTargets.Class)]
            public class SampleAttribute(params Type[] types) : global::System.Attribute
            {
            }
        }
        """;

    [Test]
    public void TestStaticClass()
    {
    }


    public class Static
    {
        static Static()
        {
        }

        public Static(int i)
        {
            
        }
    }
}