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
using System.Linq;
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

// ----- Custom Deck Compositions -----

// You can build a deck from any sequence of the built-in Card type.
// The order you pass in becomes the original (unshuffled) order.
// Reset() will restore to exactly this sequence.
var custom = new Deck(new []
{
    new Card((Suite)0, (Rank)1),   // A♠
    new Card((Suite)1, (Rank)13),  // K♥
    Card.Joker(1)                  // Distinct Joker
});

// Short deck (strip 2-5)
// 6..13 (inclusive) across all four suits
var shortDeck = new Deck(
    from s in Enumerable.Range(0, 4)
    from r in Enumerable.Range(6, 8)   // 6..13
    select new Card((Suite)s, (Rank)r)
);

// Pinochle-style (9-Ace, two copies of each)
var pinochle = new Deck(
    Enumerable.Repeat(0, 2).SelectMany(_ =>
        from s in Enumerable.Range(0, 4)
        from r in Enumerable.Range(9, 6)  // 9..14 (Ace)
        select new Card((Suite)s, (Rank)r)
    )
);

// "Stacked" deck for deterministic tests
// Put specific cards on top for predictable draws.
var stackedTop = new Deck(new []
{
    new Card((Suite)0, (Rank)1),  // A♠ (will be drawn first)
    new Card((Suite)0, (Rank)13), // K♠ (second)
    new Card((Suite)1, (Rank)1),  // A♥ (third)
    // …followed by any sequence you like
});

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