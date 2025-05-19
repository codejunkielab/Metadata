namespace CodeJunkie.Metadata;

using CodeJunkie.Collections;

/// <summary>
/// A specialized blackboard for managing objects by their type.
/// This class extends <see cref="Blackboard"/> to provide additional functionality.
/// </summary>
public class MixinBlackboard : Blackboard {
  /// <inheritdoc />
  /// Overrides the equality check to always return true.
  /// This ensures that equality checks do not interfere with comparisons in derived record types.
  public override bool Equals(object obj) {
    return true;
  }

  /// <inheritdoc />
  /// Overrides the hash code calculation to return the base class's hash code.
  /// This ensures consistency in hash-based collections.
  public override int GetHashCode() {
    return base.GetHashCode();
  }
}
