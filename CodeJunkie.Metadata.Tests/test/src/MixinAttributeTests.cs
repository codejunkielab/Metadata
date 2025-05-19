namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="MixinAttribute"/> class.
/// </summary>
public class MixinAttributeTests {
  /// <summary>
  /// Verifies that an instance of <see cref="MixinAttribute"/> is created correctly.
  /// </summary>
  [Fact]
  public void Instance_ShouldBeOfTypeMixinAttribute() {
    // Arrange & Act
    var attr = new MixinAttribute();

    // Assert
    attr.ShouldBeOfType<MixinAttribute>();
  }
}
