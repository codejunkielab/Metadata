namespace CodeJunkie.Metadata;

using System;
using System.Collections.Generic;
using CodeJunkie.Collections;

/// <summary>
/// Represents a graph of registered types, including generic and nested types.
/// </summary>
public interface ITypeGraph {
  /// <summary>
  /// All registered identifiable types.
  /// </summary>
  IEnumerable<Type> IdentifiableTypes { get; }

  /// <summary>
  /// Caches type lookup tables from the given registry.
  /// </summary>
  /// <param name="registry">The type registry to process.</param>
  void Register(ITypeRegistry registry);

  /// <summary>
  /// Gets the latest version of a type by its unique ID in O(1) time.
  /// </summary>
  /// <param name="id">The unique identifier of the type.</param>
  /// <returns>The latest version, or null if not found.</returns>
  int? GetLatestVersion(string id);

  /// <summary>
  /// Finds a type by its ID and optional version.
  /// </summary>
  /// <param name="id">The unique identifier of the type.</param>
  /// <param name="version">The version to retrieve, or null for the latest.</param>
  /// <returns>The type metadata, or null if not found.</returns>
  Type? GetIdentifiableType(string id, int? version = null);

  /// <summary>
  /// Checks if metadata exists for a given type.
  /// </summary>
  /// <param name="type">The type to check.</param>
  /// <returns>True if metadata exists; otherwise, false.</returns>
  bool HasMetadata(Type type);

  /// <summary>
  /// Gets metadata for a given type.
  /// </summary>
  /// <param name="type">The type to retrieve metadata for.</param>
  /// <returns>The metadata, or null if not found.</returns>
  ITypeMetadata? GetMetadata(Type type);

  /// <summary>
  /// Gets the immediate subtypes of a given type.
  /// </summary>
  /// <param name="type">The parent type.</param>
  /// <returns>A set of immediate subtypes.</returns>
  IReadOnlySet<Type> GetSubtypes(Type type);

  /// <summary>
  /// Gets all subtypes of a given type, including descendants.
  /// </summary>
  /// <param name="type">The ancestor type.</param>
  /// <returns>A set of all descendant subtypes.</returns>
  IReadOnlySet<Type> GetDescendantSubtypes(Type type);

  /// <summary>
  /// Gets all properties of a given type, including inherited ones.
  /// </summary>
  /// <param name="type">The type to inspect.</param>
  /// <returns>A collection of all properties.</returns>
  IEnumerable<PropertyMetadata> GetProperties(Type type);

  /// <summary>
  /// Gets the first attribute of a specified type applied to a type.
  /// </summary>
  /// <typeparam name="TAttribute">The attribute type to find.</typeparam>
  /// <param name="type">The type to search.</param>
  /// <returns>The attribute instance, or null if not found.</returns>
  TAttribute? GetAttribute<TAttribute>(Type type) where TAttribute : Attribute;

  /// <summary>
  /// Gets all attributes of a given type, including inherited ones.
  /// </summary>
  /// <param name="type">The type to inspect.</param>
  /// <returns>A dictionary mapping attribute types to their instances.</returns>
  IReadOnlyDictionary<Type, Attribute[]> GetAttributes(Type type);

  /// <summary>
  /// Registers a custom type with the introspection system.
  /// </summary>
  /// <param name="type">The system type.</param>
  /// <param name="name">The name of the type.</param>
  /// <param name="genericTypeGetter">Callback to retrieve the generic type.</param>
  /// <param name="factory">Function to create a new instance of the type.</param>
  /// <param name="id">The unique identifier for the type.</param>
  /// <param name="version">The version of the type (default is 1).</param>
  void AddCustomType(Type type,
                     string name,
                     Action<ITypeReceiver> genericTypeGetter,
                     Func<object> factory,
                     string id,
                     int version = 1);
}
