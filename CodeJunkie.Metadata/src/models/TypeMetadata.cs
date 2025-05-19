namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// Provides metadata for a type accessible from the top-level namespace.
/// </summary>
public interface ITypeMetadata {
  /// <summary>
/// The name of the type, including open generics if applicable.
  /// </summary>
  string Name { get; }
}

/// <summary>
/// Metadata for a closed generic type.
/// </summary>
public interface IClosedTypeMetadata : ITypeMetadata {
  /// <summary>
/// A delegate to invoke <see cref="ITypeReceiver.Receive{T}" /> with the generic type.
  /// </summary>
  Action<ITypeReceiver> GenericTypeGetter { get; }
}

/// <summary>
/// Metadata for a concrete (instantiable) type.
/// </summary>
public interface IConcreteTypeMetadata : IClosedTypeMetadata {
  /// <summary>
/// A delegate to create a new instance of the type.
  /// </summary>
  Func<object> Factory { get; }
}

/// <summary>
/// Metadata about an introspective type.
/// </summary>
public interface IIntrospectiveTypeMetadata : IClosedTypeMetadata {
  /// <summary>
/// Provides details about the metatype, including properties and attributes.
  /// </summary>
  IMetatype Metatype { get; }
}

/// <summary>
/// Metadata about a concrete introspective type.
/// </summary>
public interface IConcreteIntrospectiveTypeMetadata : IIntrospectiveTypeMetadata, IConcreteTypeMetadata {
  /// <summary>
/// The version of the introspective type, defined by <see cref="VersionAttribute" />.
  /// </summary>
  int Version { get; }
}

/// <summary>
/// Metadata about an identifiable introspective type.
/// </summary>
public interface IIdentifiableTypeMetadata : IIntrospectiveTypeMetadata {
  /// <summary>
/// The identifier of the introspective type, defined by <see cref="IdAttribute" />.
  /// </summary>
  string Id { get; }
}

/// <summary>
/// Metadata for a type that is abstract, generic, or both.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
public record TypeMetadata(string Name) : ITypeMetadata;

/// <summary>
/// Metadata for a concrete type that can be instantiated.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
/// <param name="GenericTypeGetter"><inheritdoc cref="IClosedTypeMetadata.GenericTypeGetter" path="/summary" /></param>
/// <param name="Factory"><inheritdoc cref="IConcreteTypeMetadata.Factory" path="/summary" /></param>
public record ConcreteTypeMetadata(string Name,
                                   Action<ITypeReceiver> GenericTypeGetter,
                                   Func<object> Factory) :
  TypeMetadata(Name),
  IConcreteTypeMetadata;

/// <summary>
/// Metadata for an abstract introspective type.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
/// <param name="GenericTypeGetter"><inheritdoc cref="IClosedTypeMetadata.GenericTypeGetter" path="/summary" /></param>
/// <param name="Metatype"><inheritdoc cref="IIntrospectiveTypeMetadata.Metatype" path="/summary" /></param>
public record AbstractIntrospectiveTypeMetadata(string Name,
                                                Action<ITypeReceiver> GenericTypeGetter,
                                                IMetatype Metatype) :
  TypeMetadata(Name),
  IIntrospectiveTypeMetadata;

/// <summary>
/// Metadata for a concrete introspective type.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
/// <param name="GenericTypeGetter"><inheritdoc cref="IClosedTypeMetadata.GenericTypeGetter" path="/summary" /></param>
/// <param name="Factory"><inheritdoc cref="IConcreteTypeMetadata.Factory" path="/summary" /></param>
/// <param name="Metatype"><inheritdoc cref="IIntrospectiveTypeMetadata.Metatype" path="/summary" /></param>
/// <param name="Version"><inheritdoc cref="IConcreteIntrospectiveTypeMetadata.Version" path="/summary" /></param>
public record IntrospectiveTypeMetadata(string Name,Action<ITypeReceiver> GenericTypeGetter,
                                        Func<object> Factory,
                                        IMetatype Metatype,
                                        int Version) :
  ConcreteTypeMetadata(Name, GenericTypeGetter, Factory),
  IConcreteIntrospectiveTypeMetadata;

/// <summary>
/// Metadata for an abstract and identifiable introspective type.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
/// <param name="GenericTypeGetter"><inheritdoc cref="IClosedTypeMetadata.GenericTypeGetter" path="/summary" /></param>
/// <param name="Metatype"><inheritdoc cref="IIntrospectiveTypeMetadata.Metatype" path="/summary" /></param>
/// <param name="Id"><inheritdoc cref="IIdentifiableTypeMetadata.Id" path="/summary" /></param>
public record AbstractIdentifiableTypeMetadata(string Name,
                                               Action<ITypeReceiver> GenericTypeGetter,
                                               IMetatype Metatype,
                                               string Id) :
  AbstractIntrospectiveTypeMetadata(Name, GenericTypeGetter, Metatype),
  IIdentifiableTypeMetadata;


/// <summary>
/// Metadata for a concrete and identifiable introspective type.
/// </summary>
/// <param name="Name"><inheritdoc cref="ITypeMetadata.Name" path="/summary" /></param>
/// <param name="GenericTypeGetter"><inheritdoc cref="IClosedTypeMetadata.GenericTypeGetter" path="/summary" /></param>
/// <param name="Factory"><inheritdoc cref="IConcreteTypeMetadata.Factory" path="/summary" /></param>
/// <param name="Metatype"><inheritdoc cref="IIntrospectiveTypeMetadata.Metatype" path="/summary" /></param>
/// <param name="Id"><inheritdoc cref="IIdentifiableTypeMetadata.Id" path="/summary" /></param>
/// <param name="Version"><inheritdoc cref="IConcreteIntrospectiveTypeMetadata.Version" path="/summary" /></param>
public record IdentifiableTypeMetadata(string Name,
                                       Action<ITypeReceiver> GenericTypeGetter,
                                       Func<object> Factory,
                                       IMetatype Metatype,
                                       string Id,
                                       int Version) :
  IntrospectiveTypeMetadata(Name, GenericTypeGetter, Factory, Metatype, Version),
  IIdentifiableTypeMetadata,
  IConcreteIntrospectiveTypeMetadata;
