namespace CodeJunkie.Metadata.Generator.Utils;

using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides extension methods for working with TypeSyntax objects in Roslyn.
/// These methods help in identifying and manipulating nullable types.
/// </summary>
public static class Extensions {
  /// <summary>
  /// Determines whether the given TypeSyntax represents a nullable type.
  /// </summary>
  /// <param name="type">The TypeSyntax to check.</param>
  /// <returns>True if the type is nullable; otherwise, false.</returns>
  public static bool IsNullable(this TypeSyntax type) {
    return type is NullableTypeSyntax ||
      (type is GenericNameSyntax generic &&
       generic.Identifier.ValueText == "Nullable");
  }

  /// <summary>
  /// Unwraps a nullable type to retrieve its underlying type.
  /// </summary>
  /// <param name="type">The TypeSyntax to unwrap.</param>
  /// <returns>The underlying type if the type is nullable; otherwise, the original type.</returns>
  public static TypeSyntax UnwrapNullable(this TypeSyntax type) {
    return type switch {
      NullableTypeSyntax nullable =>
        nullable.ElementType,
        GenericNameSyntax generic
          when generic.Identifier.ValueText == "Nullable" =>
          generic.TypeArgumentList.Arguments.First(),
        _ => type};
  }
}
