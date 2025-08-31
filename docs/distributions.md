## RNGenie.Distributions

Probability distributions built on top of RNGenie.Core.

---

## Install

```sh
dotnet add package RNGenie.Distributions
```

---

## Usage

```cs
using RNGenie.Core.Sources;
using RNGenie.Distributions.Continuous;

var rng = new Pcg32Source(123);

// Uniform distribution
var uniform = new Uniform(5, 12);
Console.WriteLine(uniform.Sample(rng));

// Gaussian (normal) distribution
var normal = new Gaussian(0, 1);
Console.WriteLine(normal.Sample(rng));

```

---

## Features

- Uniform, Triangular, Gaussian
- All implement `IDistribution<T>` with `.Sample(rng)`
- Deterministic with seeded RNGs
- Useful for games, simulations, and Monte Carlo methods

---
