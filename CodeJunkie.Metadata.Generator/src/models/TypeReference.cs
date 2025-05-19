namespace CodeJunkie.Metadata.Generator.Models;

using System;
using System.Collections.Immutable;
using System.Linq;
using CodeJunkie.Metadata.Generator.Utils;

public sealed record TypeReference(string SimpleName,
                                   Construction Construction,
                                   bool IsPartial,
                                   ImmutableArray<string> TypeParameters) {
  public static string GetGenerics(ImmutableArray<string> typeParameters) =>
    typeParameters.Length > 0
    ? $"<{string.Join(", ", typeParameters)}>"
    : string.Empty;

  public static string GetOpenGenerics(int numTypeParameters) =>
    numTypeParameters > 0
    ? $"<{new string(',', numTypeParameters - 1)}>"
    : string.Empty;

  /// <summary>
  /// Generates the code string required to declare the type.
  /// </summary>
  /// <param name="isPartial">Indicates whether the type is partial.</param>
  /// <param name="construction">The construction type of the type (e.g., class, struct).</param>
  /// <param name="name">The name of the type, including any generic parameters.</param>
  /// <returns>A string representing the type declaration code.</returns>
  public static string GetConstructionCodeString(bool isPartial,
                                                 Construction construction,
                                                 string name) {
    var partial = isPartial ? "partial " : string.Empty;
    var code = construction switch {
      Construction.StaticClass => $"static {partial}class ",
        Construction.Class => $"{partial}class ",
        Construction.RecordStruct => $"{partial}record struct ",
        Construction.RecordClass => $"{partial}record class ",
        Construction.Interface => $"{partial}interface ",
        Construction.Struct => $"{partial}struct ",
        _ => throw new ArgumentException($"Unsupported construction type: {construction}", nameof(construction))
    };

    return code + name;
  }

  /// <summary>
  /// Open generics portion of the type name (if generic). Otherwise, blank string.
  /// </summary>
  public string OpenGenerics => GetOpenGenerics(TypeParameters.Length);

  /// <summary>
  /// Gets the name of the type, including the open generics portion if the type is generic.
  /// </summary>
  public string SimpleNameOpen => SimpleName + OpenGenerics;

  /// <summary>
  /// Gets the name of the type, including all generic type parameters.
  /// </summary>
  public string SimpleNameClosed => SimpleName + GetGenerics(TypeParameters);

  /// <summary>
  /// Indicates whether the type is generic.
  /// </summary>
  public bool IsGeneric => TypeParameters.Length > 0;

  public TypeReference MergePartialDefinition(TypeReference reference) =>
    new(SimpleName,
        Construction,
        IsPartial || reference.IsPartial,
        TypeParameters);

  public string CodeString => GetConstructionCodeString(IsPartial, Construction, SimpleNameClosed
      );

  public bool Equals(TypeReference? other) =>
    other is not null &&
    SimpleName == other.SimpleName &&
    Construction == other.Construction &&
    IsPartial == other.IsPartial &&
    TypeParameters.SequenceEqual(other.TypeParameters);

  public override int GetHashCode() => HashCode.Combine(
      SimpleName,
      Construction,
      IsPartial,
      TypeParameters);
}
