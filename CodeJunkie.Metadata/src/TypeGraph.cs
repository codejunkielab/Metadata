namespace CodeJunkie.Metadata;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CodeJunkie.Collections;

internal class TypeGraph : ITypeGraph {
#region Caches
  private readonly ConcurrentDictionary<Type, ITypeMetadata> _types = new();
  private readonly ConcurrentDictionary<string, Dictionary<int, Type>> _identifiableTypesByIdAndVersion = new();
  private readonly ConcurrentDictionary<string, int> _identifiableLatestVersionsById = new();
  private readonly ConcurrentDictionary<Type, Set<Type>> _typesByBaseType = new();
  private readonly ConcurrentDictionary<Type, Set<Type>> _typesByAncestor = new();
  private readonly ConcurrentDictionary<Type, IEnumerable<PropertyMetadata>> _properties = new();
  private readonly ConcurrentDictionary<Type, Dictionary<Type, Attribute[]>> _attributes = new();
  private readonly IReadOnlySet<Type> _emptyTypeSet = new Set<Type>();
  private readonly IReadOnlyDictionary<Type, Attribute[]> _emptyAttributes = new Dictionary<Type, Attribute[]>();
#endregion Caches

  internal void Reset() {
    _types.Clear();
    _identifiableTypesByIdAndVersion.Clear();
    _identifiableLatestVersionsById.Clear();
    _typesByBaseType.Clear();
    _typesByAncestor.Clear();
    _properties.Clear();
    _attributes.Clear();
  }

#region ITypeGraph
  public IEnumerable<Type> IdentifiableTypes => _identifiableTypesByIdAndVersion
    .Values
    .SelectMany(versions => versions.Values);

  public void Register(ITypeRegistry registry) {
    RegisterTypes(registry);
    PromoteInheritedIdentifiableTypes(registry);
    ComputeTypesByBaseType(registry);
  }

  public int? GetLatestVersion(string id) =>
    _identifiableLatestVersionsById.TryGetValue(id, out var version)
    ? version
    : null;

  public Type? GetIdentifiableType(string id, int? version = null) =>
    ((version ?? GetLatestVersion(id)) is int actualVersion &&
     _identifiableTypesByIdAndVersion.TryGetValue(id, out var versions) &&
     versions.TryGetValue(actualVersion, out var type)) ? type : null;

  public bool HasMetadata(Type type) => _types.ContainsKey(type);

  public ITypeMetadata? GetMetadata(Type type) =>
    _types.TryGetValue(type, out var metadata) ? metadata : null;

  public IReadOnlySet<Type> GetSubtypes(Type type) =>
    _typesByBaseType.TryGetValue(type, out var subtypes)
    ? subtypes
    : _emptyTypeSet;

  public IReadOnlySet<Type> GetDescendantSubtypes(Type type) {
    CacheDescendants(type);
    return _typesByAncestor[type];
  }

  public IEnumerable<PropertyMetadata> GetProperties(Type type) {
    if (!_types.ContainsKey(type) ||
        _types[type] is not IIntrospectiveTypeMetadata metadata) {
      return [];
    }

    if (!_properties.TryGetValue(type, out var properties)) {
      _properties[type] =
        GetTypeAndBaseTypes(type)
        .Select(GetMetadata)
        .OfType<IIntrospectiveTypeMetadata>()
        .SelectMany(metadata => metadata.Metatype.Properties)
        .Distinct()
        .OrderBy(property => property.Name);
    }
    return _properties[type];
  }

  public TAttribute? GetAttribute<TAttribute>(Type type)
    where TAttribute : Attribute =>
    GetAttributes(type).TryGetValue(typeof(TAttribute), out var attributes) &&
    attributes is { Length: > 0 } &&
    attributes[0] is TAttribute attribute
    ? attribute
    : null;

  public IReadOnlyDictionary<Type, Attribute[]> GetAttributes(Type type) {
    if (!_types.ContainsKey(type) ||
        _types[type] is not IIntrospectiveTypeMetadata metadata) {
      return _emptyAttributes;
    }

    if (!_attributes.TryGetValue(type, out var attributes)) {
      _attributes[type] = GetTypeAndBaseTypes(type)
        .Select(type => _types[type])
        .OfType<IIntrospectiveTypeMetadata>()
        .SelectMany((metadata) => metadata.Metatype.Attributes)
        .GroupBy(
            kvp => kvp.Key,
            kvp => kvp.Value)
        .ToDictionary(
            group => group.Key,
            elementSelector: group => group
            .SelectMany(attributes => attributes)
            .ToArray());
    }
    return _attributes[type];
  }

  public void AddCustomType(Type type,
                            string name,
                            Action<ITypeReceiver> genericTypeGetter,
                            Func<object> factory,
                            string id,
                            int version = 1) =>
    RegisterType(
        type,
        new IdentifiableTypeMetadata(
          name,
          genericTypeGetter,
          factory,
          new EmptyMetatype(type),
          id,
          version
          ));

#endregion ITypeGraph

#region Private Utilities
  private void CacheDescendants(Type type) {
    if (_typesByAncestor.ContainsKey(type)) {
      return;
    }
    _typesByAncestor[type] = FindDescendants(type);
  }

