namespace CodeJunkie.Metadata.Generator.Models;

using CodeJunkie.Metadata.Generator.Utils;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Linq;

/// <summary>
/// Represents a registry for declared types, including global usings, scope tree, and type metadata.
/// </summary>
public class DeclaredTypeRegistry {
  /// <summary>
  /// Gets the global using directives available in the registry.
  /// </summary>
  public ImmutableArray<UsingDirective> GlobalUsings { get; init; }

  /// <summary>
  /// Gets the scope tree that organizes the declared types hierarchically.
  /// </summary>
  public ScopeTree ScopeTree { get; init; }

  /// <summary>
  /// Gets all declared types in the registry, indexed by their unique names.
  /// </summary>
  public ImmutableDictionary<string, DeclaredType> AllTypes { get; init; }

  /// <summary>
  /// Gets the set of declared types that are visible and accessible.
  /// </summary>
  public ImmutableHashSet<DeclaredType> VisibleTypes { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DeclaredTypeRegistry"/> class.
  /// </summary>
  /// <param name="globalUsings">The global using directives.</param>
  /// <param name="scopeTree">The scope tree for organizing types.</param>
  /// <param name="allTypes">All declared types in the registry.</param>
  /// <param name="visibleTypes">The set of visible types.</param>
  public DeclaredTypeRegistry(ImmutableArray<UsingDirective> globalUsings,
                              ScopeTree scopeTree,
                              ImmutableDictionary<string, DeclaredType> allTypes,
                              ImmutableHashSet<DeclaredType> visibleTypes) {
    GlobalUsings = globalUsings;
    ScopeTree = scopeTree;
    AllTypes = allTypes;
    VisibleTypes = visibleTypes;
  }

  /// <summary>
  /// Computes the hash code for the current instance.
  /// </summary>
  /// <returns>The computed hash code.</returns>
  public override int GetHashCode() =>
    HashCode.Combine(GlobalUsings, ScopeTree, AllTypes, VisibleTypes);

  /// <summary>
  /// Determines whether the specified object is equal to the current instance.
  /// </summary>
  /// <param name="obj">The object to compare with the current instance.</param>
  /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
  public override bool Equals(object? obj) =>
    obj is DeclaredTypeRegistry data &&
    GlobalUsings.SequenceEqual(data.GlobalUsings) &&
    AllTypes.SequenceEqual(data.AllTypes) &&
    VisibleTypes.SequenceEqual(data.VisibleTypes);

  /// <summary>
  /// Writes the type registry to the specified <see cref="IndentedTextWriter"/>.
  /// </summary>
  /// <param name="writer">The writer to output the type registry.</param>
  public void Write(IndentedTextWriter writer) {
    writer.WriteLine(
      "public partial class TypeRegistry : " +
      $"{Constants.TypeRegistryInterface} {{");

    writer.Indent++;
    writer.WriteLine(
      $"public static {Constants.TypeRegistryInterface} Instance " +
      "{ get; } = new TypeRegistry();");
    writer.WriteLine();

    // Visible types property
    writer.WriteLine(
      "public System.Collections.Generic.IReadOnlyDictionary" +
      "<System.Type, CodeJunkie.Metadata.ITypeMetadata> " +
      "VisibleTypes { get; } = new System.Collections.Generic.Dictionary" +
      "<System.Type, CodeJunkie.Metadata.ITypeMetadata>() {");

    writer.Indent++;
    writer.WriteCommaSeparatedList(
      VisibleTypes
        .Where(
          type => type.Kind is
          DeclaredTypeKind.AbstractType or
          DeclaredTypeKind.ConcreteType)
        .OrderBy(type => type.FullNameOpen),
      (type) => {
        var knownToBeAccessibleFromGlobalScope = VisibleTypes.Contains(type);
        writer.Write($"[typeof({type.FullNameOpen})] = ");
        type.WriteMetadata(writer, knownToBeAccessibleFromGlobalScope);
      },
      multiline: true
    );
    writer.Indent--;
    writer.WriteLine("};");

    writer.WriteLine();

    // Module initializer that automatically registers types.
    writer.WriteLine("[System.Runtime.CompilerServices.ModuleInitializer]");
    writer.WriteLine(
      "internal static void Initialize() => " +
      $"{Constants.TypesGraph}.Register(TypeRegistry.Instance);");

    writer.Indent--;
    writer.WriteLine("}");

    writer.WriteLine();
  }
}
