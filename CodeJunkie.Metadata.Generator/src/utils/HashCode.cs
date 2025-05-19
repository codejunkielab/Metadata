namespace CodeJunkie.Metadata.Generator.Utils;

/// <summary>
/// Provides utility methods for generating hash codes.
/// This class is particularly useful for combining multiple objects into a single hash code,
/// especially in environments where System.HashCode is not available (e.g., netstandard2.0).
/// </summary>
public static class HashCode {
  private const int _seed = 1009;
  private const int _factor = 9176;

  /// <summary>
  /// Combines the hash codes of multiple objects into a single hash code.
  /// </summary>
  /// <param name="values">An array of objects whose hash codes will be combined.</param>
  /// <returns>A single hash code representing the combination of the input objects.</returns>
  /// <remarks>
  /// This implementation is based on a method described in https://stackoverflow.com/a/34006336.
  /// It uses a seed and a multiplication factor to generate a combined hash code.
  /// </remarks>
  public static int Combine(params object?[] values) {
    var hash = _seed;
    foreach (var obj in values) {
      hash = (hash * _factor) + (obj is null ? 0 : obj.GetHashCode());
    }
    return hash;
  }
}