  private Set<Type> FindDescendants(Type type) {
    var descendants = new Set<Type>();
    var queue = new Queue<Type>();
    queue.Enqueue(type);

    while (queue.Count > 0) {
      var currentType = queue.Dequeue();
      descendants.Add(currentType);

      if (_typesByBaseType.TryGetValue(currentType, out var children)) {
        foreach (var child in children) {
          queue.Enqueue(child);
        }
      }
    }

    descendants.Remove(type);

    return descendants;
  }

  private void RegisterTypes(ITypeRegistry registry) {
    foreach (var type in registry.VisibleTypes.Keys) {
      RegisterType(type, registry.VisibleTypes[type]);
    }
  }

  private void RegisterType(Type @type,
                            ITypeMetadata metadata,
                            bool overwrite = false) {
    _types[type] = metadata;

    if (metadata is IIdentifiableTypeMetadata identifiableTypeMetadata) {
      if (metadata is IConcreteIntrospectiveTypeMetadata introspectiveMetadata) {
        var id = identifiableTypeMetadata.Id;
        var version = introspectiveMetadata.Version;

        if (_identifiableTypesByIdAndVersion.TryGetValue(id, out var versions)) {
          if (!overwrite && versions.TryGetValue(version, out var existingType)) {
            throw new DuplicateNameException(
                $"Cannot register introspective type `{type}` with id `{id}` " +
                $"and version `{version}`. A different type with the same id " +
                $"and version has already been registered: {existingType}.");
          }
        }
        else {
          versions = [];
          _identifiableTypesByIdAndVersion[id] = versions;
        }

        versions[version] = type;

        if (_identifiableLatestVersionsById.TryGetValue(id, out var existingVersion)) {
          if (version > existingVersion) {
            _identifiableLatestVersionsById[id] = version;
          }
        }
        else {
          _identifiableLatestVersionsById[id] = version;
        }
      }
    }
  }

  private void PromoteInheritedIdentifiableTypes(ITypeRegistry registry) {
    foreach (var visibleType in registry.VisibleTypes.Keys) {
      var metadata = registry.VisibleTypes[visibleType];

      if (metadata is IIdentifiableTypeMetadata) {
        continue;
      }

      if (metadata is not IntrospectiveTypeMetadata introspectiveMetadata) {
        continue;
      }

      var version = introspectiveMetadata.Version;

      foreach (var type in GetTypeAndBaseTypes(visibleType).Skip(1)) {
        metadata = _types[type];

        if (metadata is not IIdentifiableTypeMetadata idMetadata) {
          continue;
        }

        metadata = new IdentifiableTypeMetadata(
            Name: introspectiveMetadata.Name,
            GenericTypeGetter: introspectiveMetadata.GenericTypeGetter,
            Factory: introspectiveMetadata.Factory,
            Metatype: introspectiveMetadata.Metatype,
            Id: idMetadata.Id,
            Version: version);

        RegisterType(visibleType, metadata, overwrite: true);

        break;
      }
    }
  }

  private void ComputeTypesByBaseType(ITypeRegistry registry) {
    foreach (var type in registry.VisibleTypes.Keys) {
      var lastType = type;
      var baseType = type.BaseType;

      if (!_typesByBaseType.ContainsKey(type)) {
        _typesByBaseType[type] = [];
      }

      while (baseType != null) {
        if (!_typesByBaseType.TryGetValue(baseType, out var existingSet)) {
          existingSet = [];
          _typesByBaseType[baseType] = existingSet;
        }
        existingSet.Add(lastType);

        lastType = baseType;
        baseType = lastType.BaseType;
      }
    }
  }

  /// <summary>
  /// Enumerates through a type and its base type hierarchy to discover all
  /// metatypes that describe the type and its base types.
  /// </summary>
  /// <param name="type">Type whose type hierarchy should be examined.</param>
  /// <returns>The type's metatype (if it has one), and any metatypes that
  /// describe its base types, in the order of the most derived type to the
  /// least derived type.</returns>
  private IEnumerable<Type> GetTypeAndBaseTypes(Type type) {
    var currentType = type;

    do {
      if (_types.ContainsKey(currentType) &&
          _types[currentType] is IIntrospectiveTypeMetadata) {
        yield return currentType;
      }

      currentType = currentType.BaseType;
    } while (currentType != null);
  }

  internal class EmptyMetatype(Type type) : IMetatype {
    private static readonly List<PropertyMetadata> _properties = [];
    private static readonly Dictionary<Type, Attribute[]> _attributes = [];
    private static readonly List<Type> _mixins = [];
    private static readonly Dictionary<Type, Action<object>> _mixinHandlers = [];

    public Type Type => type;
    public bool HasInitProperties => false;
    public IReadOnlyList<PropertyMetadata> Properties => _properties;
    public IReadOnlyDictionary<Type, Attribute[]> Attributes => _attributes;
    public IReadOnlyList<Type> Mixins => _mixins;
    public IReadOnlyDictionary<Type, Action<object>> MixinHandlers => _mixinHandlers;

    public object Construct(IReadOnlyDictionary<string, object?>? args = null) =>
      throw new NotImplementedException();

    public override bool Equals(object obj) => true;
    public override int GetHashCode() => base.GetHashCode();
  }

#endregion Private Utilities
}
