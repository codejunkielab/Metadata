namespace CodeJunkie.Metadata;

/// <summary>
/// Provides a system for caching and looking up types based on their hierarchy.
/// </summary>
public static class Types {
  /// <summary>
  /// Shared instance of the type graph for hierarchy lookups.
  /// </summary>
  public static ITypeGraph Graph => InternalGraph;

  /// <summary>
  /// Internal type graph instance for managing type relationships.
  /// </summary>
  internal static TypeGraph InternalGraph { get; } = new TypeGraph();
}
