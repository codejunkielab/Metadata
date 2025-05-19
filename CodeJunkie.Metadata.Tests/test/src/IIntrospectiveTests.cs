namespace CodeJunkie.Metadata.Tests;

using Shouldly;
using System;
using Xunit;

[Mixin]
public interface IMixin1 : IMixin<IMixin1> {
  void IMixin<IMixin1>.Handler() => IIntrospectiveTest.Called1 = true;
}

[Mixin]
public interface IMixin2 : IMixin<IMixin2> {
  void IMixin<IMixin2>.Handler() => IIntrospectiveTest.Called2 = true;
}

[Meta(typeof(IMixin1), typeof(IMixin2))]
public partial class MyTypeWithAMixin { }

/// <summary>
/// Unit tests for introspective mixin functionality.
/// </summary>
public class IIntrospectiveTest {
  public static bool Called1 { get; set; }
  public static bool Called2 { get; set; }

  public IIntrospectiveTest() {
    Called1 = false;
    Called2 = false;
  }

  /// <summary>
  /// Verifies that invoking a specific mixin calls its handler.
  /// </summary>
  [Fact]
  public void InvokeMixin_ShouldCallSpecificMixinHandler() {
    // Arrange
    IIntrospectiveRef myType = new MyTypeWithAMixin();

    // Act
    myType.InvokeMixin(typeof(IMixin1));

    // Assert
    Called1.ShouldBeTrue();
  }

  /// <summary>
  /// Verifies that invoking all mixins calls all handlers.
  /// </summary>
  [Fact]
  public void InvokeMixins_ShouldCallAllHandlers() {
    // Arrange
    IIntrospectiveRef myType = new MyTypeWithAMixin();

    // Act
    myType.InvokeMixins();

    // Assert
    Called1.ShouldBeTrue();
    Called2.ShouldBeTrue();
  }

  /// <summary>
  /// Verifies that invoking a missing mixin throws an exception.
  /// </summary>
  [Fact]
  public void InvokeMixin_ShouldThrowOnMissingMixin() {
    // Arrange
    IIntrospectiveRef myType = new MyTypeWithAMixin();

    // Act & Assert
    Should.Throw<InvalidOperationException>(
        () => myType.InvokeMixin(typeof(IIntrospectiveTest)));
  }
}
