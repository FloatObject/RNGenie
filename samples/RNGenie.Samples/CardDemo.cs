using RNGenie.Cards;
using RNGenie.Core.Sources;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of card RNG functionality.
    /// </summary>
    public static class CardDemo
    {
        /// <summary>
        /// Run the card demo.
        /// </summary>
        public static void Run()
        {
            // Use a deterministic RNG source (same seed => same shuffle order).
            var rng = new Pcg32Source(123);

            var newDeck = new Deck();

            // Draw first 5 cards (unshuffled).
            Card[] firstFiveUnshuffled = newDeck.Draw(5);

            Console.WriteLine("First 5 cards from unshuffled deck:");
            Console.WriteLine(string.Join(", ", firstFiveUnshuffled));

            Console.WriteLine("\nShuffling...");

            // Fisher-Yates shuffle, remaining cards only.
            newDeck.ShuffleRemaining(rng);

            // Next 5 cards (shuffled).
            Card[] nextFiveShuffled = newDeck.Draw(5);

            Console.WriteLine("\nNext 5 cards from shuffled deck:");
            Console.WriteLine(string.Join(", ", nextFiveShuffled));

            Console.WriteLine($"\nCards Remaining: {newDeck.RemainingSpan.Length}");

            // New deck, shuffle determinism.
            Console.WriteLine("\nTwo new decks, same seed (420) shuffle.");

            var newSource1 = new Pcg32Source(seed: 420);
            var newSource2 = new Pcg32Source(seed: 420);

            var newDeck1 = new Deck();
            var newDeck2 = new Deck();

            newDeck1.Shuffle(newSource1);
            newDeck2.Shuffle(newSource2);

            Card[] deck1First5 = newDeck1.Draw(5);
            Card[] deck2First5 = newDeck2.Draw(5);

            Console.WriteLine("\nDeck 1 first 5:");
            Console.WriteLine(string.Join(", ", deck1First5));

            Console.WriteLine("\nDeck 2 first 5:");
            Console.WriteLine(string.Join(", ", deck2First5));
        }
    }
}
