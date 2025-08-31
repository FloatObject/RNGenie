## RNGenie.Picker

Uniform and weighted random selection utilities.

---

## Install

```sh
dotnet add package RNGenie.Picker
```

---

## Usage

```cs
using RNGenie.Core.Sources;
using RNGenie.Picker;

var rng = new Pcg32Source(123);

// Uniform pick
var fruit = rng.PickOne(new[] { "apple", "banana", "cherry" });

// One-off weighted pick
var loot = rng.PickWeighted(new[] { ("common", 0.9), ("rare", 0.1) });

// Reusable weighted table
var table = new WeightedPicker<string>()
    .Add("common", 0.9)
    .Add("rare",   0.1);

var drop = rng.PickWeighted(table);
```

---

## Features

- Uniform picks from arrays/lists
- Weighted picks inline or via `WeightedPicker<T>`
- Deterministic with seeded RNGs
- Great for loot tables, gacha, simulations

---
