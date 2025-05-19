namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="IdAttribute"/> class.
/// </summary>
public class IdAttributeTests {
  /// <summary>
  /// Verifies that the Id property is initialized correctly.
  /// </summary>
  [Fact]
  public void IdProperty_ShouldBeInitializedCorrectly() {
    // Arrange
    var expectedId = "id";

    // Act
    var idAttribute = new IdAttribute(expectedId);

    // Assert
    idAttribute.Id.ShouldBe(expectedId);
  }
}
