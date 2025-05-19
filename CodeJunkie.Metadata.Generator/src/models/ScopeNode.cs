namespace CodeJunkie.Metadata.Generator.Models;

using System.Collections.Generic;

/// <summary>
/// Represents a node in the type name resolution tree, which can be a namespace or a type.
/// </summary>
/// <param name="Parent">The parent node in the tree. Null if this is the root node.</param>
/// <param name="Name">The name of the namespace or type represented by this node.</param>
/// <param name="TypeChildren">A dictionary mapping type names to their corresponding type nodes.</param>
public abstract record ScopeNode(ScopeNode? Parent,
                                 string Name,
                                 Dictionary<string, TypeTreeNode> TypeChildren);

/// <summary>
/// Represents a type node in the type name resolution tree.
/// </summary>
/// <param name="Parent">The parent node in the tree. Null if this is the root node.</param>
/// <param name="Type">The declared type associated with this node.</param>
/// <param name="TypeChildren">A dictionary mapping type names to their corresponding type nodes.</param>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed record TypeTreeNode(ScopeNode? Parent,
                                  DeclaredType Type,
                                  Dictionary<string, TypeTreeNode> TypeChildren)
  : ScopeNode(Parent, Type.Reference.SimpleNameOpen, TypeChildren);

/// <summary>
/// Represents a namespace node in the type name resolution tree.
/// </summary>
/// <param name="Parent">The parent node in the tree. Null if this is the root node.</param>
/// <param name="Name">The name of the namespace represented by this node.</param>
/// <param name="Children">A dictionary mapping child namespace names to their corresponding namespace nodes.</param>
/// <param name="TypeChildren">A dictionary mapping type names to their corresponding type nodes.</param>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed record NamespaceNode(ScopeNode? Parent,
                                   string Name,
                                   Dictionary<string, NamespaceNode> Children,
                                   Dictionary<string, TypeTreeNode> TypeChildren)
  : ScopeNode(Parent, Name, TypeChildren);
