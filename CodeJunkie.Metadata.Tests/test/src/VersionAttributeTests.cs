namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="VersionAttribute"/> class.
/// </summary>
public class VersionAttributeTests {
  /// <summary>
  /// Verifies that the Version property is set correctly during initialization.
  /// </summary>
  [Fact]
  public void Version_ShouldBeSetCorrectly() {
    // Arrange
    var expectedVersion = 2;

    // Act
    var attr = new VersionAttribute(expectedVersion);

    // Assert
    attr.Version.ShouldBe(expectedVersion);
  }
}
