namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// An attribute used to define a mixin.
/// A mixin is an interface that extends <see cref="IMixin{TMixin}"/> and provides additional functionality to a class.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class MixinAttribute : Attribute {
  /// <summary>
  /// Initializes a new instance of the <see cref="MixinAttribute"/> class,
  /// marking an interface as a mixin.
  /// </summary>
  public MixinAttribute() { }
}
