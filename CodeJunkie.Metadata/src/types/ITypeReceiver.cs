namespace CodeJunkie.Metadata;

/// <summary>
/// Interface for receiving a single generic type argument.
/// </summary>
public interface ITypeReceiver {
  /// <summary>
  /// Receives a single generic type argument.
  /// </summary>
  /// <typeparam name="T">The generic type.</typeparam>
  void Receive<T>();
}

/// <summary>
/// Interface for receiving two generic type arguments.
/// </summary>
public interface ITypeReceiver2 {
  /// <summary>
  /// Receives two generic type arguments.
  /// </summary>
  /// <typeparam name="TA">The first generic type.</typeparam>
  /// <typeparam name="TB">The second generic type.</typeparam>
  void Receive<TA, TB>();
}
