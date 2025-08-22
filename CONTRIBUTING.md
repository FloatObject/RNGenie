# Contributing to RNGenie

## Setup
- .NET 8 or 6
- `dotnet restore`
- `dotnet build`
- `dotnet test`

## PRs
- Add/extend tests for new features.
- Keep public APIs small and documented with XML summaries.
- New distributions: implement `IDistribution<T>` and add basic statistical sanity tests.
- New dice rules: extend `Dice` or add a new type under `RNGenie.Dice` with tests.

## Style
- Nullable enabled, warnings addressed.
- No external deps for Core unless absolutely necessary.
