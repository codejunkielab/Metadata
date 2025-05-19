namespace CodeJunkie.Metadata.Tests;

using System;
using Shouldly;
using Xunit;


public class TypeNodeTest {
  // テスト対象の TypeNode インスタンス
  private readonly TypeNode _type = new(
    OpenType: typeof(string),
    ClosedType: typeof(string),
    IsNullable: false,
    Arguments: [],
    GenericTypeGetter: receiver => receiver.Receive<string>(),
    GenericTypeGetter2: (_) => throw new NotImplementedException()
  );

  [Fact]
  public void TypeNode_ShouldBeInitializedCorrectly() {
    // Arrange: テスト対象のインスタンスはすでに初期化済み

    // Act: 特定のアクションは不要

    // Assert: TypeNode の型が正しいことを検証
    _type.ShouldBeOfType<TypeNode>();
  }
}
