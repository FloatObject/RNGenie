# 🎩 RNGenie.Dice 🎲

**Deterministic RPG-Style Dice Roller with Notation Support**

RNGenie.Dice makes dice rolling expressive, reproducible, and extensible.  
Forget reinventing random rolls - just write classic RPG-style notations like 3d6+2 and get deterministic  
results when seeded.

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Dice.svg)](https://www.nuget.org/packages/RNGenie.Dice/)
[![Downloads](https://img.shields.io/nuget/dt/RNGenie.Dice.svg)](https://www.nuget.org/packages/RNGenie.Dice/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](https://github.com/FloatObject/RNGenie/blob/master/CONTRIBUTING.md)

---

## ✨ Features

- Support for **RPG-style dice notation** (`NdM±X`).
- **Deterministic results** when used with a seedable `IRandomSource` (e.g. `Pcg32Source`).
- Access to **raw roll breakdowns** (individual dice + modifiers).
- Explicit and Fluent API for increased versatility.
- Built to be **extensible** (future: exploding dice, keep-highest, adv/dis).

---

## 📄 Documentation

See the [Dice Docs](https://github.com/FloatObject/RNGenie/blob/master/docs/dice.md) for usage and API details.

---

## 🚀 Quick Start

Install Core + Dice:
```sh
dotnet add package RNGenie.Core
dotnet add package RNGenie.Dice
```

Basic usage:
```cs
using RNGenie.Core.Sources;
using RNGenie.Dice;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Roll with static DiceRoller.
var (total, rolls, mod) = DiceRoller.Roll("3d6+2", rng);
Console.WriteLine($"Dice result: {total} (Rolls: {string.Join(",", rolls)}, Modifier: {mod})");

// Roll straight from the RNG source object.
var crit = rng.Roll("1d20");
Console.WriteLine($"Crit check: {crit.total}");
```

Output:
```text
Dice result: 14 (Rolls: 5,6,1, Modifier: +2)
Crit check: 17
```
---

## 🧩 Extensibility

Extend RNGenie.Dice by:
- Adding new dice mechanics (e.g. exploding dice, keep-highest rolls).
- Supporting special notations like advantage/disadvantage.
- Integrating with loot tables or game engines.

---

## 📦 Roadmap

- Exploding dice (`!`).
- Advantage/Disadvantage notation (`adv/dis`).
- Keep-highest/lowest mechanics.
- JSON-driven roll configurations.

---

## 👩‍💻 Contributing

Pull requests are welcome!

Good first issues:
- Add new dice notations.
- Extend dice mechanics. 

See CONTRIBUTING.md for guidance.
Also, check [here](https://github.com/FloatObject/RNGenie/issues) for existing dice issue writeups.

---

## 📜 License

RNGenie is licensed under the MIT License.
This means you're free to use it in open source, commercial, or personal projects.

---

