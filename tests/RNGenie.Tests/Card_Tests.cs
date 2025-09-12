using RNGenie.Cards;
using RNGenie.Core.Sources;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for Cards.
    /// </summary>
    public class CardTests
    {

        // ----------- Helpers ------------
        private static bool IsPermutation<T>(IReadOnlyList<T> a, IReadOnlyList<T> b) where T : notnull
        {
            if (a.Count != b.Count) return false;
            var map = new Dictionary<T, int>();

            foreach (var x in a)
                map[x] = map.TryGetValue(x, out var c) ? c + 1 : 1;

            foreach (var y in b)
            {
                if (!map.TryGetValue(y, out var c)) return false;

                if (c == 1)
                    map.Remove(y);
                else
                    map[y] = c - 1;
            }
            return map.Count == 0;
        }

        private static Card[] AllStandard52()
        {
            var list = new List<Card>(52);
            for (int s = 0; s < 4; s++)
            {
                for (int r = 1; r <= 13; r++)
                {
                    list.Add(new Card((Suite)s, (Rank)r));
                }
            }
            return list.ToArray();
        }

        // ------------- Correctness --------------

        [Fact]
        public void NewDeck_Has52UniqueCards_NoJokers()
        {
            var d = new Deck();

            Assert.Equal(52, d.Capacity);
            Assert.Equal(52, d.Count);

            Card[] remaining = d.RemainingSpan.ToArray();

            Assert.Equal(52, remaining.Length);
            Assert.Equal(52, remaining.Distinct().Count());

            Assert.True(remaining.SequenceEqual(AllStandard52())); // Original order check.
        }

        [Fact]
        public void NewDeck_IncludeTwoDistinctJokers()
        {
            var d = new Deck(includeJokers: true);

            Assert.Equal(54, d.Capacity);
            Assert.Equal(54, d.Count);

            Card[] rem = d.RemainingSpan.ToArray();

            Assert.Equal(54, rem.Length);
            Assert.Equal(54, rem.Distinct().Count());

            // Last two are jokers and distinct.
            Assert.True(rem[^2].IsJoker);
            Assert.True(rem[^1].IsJoker);
            Assert.NotEqual(rem[^2], rem[^1]);
        }

        [Fact]
        public void Shuffle_IsPermutation_And_ResetsIndex()
        {
            var d = new Deck();
            var rng = new Pcg32Source(123);

            Card[] before = d.RemainingSpan.ToArray();
            d.Draw(); // Advance the index.
            Assert.Equal(51, d.Count);

            // Cards shuffled back together, index reset.
            d.Shuffle(rng);
            Assert.Equal(d.Capacity, d.Count);
            Card[] after = d.RemainingSpan.ToArray();

            Assert.True(IsPermutation(before, after));
            Assert.False(before.SequenceEqual(after)); // Extremely likely.
        }

        [Fact]
        public void Draw_SingleAndMultiple_ConsumeAndReturn_CorrectCards()
        {
            var d = new Deck();
            Card top = d.Peek();
            Card drawn1 = d.Draw();

            Assert.Equal(top, drawn1);
            Assert.Equal(51, d.Count);

            Card[] next5 = d.Draw(5);

            Assert.Equal(5, next5.Length);
            Assert.Equal(46, d.Count);

            // next5 should equal the next 5 originally.
            IEnumerable<Card> expected = new Deck().RemainingSpan.ToArray().Skip(1).Take(5);
            Assert.True(next5.SequenceEqual(expected));
        }

        [Fact]
        public void DrawZero_ReturnsEmpty_And_DoesNotAdvance()
        {
            var d = new Deck();
            int before = d.Count;
            Card[] arr = d.Draw(0);

            Assert.Empty(arr);
            Assert.Equal(before, d.Count);
        }

        [Fact]
        public void Peek_DoesNotConsume()
        {
            var d = new Deck();
            Card a = d.Peek();
            Card b = d.Peek();

            Assert.Equal(a, b);
            Assert.Equal(52, d.Count);
        }

        [Fact]
        public void Reset_RestoresOriginalOrder()
        {
            var baseline = new Deck();
            var d = new Deck();
            var rng = new Pcg32Source(7);

            d.Shuffle(rng);
            d.Draw(17);
            d.Reset();

            Assert.True(d.RemainingSpan.SequenceEqual(baseline.RemainingSpan));
        }

        [Fact]
        public void RemainingSpan_And_RemainingEnumerator_Agree()
        {
            var d = new Deck(includeJokers: true);
            d.Draw(7);

            Assert.Equal(d.Count, d.RemainingSpan.Length);
            Assert.Equal(d.RemainingSpan.ToArray(), d.Remaining.ToArray());
        }

        // ----------- Reproducibility -------------

        [Fact]
        public void Deterministic_ShuffleWithSeed_MatchesAcrossDecks()
        {
            var rng1 = new Pcg32Source(999);
            var rng2 = new Pcg32Source(999);

            var a = new Deck(includeJokers: true);
            var b = new Deck(includeJokers: true);

            a.Shuffle(rng1);
            b.Shuffle(rng2);

            Assert.Equal(a.RemainingSpan.ToArray(), b.RemainingSpan.ToArray());

            // And drawing sequence matches.
            Card[] seqA = Enumerable.Range(0, 10).Select(_ => a.Draw()).ToArray();
            Card[] seqB = Enumerable.Range(0, 10).Select(_ => b.Draw()).ToArray();
            Assert.Equal(seqA, seqB);
        }

        // -------------- ShuffleRemaining --------------

        [Fact]
        public void ShuffleRemaining_OnlyPermutesSuffix_And_DoesNotResetIndex()
        {
            var d = new Deck();
            var rng = new Pcg32Source(42);

            // Draw a prefix.
            Card[] drawn = d.Draw(13);
            Card[] beforeSuffix = d.RemainingSpan.ToArray();

            d.ShuffleRemaining(rng);

            // Drawn prefix is unchanged and index unchanged.
            Assert.Equal(39, d.Count);
            Assert.Equal(drawn, new Deck().Draw(13)); // The same 13 from a fresh deck.
            Assert.Equal(13, 52 - d.Count);           // Index unchanged (not reset).

            // Suffix is a permutation of previous suffix.
            Card[] afterSuffix = d.RemainingSpan.ToArray();
            Assert.True(IsPermutation(beforeSuffix, afterSuffix));

            // Prefix cards do not appear in the remaining array.
            Assert.False(afterSuffix.Intersect(drawn).Any());
        }

        // --------- Edge cases and Exceptions -----------

        [Fact]
        public void DrawPastEnd_Throws()
        {
            var d = new Deck();
            d.Draw(52);
            Assert.Throws<InvalidOperationException>(() => d.Draw());
            Assert.Throws<InvalidOperationException>(() => d.Draw(1));
        }

        [Fact]
        public void PeekOnEmpty_Throws()
        {
            var d = new Deck();
            d.Draw(52);
            Assert.Throws<InvalidOperationException>(() => d.Peek());
        }

        [Fact]
        public void Shuffle_NullRNG_Throws()
        {
            var d = new Deck();
            Assert.Throws<ArgumentNullException>(() => d.Shuffle(null!));
            Assert.Throws<ArgumentNullException>(() => d.ShuffleRemaining(null!));
        }

        // --------- Custom Deck Compositions ----------

        [Fact]
        public void CustomComposition_Null_Throws()
        {
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Deck(initialCards: null!));
            Assert.Equal("initialCards", ex.ParamName);
        }

        [Fact]
        public void CustomComposition_Empty_Throws()
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => new Deck(Array.Empty<Card>()));
            Assert.Equal("initialCards", ex.ParamName);
            Assert.Contains("at least one card", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CustomComposition_CountAndCapacity_MatchInput()
        {
            var cards = new List<Card>
            {
                new Card((Suite)0, (Rank)1),  // A♠
                new Card((Suite)1, (Rank)13), // K♥
                Card.Joker(1)
            };

            var d = new Deck(cards);

            Assert.Equal(cards.Count, d.Capacity);
            Assert.Equal(cards.Count, d.Count);
        }

        [Fact]
        public void CustomComposition_PreservesInitialOrder_BeforeShuffle()
        {
            var cards = new List<Card>
            {
                new Card((Suite)0, (Rank)1),   // A♠
                new Card((Suite)2, (Rank)7),   // 7♦
                new Card((Suite)3, (Rank)10),  // 10♣
                Card.Joker(2)
            };

            var d = new Deck(cards);

            // RemainingSpan should match the exact input order.
            Assert.True(cards.SequenceEqual(d.RemainingSpan.ToArray()));
        }

        [Fact]
        public void CustomComposition_Reset_RestoresOriginalOrder()
        {
            var cards = new List<Card>
            {
                new Card((Suite)0, (Rank)1),
                new Card((Suite)1, (Rank)5),
                new Card((Suite)2, (Rank)9),
                new Card((Suite)3, (Rank)13),
                Card.Joker(1)
            };

            var d = new Deck(cards);

            // Shuffle, then Reset should restore original order.
            d.Shuffle(new Pcg32Source(123));
            d.Reset();

            Assert.True(cards.SequenceEqual(d.RemainingSpan.ToArray()));
        }

        [Fact]
        public void CustomComposition_Shuffle_IsDeterministic_WithSameSeed()
        {
            // Short-deck style (6..A) to ensure it's clearly non-standard.
            var build = new Func<List<Card>>(() =>
                (from s in Enumerable.Range(0, 4)
                 from r in Enumerable.Range(6, 8) // 6..13
                 select new Card((Suite)s, (Rank)r)).ToList()
            );

            var a = new Deck(build());
            var b = new Deck(build());

            var rngA = new Pcg32Source(42);
            var rngB = new Pcg32Source(42);

            a.Shuffle(rngA);
            b.Shuffle(rngB);

            Assert.Equal(a.RemainingSpan.ToArray(), b.RemainingSpan.ToArray());
        }

        [Fact]
        public void CustomComposition_ShuffleRemaining_DoesNotReintroduceDrawn()
        {
            var cards = (from s in Enumerable.Range(0, 4)
                         from r in Enumerable.Range(1, 13)
                         select new Card((Suite)s, (Rank)r)).ToList();

            var d = new Deck(cards);

            // Draw a few cards from the custom composition.
            var drawn = d.Draw(7);
            var preCount = d.Count;

            d.ShuffleRemaining(new Pcg32Source(7));

            // Count unchanged; drawn cards are not reintroduced.
            Assert.Equal(preCount, d.Count);

            var drawnSet = new HashSet<Card>(drawn);
            Assert.DoesNotContain(d.Remaining, c => drawnSet.Contains(c));
        }
    }
}