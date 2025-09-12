# 🎩 RNGenie.Picker 🎯

**Deterministic Uniform & Weighted Picks for Loot Tables and Simulations**

RNGenie.Picker provides simple, reproducible selection utilities.
Define rarity tables or equal-weight sets and pick items deterministically with a seeded RNG.

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Picker.svg)](https://www.nuget.org/packages/RNGenie.Picker/)
[![Downloads](https://img.shields.io/nuget/dt/RNGenie.Picker.svg)](https://www.nuget.org/packages/RNGenie.Picker/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](https://github.com/FloatObject/RNGenie/blob/master/CONTRIBUTING.md)

---

## ✨ Features

- **Weighted selection** with a fluent API (great for rarity tables).
- **Seeded determinism** via RNGenie.Core RNG sources.
- Works with **any type** (`WeightedPicker<T>`).
- Minimal allocations, straightforward API.

---

## 📄 Documentation

See the [Picker Docs](https://github.com/FloatObject/RNGenie/blob/master/docs/picker.md) for usage and API details.

---

## 🚀 Quick Start

Install Core + Picker:
```sh
dotnet add package RNGenie.Core
dotnet add package RNGenie.Picker
```

Basic usage:
```cs
using RNGenie.Core.Sources;
using RNGenie.Picker;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Weighted rarity table
var rarity = new WeightedPicker<string>()
	.Add("Common", 0.75)
	.Add("Rare", 0.20)
	.Add("Epic", 0.05)
	.One(rng);

Console.WriteLine($"You got a {rarity} item!");
```

Output:
```text
You got a Rare item!
```
---

## 🧩 Extensibility

- Layer pickers to build **tiered loot tables** (pick rarity -> pick item from that bucket).
- Use the same RNG across systems to keep simulation runs **replayable**.
- Compose with distributions (e.g. roll counts, cooldowns).

---

## 📦 Roadmap

- "Pick many" helpers (with/without replacement).
- Alias-method sampler for large, static tables.
- Policy hooks (e.g. pity timers).

---

## 👩‍💻 Contributing

Pull requests are welcome!

Good first issues:
- Add helper APIs for "pick many"
- Benchmarks for large tables

See CONTRIBUTING.md for guidance.
Also, check [here](https://github.com/FloatObject/RNGenie/issues) for existing picker issue writeups.

---

## 📜 License

RNGenie is licensed under the MIT License.
This means you're free to use it in open source, commercial, or personal projects.

---

