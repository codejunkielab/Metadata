namespace CodeJunkie.Metadata.Generator.Utils;

using System.Collections.Generic;
using System.Linq;
using CodeJunkie.Metadata.Generator.Models;
using Microsoft.CodeAnalysis;

/// <summary>
/// Provides utility methods for creating diagnostic messages related to metadata generation.
/// </summary>
public static class Diagnostics {
  /// <summary>
  /// Prefix used for all diagnostic error codes.
  /// </summary>
  public const string ErrorPrefix = "Metadata";

  /// <summary>
  /// Category used for all diagnostic messages.
  /// </summary>
  public const string ErrorCategory = "CodeJunkie.Metadata.Generator";

  /// <summary>
  /// Creates a diagnostic message indicating that a type is not visible from the global namespace.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the type that is not visible.</param>
  /// <param name="offendingTypes">The types that are not fully visible.</param>
  /// <returns>A diagnostic message with a warning severity.</returns>
  public static Diagnostic TypeNotVisible(Location location,
                                          string name,
                                          IEnumerable<DeclaredType> offendingTypes) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_000",
          "Introspective Type Global Namespace Visibility",
          messageFormat:
          "Cannot determine if the introspective type `{0}` is visible from the " +
          "global namespace. Please make sure the type **and all of its " +
          "containing types** have `public` or `internal` accessibility " +
          "modifiers in the file that contains the [" +
          Constants.MetadataAttributeName + "] attribute on the type. " +
          "The following types are not indicated to be fully visible: {1}.",
          category: ErrorCategory,
          DiagnosticSeverity.Warning,
          isEnabledByDefault: true),
        location,
        name,
        string.Join(",", offendingTypes.Select(t => $"`{t.Reference.SimpleNameClosed}`")));

  /// <summary>
  /// Creates a diagnostic message indicating that a type is not fully marked as partial.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the type that is not fully partial.</param>
  /// <param name="offendingTypes">The types that need to be marked as partial.</param>
  /// <returns>A diagnostic message with an error severity.</returns>
  public static Diagnostic TypeNotFullyPartial(Location location,
                                               string name,
                                               IEnumerable<DeclaredType> offendingTypes) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_001",
          "Introspective Type Not Fully Partial",
          messageFormat:
          "Introspective type `{0}` is not fully partial. Please make sure the " +
          "type **and all of its containing types** are marked as partial. " +
          "The following types still need to be marked as partial: {1}.",
          category: ErrorCategory,
          DiagnosticSeverity.Error,
          isEnabledByDefault: true),
        location,
        name,
        string.Join(",", offendingTypes.Select(t => $"`{t.Reference.SimpleNameClosed}`")));

  /// <summary>
  /// Creates a diagnostic message indicating that a type is generic, which is not allowed.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the type that is generic.</param>
  /// <param name="offendingTypes">The types that are generic.</param>
  /// <returns>A diagnostic message with a warning severity.</returns>
  public static Diagnostic TypeIsGeneric(Location location,
                                         string name,
                                         IEnumerable<DeclaredType> offendingTypes) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_002",
          "Introspective Type Is Generic",
          messageFormat:
          "Introspective type `{0}` cannot be generic. Please make sure the " +
          "type **and all of its containing types** are not generic. " +
          "The following types are generic: {1}.",
          category: ErrorCategory,
          DiagnosticSeverity.Warning,
          isEnabledByDefault: true),
        location,
        name,
        string.Join(",", offendingTypes.Select(t => $"`{t.Reference.SimpleNameClosed}`")));

  /// <summary>
  /// Creates a diagnostic message indicating that a type does not have a unique ID.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the type with a duplicate ID.</param>
  /// <param name="type">The type with the duplicate ID.</param>
  /// <param name="existingType">The existing type that shares the same ID.</param>
  /// <returns>A diagnostic message with an error severity.</returns>
  public static Diagnostic TypeDoesNotHaveUniqueId(Location location,
                                                   string name,
                                                   DeclaredType type,
                                                   DeclaredType existingType) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_003",
          "Introspective Type Does Not Have Unique Id",
          messageFormat:
          "Introspective type `{0}` shares the same id `{1}` as the type " +
          "`{2}.` Please ensure the id is unique across all types in your " +
          "project.",
          category: ErrorCategory,
          DiagnosticSeverity.Error,
          isEnabledByDefault: true),
        location,
        name,
        type.Id,
        existingType.FullNameClosed);

  /// <summary>
  /// Creates a diagnostic message indicating that a type has an invalid version.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the type with an invalid version.</param>
  /// <param name="type">The type with the invalid version.</param>
  /// <returns>A diagnostic message with a warning severity.</returns>
  public static Diagnostic TypeHasInvalidVersion(Location location,
                                                 string name,
                                                 DeclaredType type) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_004",
          "Introspective Type Invalid Version",
          messageFormat:
          "Introspective type `{0}` has an invalid version `{1}`. Please ensure " +
          "the version is an integer value >= 1.",
          category: ErrorCategory,
          DiagnosticSeverity.Warning,
          isEnabledByDefault: true),
        location,
        name,
        type.Version);

  /// <summary>
  /// Creates a diagnostic message indicating that an abstract type should not have a version.
  /// </summary>
  /// <param name="location">The location in the source code where the issue was detected.</param>
  /// <param name="name">The name of the abstract type with a version.</param>
  /// <param name="type">The abstract type with a version.</param>
  /// <returns>A diagnostic message with a warning severity.</returns>
  public static Diagnostic AbstractTypeHasVersion(Location location,
                                                  string name,
                                                  DeclaredType type) =>
    Diagnostic.Create(
        new(
          $"{ErrorPrefix}_005",
          "Introspective Abstract Type Has Version",
          messageFormat:
          "Abstract introspective type `{0}` should not have a version. Please " +
          "remove the version attribute from the type.",
          category: ErrorCategory,
          DiagnosticSeverity.Warning,
          isEnabledByDefault: true),
        location,
        name);
}
