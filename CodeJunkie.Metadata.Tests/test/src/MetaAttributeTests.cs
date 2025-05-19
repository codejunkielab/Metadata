namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="MetaAttribute"/> class.
/// </summary>
public class MetaAttributeTests {
  /// <summary>
  /// Verifies that the Mixins property contains the provided type.
  /// </summary>
  [Fact]
  public void Mixins_ShouldContainProvidedType() {
    // Arrange
    var expectedType = typeof(MetaAttributeTests);

    // Act
    var attr = new MetaAttribute(expectedType);

    // Assert
    attr.Mixins[0].ShouldBe(expectedType);
  }
}
