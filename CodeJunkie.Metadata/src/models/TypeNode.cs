namespace CodeJunkie.Metadata;

using System;

/// <summary>
/// Represents metadata for a type, including its generic and nullable characteristics.
/// </summary>
/// <param name="OpenType">The base type, or the same as <see cref="ClosedType"/> if not generic.</param>
/// <param name="ClosedType">The specific type, or the plain type if not generic.</param>
/// <param name="IsNullable">True if the type is nullable; otherwise, false.</param>
/// <param name="Arguments">An array of type arguments, if any.</param>
/// <param name="GenericTypeGetter">An action to process the type with a generic type receiver.</param>
/// <param name="GenericTypeGetter2">An action to process the type with two child type arguments, applicable only to types with exactly two type parameters.</param>
public record TypeNode(Type OpenType,
                       Type ClosedType,
                       bool IsNullable,
                       TypeNode[] Arguments,
                       Action<ITypeReceiver> GenericTypeGetter,
                       Action<ITypeReceiver2>? GenericTypeGetter2);
