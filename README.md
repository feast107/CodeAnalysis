# CodeAnalysis
 Auto generated extensions for `Microsoft.CodeAnalysis`

## Require

+ PolySharp

## Extra

+ `TypeInfo` in `Feast.CodeAnalysis.Utils`

    ``` csharp
    TypeInfo.FromSymbol(ITypeSymcol); // SymbolTypeInfo
    TypeInfo.FromType(Type);          // RuntimeTypeInfo
    TypeInfo.FromType<T>();           // RuntimeTypeInfo
    ```

    Provides common description between compile time `Symbol` and run time `Type`

    Includes relation comparation:

    + TypeInfo.SameAs(TypeInfo)
    + TypeInfo.IsAssignableTo(TypeInfo)
    + TypeInfo.IsAssignableFrom(TypeInfo)
    + TypeInfo.IsSubClassOf(TypeInfo)