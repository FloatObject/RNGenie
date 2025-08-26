# Contributing to RNGenie

We welcome contributions of all kinds - new RNG algorithms, distributions, dice rules, demos, tests, and docs.

---

## Quickstart Checklist

Before opening a PR, always:

1. `dotnet restore`
2. `dotnet format style --verify-no-changes --verbosity minimal`
3. `dotnet format whitespace --verify-no-changes --verbosity minimal`
4. `dotnet build -warnaserror:IDE1006`
5. `dotnet test`

---

## Setup

- Install **.NET 8** (or 6 if you're targeting that).
- Restore, build, and test the solution:

```bash
dotnet restore
dotnet build
dotnet test
```

---

## Workflow

1. Fork the repo and create a feature branch.
2. Make your changes with tests.
3. Run style checks (see below).
4. Verify tests pass.
5. Open a Pull Request (PR).

---

## Pre-PR Checks (Required)

Before submitting a PR, please run the following commands locally:

```bash
# Verify only auto-fixable style/whitespace issues (silent if clean).
dotnet format style --verify-no-changes --verbosity minimal
dotnet format whitespace --verify-no-changes --verbosity minimal

# Optional: fixable analyzers (unused usings / placement).
dotnet format analyzers --diagnostics IDE0005,IDE0065 --verify-no-changes --verbosity minimal

# Full build with naming enforcement.
dotnet build -warnaserror:IDE1006

# Unit tests
dotnet test
```

CI enforces these same rules, so running these ensures your PR will pass without formatting or naming failures.

---

## Pull Request Guidelines

- Tests: Add or extend tests for new features. PRs without tests will not be merged.
- Docs: Public APIs must have XML doc summaries (/// comments).
- Distributions: Implement `IDistribution<T>` and include sanity tests (mean/variance checks).
- Dice Rules: Extend Dice or add a new type under `RNGenie.Dice` with tests.

---

## Code Style Summary

RNGenie enforces consistent style via .editorconfig and CI.

**General**
- Nullable enabled.
- Warnings must be fixed or suppressed with justification.
- No external dependencies in Core unless absolutely necessary.

**Naming**
- Private fields → _camelCase (leading underscore + lowercase first letter).
- Interfaces → IPascalCase (e.g. `IRandomSource`)
- Constants (const) → PascalCase.
- Public members must include XML documentation summaries.
- Test projects (`tests/RNGenie.Tests`) are exempt from strict naming rules (underscores in test method names are fine).

**Usings**
- `System.*` usings go first.
- Usings placed outside namespaces.
- Remove unused usings.

**Formatting**
- Spaces, indent size = 4.
- New line before { and between query clauses.
- Expression-bodied members allowed only when single-line.
- Prefer pattern matching, switch expressions, tuple swaps.
- Prefer null propagation (?.) and null coalescing (??).

**Commit Messages**
Use clear, conventional commit messages, for example:
- feat(dist): add Beta distribution
- fix(dice): correct weighting bug
- chore(format): apply dotnet format cleanup

---
