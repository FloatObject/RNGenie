# 🎩 RNGenie 🔮
**Extensible Randomness Helpers for Games and Simulations**

RNGenie is a lightweight C# library that makes randomness in your projects **easy, reproducible, and fun**.
Instead of rewriting weighted picks, dice rollers, or loot tables for every project, just let the genie grant your wishes. ✨

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Core.svg)](https://www.nuget.org/packages/RNGenie.Core/)
[![Downloads](https://img.shields.io/nuget/dt/RNGenie.Core.svg)](https://www.nuget.org/packages/RNGenie.Core/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FloatObject/RNGenie/actions)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](./CONTRIBUTING.md)

---

## ✨ Features (per package)
- **RNGenie.Core** → pluggable RNG sources (`Pcg32`, `SystemRandomSource`, `CryptoRandomSource`) + abstractions (`IRandomSource`, reproducibility, branching timelines).
- **RNGenie.Dice** → RPG-style dice roller with notation (`3d6+2`), deterministic when seeded.
- **RNGenie.Picker** → uniform and weighted selection for loot tables, drop rates, and simulations.
- **RNGenie.Distributions** → probability distributions (uniform, triangular, normal approximation).
- **(Future) RNGenie.Json** → save/load RNG state, expore samples for visualization.

---

## 🚀 Quick Start

Install the core package:
```sh
dotnet add package RNGenie.Core
```

Install extras as needed:
```sh
dotnet add package RNGenie.Dice
dotnet add package RNGenie.Picker
dotnet add package RNGenie.Distributions
```

Basic usage:
```cs
using RNGenie.Core.Sources;
using RNGenie.Dice;
using RNGenie.Picker;
using RNGenie.Distributions;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Weighted pick
var rarity = new WeightedPicker<string>()
    .Add("Common", 0.75)
    .Add("Rare",   0.20)
    .Add("Epic",   0.05)
    .One(rng);

Console.WriteLine($"You got a {rarity} item!");

// Dice roller (explicit + fluent)
var (total, rolls, mod) = DiceRoller.Roll("3d6+2", rng);
Console.WriteLine($"Dice result: {total}");

var crit = rng.Roll("1d20");
Console.WriteLine($"Crit check: {crit.total}");

// Distribution sampling
var normal = new Gaussian(0, 1);
Console.WriteLine($"Normal sample: {normal.Sample(rng):F2}");
```

Output:
```text
You got a Rare item!
Dice result: 15 (rolls: 4,5,4 +2)
Damage roll: 11.73
```
---

## 🧩 Extensibility

RNGenie is built around simple interfaces:
```cs
public interface IRandomSource {
    int NextInt(int minInclusive, int maxExclusive);
    double NextDouble(); // [0,1)
    void NextBytes(Span<byte> buffer);
}

public interface IDistribution<T> {
    T Sample(IRandomSource rng);
}
```

That means you can:
- Plug in your own RNG algorithm (PCG, XorShift, etc.)
- Contribute new probability distributions
- Add new dice mechanics (exploding, keep-highest, etc.)
- Extend loot tables with custom rules or policies

---

## 📦 Roadmap

- **Dice Mechanics:** exploding dice (!), advantage/disadvantage (adv/dis).
- **Distributions:** Exponential, Poisson, Alias method sampler.
- **Integration:** JSON-driven loot tables, Unit/MonoGame samples.
- **Visualization:** charts for distributions & loot outcomes.

---

## 👩‍💻 Contributing

Pull requests are welcome!
Good first issues:
- Add new dice notations
- Add new probability distributions
- Extend loot table policies
See CONTRIBUTING.md for guidelines.

---

## 📜 License

RNGenie is licensed under the MIT License.
This means you're free to use it in open source, commercial, or personal projects.

---

