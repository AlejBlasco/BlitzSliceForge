# ADR-001: Support Multiple .NET Frameworks

## Status

Accepted

## Implemented in

[Issue #001](https://github.com/AlejBlasco/BlitzSliceForge/issues/1)

## Author

A.Blasco

## Context

The initial MVP of BlitzSliceForge only generates projects targeting .NET 9.0 (the current host version). However:

- Many teams and companies are still using .NET 8.0 LTS (official support until November 2026).
- Some corporate or legacy environments cannot migrate to .NET 9 immediately.
- Limiting the scaffolder to a single version drastically reduces the potential audience and early adoption.
- Future .NET versions (10.0, 11.0…) will require the same flexibility.

Without multi-framework support, the user would have to manually modify `global.json`, `Directory.Build.props`, and all `.csproj` files after generation → poor user experience.

## Decision

Add a CLI parameter `--framework <value>` (e.g., `net8.0`, `net9.0`) with default value `net9.0`.

The parameter will be propagated to:
- All `dotnet new -f {framework}` commands
- Generation of `global.json` (`"version": "8.0.100"` or similar)
- `Directory.Build.props` (`<TargetFramework>{{framework}}</TargetFramework>`)
- Pre-check: verify that the corresponding SDK is installed on the machine (using `dotnet --list-sdks`)

## Alternatives Considered

1. **Support only the current host version**  
   → Rejected: too limiting for adoption (especially .NET 8 LTS users).

2. **Automatic multi-targeting (generate multi-framework .csproj)**  
   → Rejected for MVP: high complexity, increases generated package size, not the main use case.

3. **Mandatory parameter with no default**  
   → Rejected: worsens UX for users who want the latest version without extra thinking.

4. **Chosen solution**: optional parameter with default to current version + SDK pre-check.

## Consequences

### Positive
- Greater adoption (compatible with most current .NET environments).
- More flexible and professional experience.
- Prepares the tool for future versions (.NET 10.0, etc.) without major refactoring.
- Early check prevents late errors (“dotnet new fails because SDK is missing”).

### Negative / Trade-offs
- Templates (DbContext, Program.cs, etc.) must be compatible with both net8.0 and net9.0.
- Adds an extra check step (but improves UX by failing fast).
- Possible subtle differences between versions (e.g., .NET 9 features not available in net8.0).

### Mitigated Risks
- Unit and integration tests will cover both frameworks.
- Clear documentation: “Supported frameworks: net8.0 (LTS), net9.0 (current)”.

## References / Links
- [Semantic Versioning](https://semver.org/)
- [.NET Support Policy](https://dotnet.microsoft.com/platform/support/policy/dotnet-core)
- [dotnet new documentation](https://learn.microsoft.com/dotnet/core/tools/dotnet-new)

## Final notes
