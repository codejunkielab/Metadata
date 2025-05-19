namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="MixinBlackboard"/> class.
/// </summary>
public class MixinBlackboardTests {
  /// <summary>
  /// Verifies that the Equals method always returns true, regardless of the input.
  /// </summary>
  [Fact]
  public void Equals_ShouldAlwaysReturnTrue() {
    // Arrange
    var blackboard = new MixinBlackboard();


    // Act & Assert
    blackboard.Equals(null!).ShouldBeTrue();
    blackboard.Equals(new object()).ShouldBeTrue();
    blackboard.Equals(blackboard).ShouldBeTrue();
  }

  /// <summary>
  /// Verifies that the GetHashCode method returns a valid integer.
  /// </summary>
  [Fact]
  public void GetHashCode_ShouldReturnValidInteger() {
    // Arrange
    var blackboard = new MixinBlackboard();

    // Act
    var hashCode = blackboard.GetHashCode();

    // Assert
    hashCode.ShouldBeOfType<int>();
  }
}
