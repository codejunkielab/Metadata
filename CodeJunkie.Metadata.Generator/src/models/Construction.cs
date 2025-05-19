namespace CodeJunkie.Metadata.Generator.Models;

/// <summary>
/// Represents the different types of constructions available in the metadata generator.
/// </summary>
public enum Construction {
  /// <summary>
  /// Represents a static class, which cannot be instantiated and contains only static members.
  /// </summary>
  StaticClass,

  /// <summary>
  /// Represents a regular class, which can be instantiated and may contain instance and static members.
  /// </summary>
  Class,

  /// <summary>
  /// Represents a record struct, which is a value type with built-in support for immutability and value equality.
  /// </summary>
  RecordStruct,

  /// <summary>
  /// Represents a record class, which is a reference type with built-in support for immutability and value equality.
  /// </summary>
  RecordClass,

  /// <summary>
  /// Represents an interface, which defines a contract that implementing classes or structs must adhere to.
  /// </summary>
  Interface,

  /// <summary>
  /// Represents a struct, which is a value type that can contain data and methods.
  /// </summary>
  Struct
}
