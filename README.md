# 🎩 RNGenie 🔮
**Extensible Randomness Helpers for Games and Simulations**

RNGenie is a lightweight C# library that makes randomness in your projects **easy, reproducible, and fun**.
Instead of rewriting weighted picks, dice rollers, or loot tables for every project, just let the genie grant your wishes. ✨

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Core.svg)](https://www.nuget.org/packages/RNGenie.Core/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FloatObject/RNGenie/actions)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](./CONTRIBUTING.md)

---

## ✨ Features
- **Weighted Picks** (`WeightedPicker<T>`) for loot tables and drop chances
- **Dice Roller** with standard RPG notation (`3d6+2`, exploding dice, adv/dis coming soon)
- **Probability Distributions** (uniform, triangular, normal approximation)
- **Loot Tables** with conditional chaining & JSON definitions
- **Simulation Helpers** (Monte Carlo runner)
- **Pluggable RNG Sources** (use System.Random, PCG32, or your own)
- **Branching RNG Timelines (Pcg32)** branch the timeline or spin up independent streams (see [Forking & Streams](docs/forking-streams.md))

---

## 🚀 Quick Start

Install from NuGet:
```sh
dotnet add package RNGenie.Core
```
Basic usage:
```cs
using RNGenie;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Weighted pick
var rarity = new WeightedPicker<string>()
    .Add("Common", 0.75)
    .Add("Rare",   0.20)
    .Add("Epic",   0.05)
    .One(rng);

Console.WriteLine($"You got a {rarity} item!");

// Dice roller
var (total, rolls, mod) = Dice.Roll("3d6+2", rng);
Console.WriteLine($"Dice result: {total} (rolls: {string.Join(",", rolls)} {mod:+#;-#;0})");

// Distribution sampling
var dist = new Triangular(5, 12, 20);
Console.WriteLine($"Damage roll: {dist.Sample(rng):F2}");
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
}

public interface IDistributable<T> {
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

