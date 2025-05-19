namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// An attribute used to assign a unique identifier to a class or struct.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = true)]
public class IdAttribute : Attribute {
  /// <summary>
  /// Gets the unique identifier assigned to the type.
  /// </summary>
  public string Id { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="IdAttribute"/> class with the specified unique identifier.
  /// </summary>
  /// <param name="id">A string representing the unique identifier for the type.</param>
  public IdAttribute(string id) {
    Id = id;
  }
}
