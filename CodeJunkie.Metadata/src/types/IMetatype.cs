namespace CodeJunkie.Metadata;

using System;
using System.Collections.Generic;

/// <summary>
/// Defines a metatype that describes another type.
/// </summary>
public interface IMetatype {
  /// <summary>
  /// The system type described by this metatype.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// True if the type has init-only properties requiring initialization during construction.
  /// Use <see cref="Construct"/> to set these properties.
  /// </summary>
  bool HasInitProperties { get; }

  /// <summary>
  /// The properties of the type, excluding inherited or partial properties.
  /// Use <see cref="ITypeGraph.GetProperties(Type)"/> for all properties.
  /// </summary>
  IReadOnlyList<PropertyMetadata> Properties { get; }

  /// <summary>
  /// Attributes directly applied to the type.
  /// </summary>
  IReadOnlyDictionary<Type, Attribute[]> Attributes { get; }

  /// <summary>
  /// Mixins applied to the type, in application order.
  /// </summary>
  IReadOnlyList<Type> Mixins { get; }

  /// <summary>
  /// A dictionary of mixin handler functions, keyed by mixin type.
  /// </summary>
  IReadOnlyDictionary<Type, Action<object>> MixinHandlers { get; }

  /// <summary>
  /// Creates an instance of the type with the given arguments.
  /// Throws an exception if the type is abstract. Use this to initialize init-only properties.
  /// </summary>
  /// <param name="args">A dictionary of argument names and values.</param>
  /// <returns>A new instance of the type.</returns>
  object Construct(IReadOnlyDictionary<string, object?>? args = null);
}
