using Feast.CodeAnalysis.Generators;
using Feast.CodeAnalysis.TestGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
namespace Feast.CodeAnalysis.Tests;
using AnotherName = AnotherClass;

public class Tests
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test()
    {
        // Create an instance of the source generator.
        var generator = new TestIncrementalGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(Tests),
            new[] { CSharpSyntaxTree.ParseText(CodeText) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
            });

        // Run generators and retrieve all results.
        var runResult = driver.RunGenerators(compilation).GetRunResult();

        // All generated files can be found in 'RunResults.GeneratedTrees'.
        var generatedFileSyntax = runResult.GeneratedTrees.Single(t => t.FilePath.EndsWith("Vector3.g.cs"));
    }



    public const string CodeText =
        """
        using Generators;
        using System.Collections.Generic;
             
        [Analyze]
        public class TestClass<T> where T : IDictionary<T,string> {
            public T Id { get; set; }
            
            public System.Collections.IEnumerable<T> Num { get;set; }
        }
        """;
}

