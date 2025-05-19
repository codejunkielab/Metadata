namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// An attribute used to mark a reference type for metadata generation at compile-time.
/// The target type must be a partial class or a partial record class.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public sealed class MetaAttribute : Attribute {

  /// <summary>
  /// Gets the interfaces (mixins) applied to the type.
  /// These mixins are processed at build-time and enable dynamic invocation at runtime
  /// without requiring explicit knowledge in the target type.
  /// </summary>
  public Type[] Mixins { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="MetaAttribute"/> class with the specified mixins.
  /// </summary>
  /// <param name="mixins">An array of interfaces (mixins) to be applied to the target type.</param>
  public MetaAttribute(params Type[] mixins) {
    Mixins = mixins;
  }
}
