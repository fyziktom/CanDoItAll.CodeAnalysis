namespace CanDoItAll.CodeAnalytics.Abstractions;

public enum SymbolReferenceKind {
    Invocation,
    ObjectCreation,
    PropertyAccess,
    FieldAccess,
    ConstructorParameter,
    MethodParameter,
    MethodReturn,
    Property,
    Field,
    Event,
    ServiceRegistration,
}
