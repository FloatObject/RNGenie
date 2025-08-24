using RNGenie.Core.RNG;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for the Pcg32 RNG implementation.
    /// </summary>
    public class Rng_Pcg32_Tests
    {
        /// <summary>
        /// Tests range and bound exceptions of NextInt.
        /// </summary>
        [Fact]
        public void NextInt_InRange_And_Throws_On_Invalid_Bounds()
        {
            var rng = new Pcg32Source(1);
            for (int i = 0; i < 100_000; i++)
            {
                int v = rng.NextInt(-5, 7);
                Assert.InRange(v, -5, 6);
            }
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(3, 3));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(5, 2));
        }

        /// <summary>
        /// Tests the average of NextDouble with 200,000 samples.
        /// </summary>
        [Fact]
        public void NextDouble_Mean_Is_About_Half()
        {
            var rng = new Pcg32Source(123);
            const int N = 200_000;
            double sum = 0;
            for (int i = 0; i < N; i++) sum += rng.NextDouble();
            double mean = sum / N;
            Assert.InRange(mean, 0.495, 0.505);
        }

        /// <summary>
        /// Tests NextBytes functionality.
        /// </summary>
        [Fact]
        public void NextBytes_Fills_Buffer()
        {
            var rng = new Pcg32Source(42);
            var buf = new byte[32];
            rng.NextBytes(buf);
            Assert.Contains(buf, b => b != 0); // weak sanity check
        }

        /// <summary>
        /// Tests seed functionality. Same seeds, same sequences.
        /// </summary>
        [Fact]
        public void Determinism_Same_Seed_Same_Sequence()
        {
            var a = new Pcg32Source(999);
            var b = new Pcg32Source(999);

            for (int i = 0; i < 1000; i++)
                Assert.Equal(a.NextInt(0, int.MaxValue), b.NextInt(0, int.MaxValue));
        }

        /// <summary>
        /// Tests save and restore functionality.
        /// </summary>
        [Fact]
        public void Save_And_Restore_Reproduces_Sequence()
        {
            var rng = new Pcg32Source(777);
            // advance some steps
            int before = rng.NextInt(0, 1000);
            var state = rng.Save();
            int a1 = rng.NextInt(0, 1000);
            int a2 = rng.NextInt(0, 1000);

            rng.Restore(state);
            int b1 = rng.NextInt(0, 1000);
            int b2 = rng.NextInt(0, 1000);

            Assert.NotEqual(before, a1);
            Assert.Equal(a1, b1);
            Assert.Equal(a2, b2);
        }

        /// <summary>
        /// Tests Fork vs NewStreamFromSeed by comparing future values.
        /// Fork() should match the main RNG's next value, whereas NewStreamFromSeed should be independent immediately.
        /// </summary>
        [Fact]
        public void Fork_Vs_NewStreamFromSeed_Behavior()
        {
            var rng = new Pcg32Source(100);

            // Advance the state a bit so we are past the initial position.
            rng.NextInt(0, 100);

            // Same-stream fork from current state.
            var sameFork = rng.Fork();

            // Verify same-stream fork reproduces next main RNG draw.
            var nextMain = rng.NextInt(0, 100);
            var firstFork = sameFork.NextInt(0, 100);
            Assert.Equal(nextMain, firstFork);

            // Independent stream from original seed.
            var stream = rng.NewStreamFromSeed(2);

            // Compare several draws to avoid coincidence. Sequences should differ.
            bool anyDifference = false;
            for (int i = 0; i < 10; i++)
            {
                var a = sameFork.NextInt(0, 100);
                var b = stream.NextInt(0, 100);
                if (a != b) anyDifference = true;
            }

            Assert.True(anyDifference, "NewStreamFromSeed should be independent of Fork().");
        }

        /// <summary>
        /// Tests branching/forking functionality by comparing fork results to main RNG results.
        /// Fork() should branch from current state with the same stream (next values match).
        /// Fork(streamId) should branch with a different stream (independent immediately).
        /// </summary>
        [Fact]
        public void Fork_From_State_Branches_Timeline()
        {
            var rng = new Pcg32Source(1000);

            // Move state forward a bit.
            rng.NextInt(0, 1000);

            // Same-stream fork: clone at this point.
            var sameFork = rng.Fork();

            // Verify next values match between fork and main.
            var nextMain = rng.NextInt(0, 1000);
            var firstFork = sameFork.NextInt(0, 1000);
            Assert.Equal(nextMain, firstFork);

            // Verify continued matching as both are advanced in lockstep.
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(rng.NextInt(0, 1000), sameFork.NextInt(0, 1000));
            }

            // Different-stream fork: independent immediately.
            var diffFork = rng.Fork(2); // any id different from the original.

            // Compare several draws. Sequences should not be identical.
            bool anyDifference = false;
            for (int i = 0; i < 10; i++)
            {
                var a = rng.NextInt(0, 1000);
                var b = diffFork.NextInt(0, 1000);
                if (a != b) anyDifference = true;
            }

            Assert.True(anyDifference, "Different-stream fork should diverge immediately.");
        }

        /// <summary>
        /// Tests NewStreamFromSeed reproducibility.
        /// </summary>
        [Fact]
        public void NewStreamFromSeed_Is_Independent_And_Reproducible()
        {
            var rng = new Pcg32Source(2024);

            // Same seed + same streamId -> identical sequences
            var s1 = rng.NewStreamFromSeed(10);
            var s2 = rng.NewStreamFromSeed(10);
            for (int i = 0; i < 100; i++)
                Assert.Equal(s1.NextInt(0, 100000), s2.NextInt(0, 100000));

            // Different streamId -> different sequence (probabilistic check)
            var s3 = rng.NewStreamFromSeed(11);
            int diffs = 0;
            for (int i = 0; i < 100; i++)
                if (s1.NextInt(0, 100000) != s3.NextInt(0, 100000)) diffs++;
            Assert.True(diffs > 50); // loose threshold to avoid flakiness
        }
    }
}
