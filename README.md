# Metadata

CodeJunkie.Metadata is a metaprogramming tool for generating metadata and enabling reflection-like capabilities in ahead-of-time (AOT).

## Installation

Add the latest versions of [Metadata] and [Metadata Generator] from NuGet to your C# project.

### Requirements
- **.NET 8 SDK**: Ensure your project targets the latest .NET 8 SDK to avoid compiler mismatches (`CS9057` warning). Treat this warning as an error to prevent downstream issues.

### Example Configuration
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>

    <WarningsAsErrors>CS9057</WarningsAsErrors>

  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CodeJunkie.Metadata" Version="x.x.x" />
    <PackageReference Include="CodeJunkie.Metadata.Generator" Version="x.x.x" PrivateAssets="all" OutputItemType="analyzer" />
  </ItemGroup>
</Project>
```

## Overview

Metadata is a metaprogramming tool that enables C# developers to:
- Generate metadata about types at build time.
- Avoid runtime reflection issues in AOT environments like iOS.
- Power other Code Junkie tools such as [States] and [Seralization].

### Key Features
- Registry of all globally visible types.
- Metadata generation for types, attributes, and properties.
- Support for mixins and type hierarchies.
- Optimized for AOT environments.

The Metadata package provides the following features:

- Create a registry of all types visible from the global scope.
- Generate metadata about visible types.
- Track types by id and version.
- Allow types to implement and look up mixins.
- Compute and cache type hierarchies, attributes, and properties.
- Track generic types of properties in a way that enables convenient serialization in AOT environments.

## Usage

### Introspective Types

Add the `[Meta]` attribute to a partial class or record to enable metadata generation.

```csharp
using CodeJunkie.Metadata;

[Meta]
public partial class MyType;

public partial class Container {
  [Meta] // Nested types are supported.
  public partial class MyNestedType;
}
```
