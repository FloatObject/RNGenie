namespace RNGenie.Cards
{
    /// <summary>
    /// Immutable card value. Represents either a standard card (Suit+Rank) or a Joker.
    /// Two Jokers are distinct (JokerId 1 or 2) so a 54-card deck has unique values.
    /// </summary>
    public readonly struct Card : IEquatable<Card>
    {
        /// <summary>True if this card is a Joker.</summary>
        public bool IsJoker { get; }
        /// <summary>1..2 for distinct jokers when <see cref="IsJoker"/> is true, otherwise 0.</summary>
        public byte JokerId { get; }
        /// <summary>Suite of a standard card. Undefined when <see cref="IsJoker"/> is true.</summary>
        public Suite Suite { get; }
        /// <summary>Rank of a standard card. Undefined when <see cref="IsJoker"/> is true.</summary>
        public Rank Rank { get; }

        /// <summary>
        /// Create a standard (non-joker) card.
        /// </summary>
        /// <param name="suite">Suite of the card.</param>
        /// <param name="rank">Rank of the card.</param>
        public Card(Suite suite, Rank rank)
        {
            IsJoker = false;
            JokerId = 0;
            Suite = suite;
            Rank = rank;
        }

        private Card(bool isJoker, byte jokerId, Suite suite, Rank rank)
        {
            IsJoker = isJoker;
            JokerId = jokerId;
            Suite = suite;
            Rank = rank;
        }

        /// <summary>
        /// Create a distinct Joker (id must be &gt;= 1).
        /// </summary>
        /// <param name="id">The Joker's id.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="id"/> &lt; 1.
        /// </exception>
        /// <returns></returns>
        public static Card Joker(byte id)
        {
            if (id == 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Joker id must be >= 1.");

            return new Card(isJoker: true, jokerId: id, suite: default, rank: default);
        }

        /// <summary>
        /// ToString() override for custom card formatting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (IsJoker) return $"Joker (★{JokerId})";

            string rankStr = Rank switch
            {
                Rank.Ace => "A",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                _ => ((byte)Rank).ToString()
            };
            string suiteStr = Suite switch
            {
                Suite.Clubs => "♣",
                Suite.Diamonds => "♦",
                Suite.Hearts => "♥",
                Suite.Spades => "♠",
                _ => "?"
            };

            return $"{rankStr}{suiteStr}";
        }

        /// <summary>
        /// Compares two card instances.
        /// </summary>
        /// <param name="other">The card to compare with this one.</param>
        /// <returns>True if cards match.</returns>
        public bool Equals(Card other)
            => IsJoker ? other.IsJoker && JokerId == other.JokerId : !other.IsJoker && Suite == other.Suite && Rank == other.Rank;

        /// <summary>
        /// Compares two card instances.
        /// </summary>
        /// <param name="obj">The card to compare with this one.</param>
        /// <returns>True if cards match.</returns>
        public override bool Equals(object? obj) => obj is Card c && Equals(c);

        /// <summary>
        /// Compares two card instances.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True if cards match.</returns>
        public static bool operator ==(Card left, Card right) => left.Equals(right);

        /// <summary>
        /// Compares two card instances.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>True if cards are different.</returns>
        public static bool operator !=(Card left, Card right) => !left.Equals(right);

        /// <summary>
        /// Returns a hash code for this card instance.
        /// </summary>
        /// <remarks>
        /// Two cards that are considered equal by <see cref="Equals(Card)"/>
        /// will return the same hash code.
        /// 
        /// - For standard cards, the hash is based on <see cref="Suite"/> and <see cref="Rank"/>.
        /// - For jokers, the hash is based on their distinct <see cref="JokerId"/>.
        /// 
        /// This method is suitable for use in hash-based collections 
        /// such as <see cref="Dictionary{TKey, TValue}"/> and <see cref="HashSet{T}"/>.
        /// </remarks>
        /// <returns>
        /// An integer hash code that uniquely represents this card's identity.
        /// </returns>
        public override int GetHashCode() => IsJoker ? HashCode.Combine(1, JokerId) : HashCode.Combine(0, Suite, Rank);
    }
}
