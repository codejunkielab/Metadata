namespace CodeJunkie.Metadata.Generator.Utils;

using System.Text.RegularExpressions;

/// <summary>
/// Provides utility methods and constants for working with C# code constructs, such as `nameof` expressions.
/// </summary>
public static class Code {
  /// <summary>
  /// Regular expression pattern for parsing `nameof()` expressions. Group 1 captures the desired value.
  /// </summary>
  public const string NAME_OF = @"(?:(?<=\.)?([^.<>\n]*)(?:<[^.\n]+>)?(?=$))";

  /// <summary>
  /// Compiled regular expression for matching `nameof()` expressions.
  /// </summary>
  public static Regex NameOfRegex { get; set; } = new Regex(NAME_OF);

  /// <summary>
  /// Extracts the equivalent value that a `nameof()` expression would produce from a given string.
  /// </summary>
  /// <param name="input">The input string, which may or may not include a `nameof()` expression.</param>
  /// <returns>The extracted value if the input contains a valid `nameof()` expression; otherwise, the original input.</returns>
  public static string NameOf(string input) {
    var text = input.StartsWith("nameof(") ? input.Substring(7, input.Length - 8) : input;

    var match = NameOfRegex.Match(text);

    return match.Success ? match.Groups[1].Value : text;
  }
}
