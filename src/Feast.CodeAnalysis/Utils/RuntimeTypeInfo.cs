namespace Feast.CodeAnalysis.Utils;

public class RuntimeTypeInfo : TypeInfo
{
    private readonly Type type;

    public RuntimeTypeInfo(Type type)
    {
        this.type = type;
        BaseClass = new(() => type.BaseType == null ? null : new RuntimeTypeInfo(type.BaseType));
        Interfaces = new(() => type.GetInterfaces()
            .Select(x => new RuntimeTypeInfo(x) as TypeInfo)
            .ToArray());
        if (type.IsGenericType)
            GenericTypes = new(() => type
                .GetGenericArguments()
                .Select(x => new RuntimeTypeInfo(x) as TypeInfo)
                .ToArray());
    }

    public override string           Name         => type.Namespace + "." + type.Name;
    public override bool             IsParameter  => type.IsGenericParameter;
    public override bool             IsInterface  => type.IsInterface;
    public override Lazy<TypeInfo[]> GenericTypes { get; } = new();
    public override Lazy<TypeInfo?>  BaseClass    { get; } = new();
    public override Lazy<TypeInfo[]> Interfaces   { get; } = new();
}