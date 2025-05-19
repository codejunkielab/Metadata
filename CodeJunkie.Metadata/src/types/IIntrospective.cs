namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// Defines a type with associated metatype information.
/// </summary>
public interface IIntrospective {
  /// <summary>
  /// Generated metatype information.
  /// </summary>
  IMetatype Metatype { get; }
}

/// <summary>
/// Defines reference types that support mixins and introspection.
/// </summary>
public interface IIntrospectiveRef : IIntrospective {
  /// <summary>
  /// Holds shared data for mixins, enabling additional instance state management.
  /// </summary>
  MixinBlackboard MixinState { get; }

  /// <summary>
  /// Determines whether a specific mixin is applied to the type.
  /// </summary>
  /// <param name="type">The mixin type to verify.</param>
  /// <returns>True if the mixin is applied; otherwise, false.</returns>
  bool HasMixin(Type type) {
    return Metatype.MixinHandlers.ContainsKey(type);
  }

  /// <summary>
  /// Invokes handlers for all mixins applied to the type.
  /// </summary>
  void InvokeMixins() {
    for (var i = 0; i < Metatype.Mixins.Count; i++) {
      Metatype.MixinHandlers[Metatype.Mixins[i]](this);
    }
  }

  /// <summary>
  /// Invokes the handler for a specific mixin applied to the type.
  /// </summary>
  /// <param name="type">The mixin type to invoke.</param>
  /// <exception cref="InvalidOperationException">Thrown if the mixin is not applied to the type.</exception>
  void InvokeMixin(Type type) {
    if (!HasMixin(type)) {
      throw new InvalidOperationException(
          $"Type {GetType()} does not have mixin {type}"
          );
    }

    Metatype.MixinHandlers[type](this);
  }
}
