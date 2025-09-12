# 🎩 RNGenie.Distributions 📈

**Lightweight Probability Distributions for Deterministic Sampling**

RNGenie.Distributions provides ready-to-use sampling primitives (e.g. Uniform, Triangular, Gaussian-approx)  
that integrate with `IRandomSource` for reproducible simulations.

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Distributions.svg)](https://www.nuget.org/packages/RNGenie.Distributions/)
[![Downloads](https://img.shields.io/nuget/dt/RNGenie.Distributions.svg)](https://www.nuget.org/packages/RNGenie.Distributions/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](https://github.com/FloatObject/RNGenie/blob/master/CONTRIBUTING.md)

---

## ✨ Features

- Common distributions (e.g. **Uniform**, **Triangular**, **Gaussian approximation (using Box-Muller algorithm**).
- All samplers implement `IDistribution<T>` -> **compose and test easily**.
- **Deterministic** when sampled with a seeded `IRandomSource`.

---

## 📄 Documentation

See the [Distribution Docs](https://github.com/FloatObject/RNGenie/blob/master/docs/distributions.md) for usage and API details.

---

## 🚀 Quick Start

Install Core + Distributions:
```sh
dotnet add package RNGenie.Core
dotnet add package RNGenie.Distributions
```

Basic usage:
```cs
using RNGenie.Core.Sources;
using RNGenie.Distributions.Continuous;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Sample from a Gaussian-like distribution
var normal = new Gaussian(mean: 0.0, stdDev: 1.0);
double x = normal.Sample(rng);

// Sample from a triangular distribution
var tri = Triangular(min: 0, mode: 10, max: 20);
double y = tri.Sample(rng);

Console.WriteLine($"Normal: {x:F3}, Triangular: {y:F3}");
```

Output:
```text
Normal: -0.318, Triangular: 12.474
```

Implement your own distribution:
```
using RNGenie.Core.Abstractions;

public sealed class Bernoulli : IDistribution<bool>
{
	private readonly double _p;
	
	public Bernoulli(double p) => _p = Math.Clamp(p, 0.0, 1.0);
	
	public bool Sample(IRandomSource rng) => rng.NextDouble() < _p;
}
```

---

## 🧩 Extensibility

- Build **compound models** by sampling multiple distributions.
- Swap RNG sources (`Pcg32Source`, `SystemRandomSource`, `CryptoRandomSource`) without changing your model code.
- Use the same seed to **replay experiments** exactly.

---

## 📦 Roadmap

- Swappable **Ziggurat** algorithm for Gaussian distributions (currently uses Box-Muller).
- **Exponential**, **Poisson**, **Gamma**, **Beta**, **Binomial** samplers.
- **Alias method** for large discrete distributions.

---

## 👩‍💻 Contributing

Pull requests are welcome!

Good first issues:
- Add new distributions + unit tests
- Numerical validation harness

See CONTRIBUTING.md for guidance.
Also, check [here](https://github.com/FloatObject/RNGenie/issues) for existing distribution issue writeups.

---

## 📜 License

RNGenie is licensed under the MIT License.
This means you're free to use it in open source, commercial, or personal projects.

---

