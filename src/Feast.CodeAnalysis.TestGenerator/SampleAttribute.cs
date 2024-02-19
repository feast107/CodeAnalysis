using System;

namespace Feast.CodeAnalysis.TestGenerator;
[Literal("System.SampleAttribute")]
[AttributeUsage(AttributeTargets.Class)]
public class SampleAttribute(params Type[] types) : Attribute
{
    
}