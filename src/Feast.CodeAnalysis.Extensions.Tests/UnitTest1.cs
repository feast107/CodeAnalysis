using Generators;

namespace Feast.CodeAnalysis.Extensions.Tests;
using S = System.SerializableAttribute;

[RelateTo(typeof(Another))]
public partial class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}

public class Another
{
}