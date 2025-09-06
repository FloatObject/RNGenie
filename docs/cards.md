## RNGenie.Cards

Card and deck mechanics for RNG-driven systems.

---

## Install

```sh
dotnet add package RNGenie.Cards
```

---

## Usage

```cs
using RNGenie.Core.Sources;  // PCG32, SystemRandomSource, CryptoRandomSource
using RNGenie.Cards;

// Create a standard 52-card deck (no jokers by default).
var deck = new Deck();

// Deterministic shuffle with a seeded RNG.
var rng = new Pcg32Source(123);
deck.Shuffle(rng);

// Peek the top card without consuming it.
var next = deck.Peek();

// Draw cards (consumes from the top).
var first = deck.Draw();      // single
var five  = deck.Draw(5);     // multiple

Console.WriteLine($"Count remaining: {deck.Count}");

// Shuffle only the remaining (already-drawn cards stay put).
deck.ShuffleRemaining(rng);

// Reset deck to original composition and order.
deck.Reset();

// ----- With Jokers -----

var deckWithJokers = new Deck(includeJokers: true);
Console.WriteLine(deckWithJokers.Capacity); // 54

// ----- Reproducible Shuffles -----

var a = new Deck(includeJokers: true);
var b = new Deck(includeJokers: true);

var r1 = new Pcg32Source(999);
var r2 = new Pcg32Source(999);

a.shuffle(r1);
b.shuffle(r2);

// Identical order and draw sequence.
bool same = a.RemainingSpan.SequenceEqual(b.RemainingSpan);
```

---

## Features

- Standard **52-card** deck, optionally **two jokers** (`includeJokers: true`) -> 54 cards)
- **Deterministic** Fisher-Yates shuffle via `IRandomSource`
- **ShuffleRemaining**: permute only the **undrawn** tail (doesn't reset index)
- **Zero-alloc views** of remaining cards via `RemainingSpan` and `Remaining` enumerator.
- Productivity helpers: `Peek`, `Draw()`, `Draw(n)`, `Reset()`
- Clear exceptions for invalid operations (empty draw, null RNG, etc.)

---