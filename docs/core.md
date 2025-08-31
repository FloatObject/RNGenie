## RNGenie.Core

The foundation of RNGenie. Provides `IRandomSource` and `IDistribution<T>` abstractions and built-in RNG sources.

---

## Install

```sh
dotnet add package RNGenie.Core
```

---

## Usage

```cs
using RNGenie.Core.Sources;

// Deterministic RNG
var rng = new Pcg32Source(seed: 123);

// Raw uniform sampling
int i = rng.NextInt(0, 100); // integer in [0, 100)
double d = rng.NextDouble(); // double in [0, 1)
```

---

## Features

- `IRandomSource` interface
- `IDistribution<T>` interface
- Built-in RNG sources:
  - `Pcg32Source` (deterministic, reproducible, supports forking/streams)
  - `SystemRandomSource` (wraps System.Random)
  - `CryptoRandomSource` (cryptographically secure)
- Supports **branching timelines** and **independent streams** via PCG32 (see [Forking & Streams](./forking-streams.md))

---
