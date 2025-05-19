namespace CodeJunkie.Metadata;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents metadata for a property, including its characteristics and accessors.
/// </summary>
/// <param name="Name">The property's name.</param>
/// <param name="IsInit">True if the property is init-only; otherwise, false.</param>
/// <param name="IsRequired">True if the property is required; otherwise, false.</param>
/// <param name="HasDefaultValue">True if the property has a default value; otherwise, false.</param>
/// <param name="Getter">A function to retrieve the property's value.</param>
/// <param name="Setter">A function to assign a value to the property.</param>
/// <param name="TypeNode">The root node of the generic type tree for closed constructed generic types.</param>
/// <param name="Attributes">A dictionary mapping attribute types to their instances.</param>
public sealed record PropertyMetadata(string Name,
                                      bool IsInit,
                                      bool IsRequired,
                                      bool HasDefaultValue,
                                      Func<object, object?>? Getter,
                                      Action<object, object?>? Setter,
                                      TypeNode TypeNode,
                                      Dictionary<Type, Attribute[]> Attributes);
