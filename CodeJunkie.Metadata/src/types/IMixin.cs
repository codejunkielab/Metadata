namespace CodeJunkie.Metadata;

/// <summary>
/// Base interface for mixins. Implement this interface and use <see cref="MixinAttribute"/> to define a mixin.
/// </summary>
/// <typeparam name="TMixin">Type of the mixin.</typeparam>
public interface IMixin<TMixin> : IIntrospectiveRef {
  /// <summary>
  /// Handler method for the mixin. Types with <see cref="MetaAttribute"/> generate mixin data at build time, allowing runtime invocation without prior knowledge of applied mixins.
  /// </summary>
  void Handler();
}
