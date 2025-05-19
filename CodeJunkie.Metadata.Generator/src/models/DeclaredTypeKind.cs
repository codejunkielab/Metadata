namespace CodeJunkie.Metadata.Generator.Models;

/// <summary>
/// Represents the kind of a declared type in the metadata generator.
/// </summary>
public enum DeclaredTypeKind {
  /// <summary>
  /// A class that cannot be instantiated and only contains static members.
  /// </summary>
  StaticClass,

  /// <summary>
  /// A type that cannot be instantiated directly and is intended to be inherited.
  /// </summary>
  AbstractType,

  /// <summary>
  /// A fully implemented type that can be instantiated.
  /// </summary>
  ConcreteType,

  /// <summary>
  /// A contract that defines a set of methods and properties without implementation.
  /// </summary>
  Interface,

  /// <summary>
  /// Represents an invalid or unrecognized type.
  /// </summary>
  Error
}
