# ADR-002: Implement add feature command

## Status

Proposed

## Implemented in

[Issue #XX](https://github.com/AlejBlasco/BlitzSliceForge/issues/XX)

## Author

[Author Name]

## Context

Currently, BlitzSliceForge CLI (`bsf`) allows users to scaffold a new solution with a clean architecture and vertical slice structure. However, there is no command to add new features (vertical slices) to an existing solution. Users must manually copy, rename, and adapt template files, which is error-prone and time-consuming. There is a need for a command that automates the generation of a new feature (vertical slice) within an existing solution, following the established conventions and structure.

## Decision

Add a new CLI command:
```bash
bsf add feature <FeatureName>
```

When executed inside a valid BlitzSliceForge solution, this command will:

- Detect the solution root (by looking for `.sln` or `global.json`).
- Locate the Application and Web projects.
- Copy and rename a template vertical slice from `Templates/Features/Product/` (or embedded equivalent).
- Replace placeholders:
  - `Product` → `<FeatureName>`
  - `product` → `<featureName>` (camelCase)
  - `{{SolutionName}}` → actual solution name
- Generate the following structure (minimum MVP):
  - `src/{SolutionName}.Application/Features/{FeatureName}/`
    - `Commands/`, `Queries/`, `Validators/`, `Dtos/`
  - `src/{SolutionName}.Web/Features/{FeatureName}/`
    - `Pages/`, `Components/`
    ```
- Use a hardcoded template based on one example feature ("Product").
- Perform simple string replacement for renaming.
- Validate that the command is run inside a valid solution folder.
- Show clear error messages if not in a valid BlitzSliceForge project.

```bash
src/{SolutionName}.Application/
└── Features/
    └── {FeatureName}/
        ├── Commands/
        │   ├── Create{FeatureName}Command.cs
        │   └── Create{FeatureName}CommandHandler.cs
        ├── Queries/
        │   ├── Get{FeatureName}ByIdQuery.cs
        │   └── Get{FeatureName}ByIdQueryHandler.cs
        ├── Validators/
        └── Dtos/

src/{SolutionName}.Web/
└── Features/
    └── {FeatureName}/
        ├── Pages/
        │   ├── Index.razor
        │   ├── Create.razor
        │   └── Edit.razor
        └── Components/
            └── {FeatureName}Card.razor
```

## Alternatives Considered

### Alternative 1: Manual feature addition

- **Description:** Users manually copy, rename, and adapt template files for each new feature.
- **Pros:** No additional CLI complexity. Maximum flexibility for users.
- **Cons:** Error-prone, repetitive, and time-consuming. Inconsistent structure and naming.

### Alternative 2: Interactive wizard

- **Description:** Implement an interactive CLI wizard to guide users through feature creation with options for CRUD, UI, etc.
- **Pros:** User-friendly, flexible, supports advanced scenarios.
- **Cons:** Higher implementation complexity. Out of scope for MVP.

## Consequences

### Positive

- Faster and more consistent feature (vertical slice) creation.
- Reduces manual errors and enforces project conventions.
- Improves developer experience and productivity.

### Negative

- Initial implementation is limited to a hardcoded template ("Product").
- No support for advanced options (CRUD, custom templates) in MVP.

### Risks

- Users may expect more flexibility (e.g., custom templates, CRUD generation) than provided in MVP.
- Incorrect detection of solution root or project folders could cause errors.

## References / Links

- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [BlitzSliceForge Documentation](https://github.com/AlejBlasco/BlitzSliceForge)