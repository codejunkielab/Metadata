namespace CodeJunkie.Metadata.Generator.Models;

using CodeJunkie.Metadata.Generator.Utils;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Represents a metadata attribute applied to a property or member.
/// This class encapsulates the attribute's name, constructor arguments,
/// and initializer arguments, providing functionality to write the attribute
/// in a formatted manner.
/// </summary>
/// <param name="Name">The name of the attribute, typically corresponding to the attribute's type.</param>
/// <param name="ConstructorArgs">A collection of arguments passed to the attribute's constructor.</param>
/// <param name="InitializerArgs">A collection of properties set using object initializer syntax,
/// which are not part of the constructor signature.</param>
public sealed record DeclaredAttribute(string Name,
                                       ImmutableArray<string> ConstructorArgs,
                                       ImmutableArray<string> InitializerArgs) {
  /// <summary>
  /// Writes a mapping of attribute names to their corresponding attribute instances
  /// into the provided <see cref="IndentedTextWriter"/>.
  /// </summary>
  /// <param name="writer">The writer to output the attribute map.</param>
  /// <param name="attributeUsages">A collection of declared attributes to be mapped and written.</param>
  public static void WriteAttributeMap(IndentedTextWriter writer,
                                       ImmutableArray<DeclaredAttribute> attributeUsages) {
    var attributesByName = attributeUsages
      .GroupBy(attr => attr.Name)
      .ToDictionary(
          group => group.Key,
          group => group.ToImmutableArray());

    writer.WriteCommaSeparatedList(
        attributesByName.Keys.OrderBy(a => a), // Sort for deterministic output.
        (attributeName) => {
          var attributes = attributesByName[attributeName];

          writer.WriteLine($"[typeof({attributeName}Attribute)] = new System.Attribute[] {{");

          writer.WriteCommaSeparatedList(
              attributes, // Respect the order they were applied.
              (attribute) => attribute.Write(writer),
              multiline: true
              );

          writer.Write("}");
        },
        multiline: true);
  }

  public bool Equals(DeclaredAttribute? other) =>
    other is not null &&
    Name == other.Name &&
    ConstructorArgs.SequenceEqual(other.ConstructorArgs) &&
    InitializerArgs.SequenceEqual(other.InitializerArgs);

  private void Write(IndentedTextWriter writer) {
    writer.Write($"new {Name}Attribute(");
    writer.Write(string.Join(", ", ConstructorArgs));
    writer.Write(")");
    if (InitializerArgs.Length > 0) {
      writer.Write(" { ");
      writer.Write(string.Join(", ", InitializerArgs));
      writer.Write(" }");
    }
  }

  public override int GetHashCode() => HashCode.Combine(
      Name,
      ConstructorArgs,
      InitializerArgs);
}
