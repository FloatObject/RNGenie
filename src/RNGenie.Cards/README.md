# 🎩 RNGenie.Cards 🃏

**Deterministic Deck Creation, Shuffling, and Drawing**

RNGenie.Cards provides utilities for working with standard playing-card decks.  
Build 52-card decks (with or without Jokers), or define your own composition using the existing `Card` type.  
Shuffle deterministically with seeded RNG and draw cards with ease - perfect for simulations, games, and experiments.

[![NuGet](https://img.shields.io/nuget/v/RNGenie.Cards.svg)](https://www.nuget.org/packages/RNGenie.Cards/)
[![Downloads](https://img.shields.io/nuget/dt/RNGenie.Cards.svg)](https://www.nuget.org/packages/RNGenie.Cards/)
[![.NET CI](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/FloatObject/RNGenie/actions/workflows/dotnet.yml)
[![style: editorconfig](https://img.shields.io/badge/style-editorconfig-blue)](https://github.com/FloatObject/RNGenie/blob/master/CONTRIBUTING.md)

---

## ✨ Features

- **Standard deck support** (52 cards, optionally with Jokers).
- **Custom deck compositions** of the built-in `Card` type via constructor.
- **Deterministic shuffling** when used with seedable RNG sources (built-in Fisher-Yates).
- **Card operations**: draw, peek, reset, remaining count.
- **Two shuffle modes**: full deck (`Shuffle`) or **remaining only** (`ShuffleRemaining`).

---

## 📄 Documentation

See the [Card Docs](https://github.com/FloatObject/RNGenie/blob/master/docs/cards.md) for usage and API details.

---

## 🚀 Quick Start

Install Core + Cards:
```sh
dotnet add package RNGenie.Core
dotnet add package RNGenie.Cards
```

Basic usage:
```cs
using RNGenie.Core.Sources;
using RNGenie.Cards;

// Seedable RNG for reproducibility
var rng = new Pcg32Source(seed: 123);

// Create a standard deck with Jokers
var deck = new Deck(includeJokers: true);

// Shuffle deterministically
deck.Shuffle(rng);

// Draw cards
var topCard = deck.Draw();
Console.WriteLine($"First card: {topCard}");

var hand = deck.Draw(5);
Console.WriteLine("Hand: " + string.Join(", ", hand));

// Remaining cards
Console.WriteLine($"Cards Remaining: {deck.RemainingSpan.Length}");
```

Output:
```text
First card: Q♣
Hand: 7♠, A♥, 2♦, K♣, 9♠
Cards Remaining: 47
```

Custom composition example:
```
// Short deck (strip 2-5)
var shortDeck = new Deck(
	from s in Enumerable.Range(0, 4)
	from r in Enumerable.Range(6, 8) // 6..13
	select new Card((Suite)s, (Rank)r)
);
```

---

## 🧩 Extensibility

Extend RNGenie.Dice by:
- Creating **custom compositions** of the built-in `Card` type (e.g. short decks, Pinochle).
- Adding wrappers for discard/reshuffle mechanics.
- Implementing alternate shuffle algorithms.

---

## 📦 Roadmap

- Built-in helpers for custom deck types (short deck, Pinochle, Tarot).
- Support for discard and reshuffle utilities.
- JSON-driven deck definitions.
- Integration samples with game frameworks.

---

## 👩‍💻 Contributing

Pull requests are welcome!

Good first issues:
- Add new deck variations.
- Extend shuffle options.
- Add helper utilities for discard/reshuffle.

See CONTRIBUTING.md for guidance.
Also, check [here](https://github.com/FloatObject/RNGenie/issues) for existing card issue writeups.

---

## 📜 License

RNGenie is licensed under the MIT License.
This means you're free to use it in open source, commercial, or personal projects.

---

