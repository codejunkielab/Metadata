namespace CodeJunkie.Metadata;

using System;
using System.Collections.Generic;

/// <summary>
/// Interface for managing a registry of types visible at the top level.
/// </summary>
public interface ITypeRegistry {
  /// <summary>
  /// A dictionary mapping system types to their metadata.
  /// </summary>
  IReadOnlyDictionary<Type, ITypeMetadata> VisibleTypes { get; }
}
