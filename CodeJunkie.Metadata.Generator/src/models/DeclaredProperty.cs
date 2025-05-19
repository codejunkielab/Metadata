namespace CodeJunkie.Metadata.Generator.Models;

using CodeJunkie.Metadata.Generator.Utils;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Represents a property on a metatype. Properties are opt-in and persisted.
/// </summary>
/// <param name="Name">Name of the property.</summary>
/// <param name="HasGetter">Indicates whether the property has a getter.</summary>
/// <param name="HasSetter">Indicates whether the property has a setter.</summary>
/// <param name="IsInit">Indicates whether the property is initialized-only.</summary>
/// <param name="IsRequired">Indicates whether the property is required.</summary>
/// <param name="IsNullable">Indicates whether the property is nullable.</summary>
/// <param name="DefaultValueExpression">Expression to use as the default value.</summary>
/// <param name="ExplicitInterfaceName">Explicit interface name, if any.</summary>
/// <param name="TypeNode">Type of the property.</summary>
/// <param name="Attributes">Attributes applied to the property.</summary>
public sealed record DeclaredProperty(string Name,
                                      bool HasGetter,
                                      bool HasSetter,
                                      bool IsInit,
                                      bool IsRequired,
                                      bool IsNullable,
                                      string? DefaultValueExpression,
                                      string? ExplicitInterfaceName,
                                      TypeNode TypeNode,
                                      ImmutableArray<DeclaredAttribute> Attributes) {
  /// <summary>
  /// Writes the property metadata to the specified writer.
  /// </summary>
  /// <param name="writer">The writer to output the metadata.</param>
  /// <param name="typeSimpleNameClosed">The closed type name of the property.</param>
  public void Write(IndentedTextWriter writer, string typeSimpleNameClosed) {
    writer.WriteLine($"new {Constants.PropertyMetadata}(");
    writer.Indent++;

    var propertyValue = "value" + (IsNullable ? "" : "!");
    var typeName = ExplicitInterfaceName ?? typeSimpleNameClosed;

    var getter = HasGetter
      ? $"static (object obj) => (({typeName})obj).{Name}"
      : "null";

    var type = TypeNode.ClosedType;

    var setter = HasSetter && !IsInit
      ? $"static (object obj, object? value) => (({typeName})obj).{Name} = ({type}){propertyValue}"
      : "null";

    writer.WriteLine($"Name: \"{Name}\",");
    writer.WriteLine($"IsInit: {(IsInit ? "true" : "false")},");
    writer.WriteLine($"IsRequired: {(IsRequired ? "true" : "false")},");
    writer.WriteLine($"HasDefaultValue: {(DefaultValueExpression is not null ? "true" : "false")},");
    writer.WriteLine($"Getter: {getter},");
    writer.WriteLine($"Setter: {setter},");
    writer.Write("TypeNode: ");
    TypeNode.Write(writer);
    writer.WriteLine(",");
    writer.WriteLine("Attributes: new System.Collections.Generic.Dictionary" + "<System.Type, System.Attribute[]>() {");

    DeclaredAttribute.WriteAttributeMap(writer, Attributes);

    writer.WriteLine("}");

    writer.Indent--;

    writer.Write(")");
  }

  /// <summary>
  /// Determines whether the specified <see cref="DeclaredProperty"/> is equal to the current instance.
  /// </summary>
  /// <param name="other">The other <see cref="DeclaredProperty"/> to compare.</param>
  /// <returns><c>true</c> if the properties are equal; otherwise, <c>false</c>.</returns>
  public bool Equals(DeclaredProperty? other) =>
    other is not null &&
    Name == other.Name &&
    HasGetter == other.HasGetter &&
    HasSetter == other.HasSetter &&
    IsInit == other.IsInit &&
    IsRequired == other.IsRequired &&
    IsNullable == other.IsNullable &&
    DefaultValueExpression == other.DefaultValueExpression &&
    TypeNode.Equals(other.TypeNode) &&
    Attributes.SequenceEqual(other.Attributes);

  /// <summary>
  /// Gets the hash code for the current instance.
  /// </summary>
  /// <returns>The hash code for the current instance.</returns>
  public override int GetHashCode() => HashCode.Combine(
      Name,
      HasGetter,
      HasSetter,
      IsInit,
      IsRequired,
      IsNullable,
      DefaultValueExpression,
      TypeNode,
      Attributes);
}
