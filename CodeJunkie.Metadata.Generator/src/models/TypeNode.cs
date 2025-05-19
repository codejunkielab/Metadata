namespace CodeJunkie.Metadata.Generator.Models;

using System;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;
using CodeJunkie.Metadata.Generator.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Represents a generic type as a hierarchy of all its constituent types.
/// Note: This is distinct from the type resolution tree node, which deals with type locations.
/// </summary>
public sealed record TypeNode(string Type,
                              bool IsNullable,
                              ImmutableArray<TypeNode> Children) {
  /// <summary>
  /// Gets the name of the type, including any generic type arguments (i.e., the closed generic type).
  /// </summary>
  public string ClosedType =>
    Type + TypeReference.GetGenerics(
        Children.Select(child => child.ClosedType).ToImmutableArray()) + Q;

  /// <summary>
  /// Gets the name of the type with open generic arguments.
  /// </summary>
  public string OpenType => Type + TypeReference.GetOpenGenerics(Children.Length) + Q;

  private string Q => IsNullable ? "?" : "";

  /// <summary>
  /// Recursively constructs a generic type node from a given type syntax.
  /// </summary>
  /// <param name="typeSyntax">The syntax representing the type.</param>
  /// <param name="isNullable">Indicates whether the type is nullable.</param>
  /// <returns>A <see cref="TypeNode"/> representing the generic type hierarchy.</returns>
  public static TypeNode Create(TypeSyntax typeSyntax, bool isNullable) {
    isNullable = isNullable || typeSyntax.IsNullable();
    typeSyntax = typeSyntax.UnwrapNullable();

    if (typeSyntax is not GenericNameSyntax genericNameSyntax) {
      return new TypeNode(
        typeSyntax.NormalizeWhitespace().ToString(),
        IsNullable: isNullable,
        Children: ImmutableArray<TypeNode>.Empty);
    }

    var type = genericNameSyntax.Identifier.NormalizeWhitespace().ToString();

    var children = genericNameSyntax.TypeArgumentList.Arguments
      .Select(
          arg => {
            typeSyntax = typeSyntax.UnwrapNullable();
            isNullable = arg.IsNullable();

            return Create(arg, isNullable);
          })
      .ToImmutableArray();

    return new TypeNode(type, isNullable, children);
  }

  /// <summary>
  /// Writes the type node as a formatted string using an <see cref="IndentedTextWriter"/>.
  /// </summary>
  /// <param name="writer">The writer to output the formatted string.</param>
  public void Write(IndentedTextWriter writer) {
    writer.WriteLine("new CodeJunkie.Metadata.TypeNode(");
    writer.Indent++;
    writer.WriteLine($"OpenType: typeof({OpenType.TrimEnd('?')}),");
    writer.WriteLine($"ClosedType: typeof({ClosedType.TrimEnd('?')}),");
    writer.WriteLine($"IsNullable: {(IsNullable ? "true" : "false")},");

    if (Children.Length > 0) {
      writer.WriteLine("Arguments: new TypeNode[] {");
      writer.Indent++;

      writer.WriteCommaSeparatedList(
        Children,
        child => child.Write(writer),
        multiline: true);

      writer.Indent--;
      writer.WriteLine("},");
    }
    else {
      writer.WriteLine("Arguments: System.Array.Empty<TypeNode>(),");
    }

    writer.WriteLine(
      "GenericTypeGetter: static receiver => " +
      $"receiver.Receive<{ClosedType}>(),");
    if (Children.Length >= 2) {
      writer.WriteLine(
        "GenericTypeGetter2: static receiver => " +
        $"receiver.Receive<{Children[0].ClosedType}, {Children[1].ClosedType}>()");
    }
    else {
      writer.WriteLine("GenericTypeGetter2: default");
    }
    writer.Indent--;
    writer.Write(")");
  }

  /// <summary>
  /// Determines whether the current <see cref="TypeNode"/> is equal to another instance.
  /// </summary>
  /// <param name="other">The other <see cref="TypeNode"/> to compare with.</param>
  /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
  public bool Equals(TypeNode? other) =>
    other is not null &&
    Type == other.Type &&
    IsNullable == other.IsNullable &&
    Children.SequenceEqual(other.Children);

  /// <summary>
  /// Gets the hash code for the current <see cref="TypeNode"/>.
  /// </summary>
  /// <returns>The hash code of the instance.</returns>
  public override int GetHashCode() => HashCode.Combine(Type, IsNullable, Children);
}
