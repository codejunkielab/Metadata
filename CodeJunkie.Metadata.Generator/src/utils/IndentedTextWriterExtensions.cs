namespace CodeJunkie.Metadata.Generator.Utils;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

/// <summary>
/// Provides extension methods for the <see cref="IndentedTextWriter"/> class,
/// enabling additional functionality such as writing comma-separated lists.
/// </summary>
public static class IndentedTextWriterExtensions {
  /// <summary>
  /// Writes a comma-separated list of items to the <see cref="IndentedTextWriter"/>.
  /// </summary>
  /// <typeparam name="T">The type of the items in the list.</typeparam>
  /// <param name="writer">The <see cref="IndentedTextWriter"/> to write to.</param>
  /// <param name="items">The collection of items to write.</param>
  /// <param name="writeItem">An action that defines how to write each item.</param>
  /// <param name="multiline">
  /// If <c>true</c>, each item is written on a new line with proper indentation.
  /// If <c>false</c>, all items are written on the same line.
  /// </param>
  /// <remarks>
  /// This method is useful for generating formatted code or text where items need to be
  /// separated by commas, optionally spanning multiple lines for better readability.
  /// </remarks>
  /// <example>
  /// <code>
  /// var writer = new IndentedTextWriter(Console.Out, "    ");
  /// var items = new[] { "Item1", "Item2", "Item3" };
  /// writer.WriteCommaSeparatedList(items, item => writer.Write(item), multiline: true);
  /// </code>
  /// </example>
  public static void WriteCommaSeparatedList<T>(this IndentedTextWriter writer,
                                                IEnumerable<T> items,
                                                Action<T> writeItem,
                                                bool multiline = false) {
    if (multiline) {
      writer.Indent++;
    }

    var enumerator = items.GetEnumerator();
    if (!enumerator.MoveNext()) {
      if (multiline) {
        writer.Indent--;
      }

      return;
    }

    writeItem(enumerator.Current);
    while (enumerator.MoveNext()) {
      writer.Write(", ");
      if (multiline) {
        writer.WriteLine();
      }
      writeItem(enumerator.Current);
    }

    if (multiline) {
      writer.WriteLine();
      writer.Indent--;
    }
  }
}
