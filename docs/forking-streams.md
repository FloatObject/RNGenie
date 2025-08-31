# Forking & Streams in RNGenie (PCG32)

RNGenie’s `Pcg32Source` offers **two ways to branch randomness** and one way to create **independent streams**:

## TL;DR

- `Fork()`  
  Branches from the **current state** using the **same stream id**.  
  → The fork’s **next value equals** the next value the main RNG would produce. After that, sequences diverge as each RNG advances independently.

- `Fork(ulong streamId)`  
  Branches from the **current state** but uses a **new stream id**.  
  → The fork is **independent immediately** (its very next value can differ).

- `NewStreamFromSeed(ulong streamId)`  
  Starts a stream from the **original seed**, with the given **stream id**.  
  → Great for **subsystems** (loot, particles, AI) that must be stable regardless of call order elsewhere.

## Why streams?

PCG32 uses a **state** and a **stream/sequence id**. The state moves forward as you draw numbers; the stream id selects an independent sequence family. This lets you:

- **Branch timelines** for what-if analysis (`Fork()` / `Fork(streamId)`).
- **Create per-subsystem streams** that don’t interfere (`NewStreamFromSeed()`).

## Behavior examples

```csharp
var rng = new Pcg32Source(123);
rng.NextInt(0, 100); // advance

// 1) Fork with the SAME stream: “branch the timeline”
var forkSame = rng.Fork();
var nextMain  = rng.NextInt(0, 100);
var firstFork = forkSame.NextInt(0, 100);
// nextMain == firstFork

// 2) Fork with a DIFFERENT stream: “independent immediately”
var forkDiff = rng.Fork(42);
var nextMain2  = rng.NextInt(0, 100);
var firstFork2 = forkDiff.NextInt(0, 100);
// nextMain2 != firstFork2  (very likely)

// 3) Independent subsystem streams from the original seed
var lootRng   = rng.NewStreamFromSeed(10);
var aiRng     = rng.NewStreamFromSeed(20);
var vfxRng    = rng.NewStreamFromSeed(30);
// These sequences are stable regardless of how others advance.
```

## ASCII demonstration diagram

Legend:
 Main   = Main RNG
 Fork   = Fork (same stream, same state)
 Diff   = Fork with different streamId
 Stream = NewStreamFromSeed (independent, from original seed)

-----------------------------------------------------------------

Case 1: Fork()  (same stream, same state)

```text
 Main:   [42] -> 57 -> 91 -> ...
 Fork:           57 -> 91 -> ...
                 ^ if advanced together, always the same results
```

=> Sequences line up immediately, then diverge as they advance separately.

-----------------------------------------------------------------

Case 2: Fork(streamId)  (same state, different stream)

```text
 Main:   [42] -> 57 -> 91 -> ...
 Diff:   [42] -> 57 -> 88 -> ...
                       ^ diverges on second draw
```

=> Both share the same state but use different sequence increments.

-----------------------------------------------------------------

Case 3: NewStreamFromSeed(streamId)  (original seed, independent stream)

```text
 Main:   [seed=123] -> 57 -> 91 -> ...
 Stream: [seed=123] -> 96 -> 12 -> ...
                       ^ restarts at the beginning of the seed
```

=> Always produces a consistent sequence for a given seed+streamId,
   regardless of how far the main RNG has advanced.

## Note on first-draw behavior (PCG32)

When you fork a Pcg32Source from the same state, the first draw from both the main RNG and any fork
(same-stream or different-stream) will always be identical.

This is not a bug - it is a property of the PCG32 algorithm itself:
- PCG32 generates its output from the old state before applying the update step.
- The streamId only affects the **increment**, which is applied when advancing to the next state.
- As a result:
  - `Fork()` (same stream):         first output matches main's next, and stays in lockstep forever.
  - `Fork(streamId)` (diff stream): first output matches main's next, but diverges starting with the second draw.
  - `NewStreamFromSeed(streamId)`:  fully independent immediately, since it's seeded from the original seed, not the current state.

This means seeing the same value on the first draw of a different-stream fork is expected.
Independence really begins on the second draw.
