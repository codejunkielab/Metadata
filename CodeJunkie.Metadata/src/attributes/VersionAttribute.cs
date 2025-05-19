namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// An attribute used to specify the version of a class type.
/// This attribute can only be applied to classes and is inherited by derived classes.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class,
    AllowMultiple = false,
    Inherited = true)]
public class VersionAttribute : Attribute {
  /// <summary>
  /// Gets the version of the class type.
  /// The version value is always 1 or greater.
  /// </summary>
  public int Version { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="VersionAttribute"/> class
  /// with the specified version number.
  /// </summary>
  /// <param name="version">The version number of the class type.
  /// If the specified value is less than 1, it defaults to 1.</param>
  public VersionAttribute(int version) {
    Version = Math.Max(version, 1);
  }
}
