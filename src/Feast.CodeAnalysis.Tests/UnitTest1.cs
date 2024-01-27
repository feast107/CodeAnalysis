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
    
    [Test]
    public void Test()
    {
        var          file = Path.Combine(Dir(),"AnotherClass.cs");
        // Create an instance of the source generator.
        var generator = new LiteralGenerator.LiteralGenerator();

        // Source generators should be tested using 'GeneratorDriver'.
        var driver = CSharpGeneratorDriver.Create(generator);

        // We need to create a compilation with the required source code.
        var compilation = CSharpCompilation.Create(nameof(Tests),
            new[] { CSharpSyntaxTree.ParseText(File.ReadAllText(file)) },
            new[]
            {
                // To support 'System.Attribute' inheritance, add reference to 'System.Private.CoreLib'.
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(StringBuilder).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Array).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
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
        
        namespace G;
        [Analyze]
        public class TestClass<T> where T : IEnumerable<T> {
            public Foo Id { get; set; }
            
            public System.Collections.IEnumerable<T> Num { get;set; }
        }
        
        public class Foo : IEnumerable<Foo>;
        """;
}

