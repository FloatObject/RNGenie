using RNGenie.Core.Abstractions;

namespace RNGenie.Cards
{
    /// <summary>
    /// A standard 52-card deck, optionally with two distinct Jokers.
    /// Deterministic shuffle via <see cref="IRandomSource"/> (Fisher-Yates).
    /// </summary>
    public sealed class Deck
    {
        private readonly Card[] _original;
        private readonly Card[] _cards;
        private int _index;

        /// <summary>Total capacity including Jokers (52 or 54).</summary>
        public int Capacity => _cards.Length;

        /// <summary>Number of cards remaining to draw.</summary>
        public int Count => Capacity - _index;

        /// <summary>A zero-allocation view of remaining cards (top at index 0 of the span).</summary>
        public ReadOnlySpan<Card> RemainingSpan => _cards.AsSpan(_index);

        /// <summary>Enumerates remaining cards from the top of the deck without allocating.</summary>
        public IEnumerable<Card> Remaining
        {
            get
            {
                for (int i = _index; i < _cards.Length; i++)
                {
                    yield return _cards[i];
                }
            }
        }

        /// <summary>
        /// Create a new Deck with optional jokers.
        /// </summary>
        /// <param name="includeJokers"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Deck(bool includeJokers = false)
        {
            int size = includeJokers ? 54 : 52;
            _original = new Card[size];
            _cards = new Card[size];

            int k = 0;

            // Standard 52
            for (int s = 0; s < 4; s++)
            {
                for (int r = 1; r <= 13; r++)
                {
                    _original[k++] = new Card((Suite)s, (Rank)r);
                }
            }
            // Jokers at the end (distinct).
            if (includeJokers)
            {
                _original[k++] = Card.Joker(1);
                _original[k++] = Card.Joker(2);
            }

            Reset();
        }

        /// <summary>
        /// Reset to the original, unshuffled composition, and set draw position to top.
        /// </summary>
        public void Reset()
        {
            Array.Copy(_original, _cards, _original.Length);
            _index = 0;
        }

        /// <summary>
        /// Fisher-Yates in-place shuffle, deterministic via the supplied RNG.
        /// </summary>
        /// <remarks>
        /// Previously drawn cards <c>ARE</c> reintroduced, all cards are shuffled.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the provided <see cref="IRandomSource"/> is null.
        /// </exception>
        public void Shuffle(IRandomSource rng)
        {
            if (rng is null)
                throw new ArgumentNullException(nameof(rng), "IRandomSource object must not be null.");

            // Classic FY: for i from n-1 down to 1, swap i with j in [0..i]
            for (int i = _cards.Length - 1; i > 0; i--)
            {
                int j = rng.NextInt(0, i + 1); // maxExclusive
                if (j != i)
                {
                    ref Card a = ref _cards[i];
                    ref Card b = ref _cards[j];
                    (a, b) = (b, a);
                }
            }

            _index = 0;
        }

        /// <summary>
        /// Fisher-Yates in-place shuffle, deterministic via the supplied RNG.
        /// </summary>
        /// <remarks>
        /// Previously drawn cards are <c>NOT</c> reintroduced, remaining cards are shuffled.
        /// </remarks>
        /// <param name="rng"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void ShuffleRemaining(IRandomSource rng)
        {
            if (rng is null)
                throw new ArgumentNullException(nameof(rng), "IRandomSource object must not be null.");

            for (int i = _cards.Length - 1; i > _index; i--)
            {
                int j = rng.NextInt(_index, i + 1);
                if (j != i)
                {
                    ref var a = ref _cards[i];
                    ref var b = ref _cards[j];
                    (a, b) = (b, a);
                }
            }
            // Note: DO NOT reset index here.
        }

        /// <summary>
        /// Peek at the top card without drawing.
        /// </summary>
        /// <returns>The card at the top of the deck.</returns>
        public Card Peek()
        {
            if (Count <= 0)
                throw new InvalidOperationException("Cannot peek, the deck is empty.");

            return _cards[_index];
        }

        /// <summary>
        /// Draw a single card from the top of the deck.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Card Draw()
        {
            if (_index >= _cards.Length)
                throw new InvalidOperationException("Cannot draw, the deck is empty.");

            return _cards[_index++];
        }

        /// <summary>
        /// Draw <paramref name="n"/> cards from the top. Allocates an array of size n.
        /// </summary>
        /// <param name="n"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="n"/> is 
        /// </exception>
        /// <returns></returns>
        public Card[] Draw(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n), "Number of cards must be non-negative.");

            if (_index + n > _cards.Length)
                throw new InvalidOperationException($"Cannot draw {n} cards, only {Count} cards remain.");

            var result = new Card[n];
            Array.Copy(_cards, _index, result, 0, n);
            _index += n;
            return result;
        }
    }
}
