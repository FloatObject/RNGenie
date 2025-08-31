## RNGenie.Dice

Parse and roll RPG-style dice notations (`NdM±X`).

---

## Install

```sh
dotnet add package RNGenie.Dice
```

---

## Usage

```cs
using RNGenie.Core.Sources;
using RNGenie.Dice;

var rng = new Pcg32Source(123);

// Explicit API
var (total, rolls, mod) = DiceRoller.Roll("3d6+2", rng);

// Fluent API
var crit = rng.Roll("1d20");
Console.WriteLine($"Crit check: {crit.total}");
```

---

## Features

- Supports `{count}d{sides}[+/-modifier]` syntax
- Deterministic with seeded RNGs
- Both explicit and fluent APIs
- Planned: exploding dice, advantage/disadvantage

---
