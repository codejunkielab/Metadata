namespace CodeJunkie.Metadata.Generator.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a hierarchical structure of namespaces and types, allowing for efficient type resolution and lookup.
/// </summary>
public class ScopeTree {
  /// <summary>
  /// The root node of the namespace tree.
  /// </summary>
  public NamespaceNode Root { get; } = new NamespaceNode(null, "", [], []);

  /// <summary>
  /// A dictionary mapping the full, open generic names of types to their corresponding DeclaredType objects.
  /// </summary>
  /// <summary>
  /// A dictionary containing all types indexed by their full, open generic names.
  /// </summary>
  public IDictionary<string, DeclaredType> TypesByFullNameOpen { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScopeTree"/> class with the specified types and type mappings.
  /// </summary>
  /// <param name="types">A collection of DeclaredType objects to populate the tree.</param>
  /// <param name="typesByFullNameOpen">A dictionary mapping full, open generic names to DeclaredType objects.</param>
  public ScopeTree(IEnumerable<DeclaredType> types,
                   IDictionary<string, DeclaredType> typesByFullNameOpen) {
    TypesByFullNameOpen = typesByFullNameOpen;
    InitializeTree(types);
  }

  /// <summary>
  /// Retrieves all types visible from the top-level scope that match the specified predicate.
  /// Get all types visible from the top level scope that match the given
  /// predicate by searching the scope tree recursively.
  /// </summary>
  /// <param name="predicate">Predicate that a type must satisfy.</param>
  /// <param name="searchGenericTypes">Whether to search generic types (default
  /// is true).</param>
  /// <param name="searchPrivateTypes">Whether to search types that are not
  /// visible from the global scope (default is false).</param>
  /// <returns>Enumeration of declared types matching the predicate.</returns>
  public IEnumerable<DeclaredType> GetTypes(Func<TypeTreeNode, bool>? predicate = null,
                                            bool searchGenericTypes = true,
                                            bool searchPrivateTypes = false) =>
    GetTypes(
        node: Root,
        predicate: predicate ?? (_ => true),
        generic: searchGenericTypes,
        @private: searchPrivateTypes)
    .OrderBy(t => t.FullNameOpen);

  private IEnumerable<DeclaredType> GetTypes(ScopeNode node,
                                             Func<TypeTreeNode, bool> predicate,
                                             bool generic = true,
                                             bool @private = false) {
    if (node is NamespaceNode nsNode) {
      foreach (var childNs in nsNode.Children.Values) {
        foreach (var type in GetTypes(childNs, predicate, generic, @private)) {
          yield return type;
        }
      }

      foreach (var child in node.TypeChildren.Values) {
        foreach (var type in GetTypes(child, predicate, generic, @private)) {
          yield return type;
        }
      }
    }

    if (node is not TypeTreeNode typeNode) {
      yield break;
    }

    if (typeNode.Type.IsGeneric && !generic) {
      yield break;
    }

    if (!typeNode.Type.IsPublicOrInternal && !@private) {
      yield break;
    }

    if (predicate(typeNode)) {
      yield return typeNode.Type;
    }

    foreach (var child in node.TypeChildren.Values) {
      foreach (var type in GetTypes(child, predicate, generic)) {
        yield return type;
      }
    }
  }

  /// <summary>
  /// Attempts to resolve a reference to a type relative to the scope of the
  /// given type. For example, this can take a relative type reference for
  /// a base class and determine the fully qualified name of the base class and
  /// return that type.
  /// </summary>
  /// <param name="globalUsings">Global using directives across the project.
  /// </param>
  /// <param name="type">Type whose scope should be searched.</param>
  /// <param name="typeReference">Relative or fully qualified reference to a
  /// type in that scope.</param>
  /// <returns>The declared type the reference is referring to, or null if
  /// the reference could not be resolved.</returns>
  public DeclaredType? ResolveTypeReference(IEnumerable<UsingDirective> globalUsings,
                                            DeclaredType type,
                                            string reference) {
    reference = reference.Replace("global::", "");
    var referenceParts = reference.Split('.');

    var nodes = new LinkedList<ScopeNode>();

    var aliasedTypes = new Dictionary<string, TypeTreeNode>();

    foreach (var containingType in GetContainingTypes(type)) {
      if (GetNode(containingType) is { } node) {
        nodes.AddFirst(node);
      }
    }

    if (GetNode(type.Location.Namespace) is { } nsNode) {
      nodes.AddLast(nsNode);
    }

    foreach (var @using in type.Usings.Union(globalUsings)) {
      if (GetNode(@using.Name) is not { } node) {
        continue;
      }

      if (@using.Alias is { } alias && node is TypeTreeNode typeNode) {
        aliasedTypes[alias] = typeNode;
        continue;
      }

      nodes.AddLast(node);
    }

    foreach (var alias in aliasedTypes.Keys) {
      if (GetTypeByAliasReference(
            reference: reference,
            alias: alias,
            aliasedTypeNode: aliasedTypes[alias]) is { } aliasedType) {
        return aliasedType;
      }
    }

    foreach (var node in nodes) {
      var candidate = GetTypes(
          node,
          predicate:
          n => FullNameMatchesReferenceParts(n.Type.FullNameOpen, referenceParts),
          generic: false,
          @private: false)
        .FirstOrDefault();

      if (candidate is not null) {
        return candidate;
      }
    }

    if (TypesByFullNameOpen.TryGetValue(reference, out var qualifiedType)) {
      return qualifiedType;
    }

    return null;
  }

  private void InitializeTree(IEnumerable<DeclaredType> typesByFullNameOpen) {
    foreach (var declaredType in typesByFullNameOpen) {
      AddType(declaredType);
    }
  }

  private void AddType(DeclaredType type) {
    var currentNs = Root;
    foreach (var @namespace in type.Location.Namespaces) {
      if (!currentNs.Children.TryGetValue(@namespace, out var childNs)) {
        childNs = new NamespaceNode(
            Parent: currentNs,
            Name: @namespace,
            Children: [],
            TypeChildren: []);

        currentNs.Children.Add(key: @namespace, value: childNs);
      }

      currentNs = currentNs.Children[@namespace];
    }

    ScopeNode current = currentNs;

    foreach (var containingType in GetContainingTypes(type)) {
      if (!current.TypeChildren.TryGetValue(
            containingType.Reference.SimpleNameOpen, out var child)) {
        child = new TypeTreeNode(
            Parent: current,
            Type: containingType,
            TypeChildren: []);

        current.TypeChildren.Add(
            key: containingType.Reference.SimpleNameOpen,
            value: child);
      }

      current = child;
    }

    if (current.TypeChildren.ContainsKey(type.Reference.SimpleNameOpen)) {
      return;
    }

    var typeNode = new TypeTreeNode(
        Parent: current, Type: type, TypeChildren: []);

    current.TypeChildren.Add(
        key: type.Reference.SimpleNameOpen,
        value: typeNode);
  }

  private DeclaredType? GetTypeByAliasReference(string reference,
                                                string alias,
                                                TypeTreeNode aliasedTypeNode) {
    if (reference == alias) {
      return aliasedTypeNode.Type;
    }

    if (reference.StartsWith(alias + ".") &&
        reference.Substring(alias.Length + 1) is { } path &&
        aliasedTypeNode.Type.FullNameOpen + "." + path is { } fullPath &&
        TypesByFullNameOpen.TryGetValue(fullPath, out var aliasedType)) {
      return aliasedType;
    }

    return null;
  }

  private ScopeNode? GetNode(DeclaredType type) => GetNode(type.FullNameOpen);

  private ScopeNode? GetNode(string fullNameOpen) => GetNode(new Queue<string>(fullNameOpen.Split('.')));

  private ScopeNode? GetNode(Queue<string> nameParts) {
    ScopeNode current = Root;

    while (nameParts.Count > 0 &&
           current is NamespaceNode nsNode &&
           nsNode.Children.TryGetValue(nameParts.Peek(), out var next)) {
      nameParts.Dequeue();
      current = next;
    }

    while (nameParts.Count > 0 &&
           current.TypeChildren.TryGetValue(nameParts.Peek(), out var next)) {
      nameParts.Dequeue();
      current = next;
    }

    return nameParts.Count == 0 ? current : null;
  }

  private IEnumerable<DeclaredType> GetContainingTypes(DeclaredType type) {
    var containingTypes = new StringBuilder();
    foreach (var containingTypeRef in type.Location.ContainingTypes) {
      containingTypes.Append(containingTypeRef.SimpleNameOpen);

      var fullNameOpen =
        (string.IsNullOrEmpty(type.Location.Namespace)
         ? ""
         : type.Location.Namespace + ".") + containingTypes.ToString();

      containingTypes.Append(".");

      if (!TypesByFullNameOpen.TryGetValue(fullNameOpen, out var containingType)) {
        throw new InvalidOperationException(
            "Could not find the containing type " +
            $"`{fullNameOpen}` of `{type.FullNameOpen}`.");
      }

      yield return containingType;
    }
  }

  private bool FullNameMatchesReferenceParts(string fullName,
                                             IList<string> referenceParts) {
    var fullNameParts = fullName.Split('.');

    if (fullNameParts.Length < referenceParts.Count) {
      return false;
    }

    for (var i = referenceParts.Count - 1; i >= 0; i--) {
      if (referenceParts[i] != fullNameParts[fullNameParts.Length - referenceParts.Count + i]) {
        return false;
      }
    }

    return true;
  }
}
