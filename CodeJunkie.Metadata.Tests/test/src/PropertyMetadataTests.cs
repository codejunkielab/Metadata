namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using Xunit;

/// <summary>
/// Unit tests for the <see cref="PropertyMetadata"/> class.
/// </summary>
public class PropertyMetadataTests {
  /// <summary>
  /// Verifies that the constructor initializes the PropertyMetadata object correctly.
  /// </summary>
  [Fact]
  public void Constructor_ShouldInitializePropertyMetadataCorrectly() {
    // Arrange
    var expectedName = "Name";
    var expectedGetterValue = "Value";

    // Act
    var property = new PropertyMetadata(
      Name: expectedName,
      IsInit: false,
      IsRequired: false,
      HasDefaultValue: false,
      Getter: _ => expectedGetterValue,
      Setter: (_, _) => { },
      TypeNode: new TypeNode(typeof(string), typeof(string), false, [], _ => { }, _ => { }),
      Attributes: []
    );

    // Assert
    property.ShouldBeOfType<PropertyMetadata>();
    property.Name.ShouldBe(expectedName);
    property.Getter(null).ShouldBe(expectedGetterValue);
  }
}
