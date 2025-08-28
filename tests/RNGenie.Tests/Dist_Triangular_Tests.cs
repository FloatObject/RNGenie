using RNGenie.Core.Abstractions;
using RNGenie.Core.Sources;
using RNGenie.Distributions.Continuous;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for triangular distribution.
    /// </summary>
    public class Dist_Triangular_Tests
    {
        /// <summary>
        /// Tests exceptions with invalid min/mode/max.
        /// </summary>
        [Fact]
        public void Constructor_Validates_Parameters()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(5, 6, 5));                         // max == min
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(5, 6, 4));                         // max  < min
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(5, 2, 6));                         // mode < min
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(0, 10, 5));                        // mode > max
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(double.NaN, 0, 1));                // non-finite
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(0, double.NaN, 1));                // non-finite
            Assert.Throws<ArgumentOutOfRangeException>(() => new Triangular(0, 0.5, double.PositiveInfinity)); // non-finite
        }

        /// <summary>
        /// Tests range with 50,000 samples.
        /// </summary>
        [Fact]
        public void Samples_Are_In_Range_HalfOpen()
        {
            var rng = new Pcg32Source(11);
            var tri = new Triangular(0, 5, 10);
            for (int i = 0; i < 50_000; i++)
            {
                double x = tri.Sample(rng);
                Assert.True(x >= tri.Min && x < tri.Max); // [Min, Max)
            }
        }

        /// <summary>
        /// Tests same-seed reproducibility with 1000 samples.
        /// </summary>
        [Fact]
        public void Deterministic_With_Same_Seed()
        {
            var a = new Pcg32Source(123);
            var b = new Pcg32Source(123);
            var tri = new Triangular(0, 2, 5);

            for (int i = 0; i < 1000; i++)
            {
                Assert.Equal(tri.Sample(a), tri.Sample(b));
            }
        }

        /// <summary>
        /// Tests mean and standard deviation ranges with 200_000 samples.
        /// </summary>
        [Fact]
        public void Mean_and_Std_Are_Reasonable()
        {
            double min = 0, mode = 2, max = 5;
            var tri = new Triangular(min, mode, max);
            var rng = new Pcg32Source(7);

            const int N = 200_000;
            double sum = 0, sum2 = 0;

            for (int i = 0; i < N; i++)
            {
                double x = tri.Sample(rng);
                sum += x; sum2 += x * x;
            }

            double mean = sum / N;
            double var = sum2 / N - mean * mean;

            double expectedMean = (min + mode + max) / 3.0;
            double expectedVar = (min * min + mode * mode + max * max - min * mode - min * max - mode * max) / 18.0;

            Assert.InRange(mean, expectedMean - 0.02, expectedMean + 0.02);
            Assert.InRange(var, expectedVar * 0.95, expectedVar * 1.05);
        }

        /// <summary>
        /// Simple IRandomSource implementation that allows us to test with bad RNG values.
        /// Used in the next test for testing RNG values outside of [0,1).
        /// </summary>
        private sealed class SequenceDoubleRng : IRandomSource
        {
            private readonly double[] _vals; private int _i;
            public SequenceDoubleRng(params double[] vals) => _vals = vals;
            public double NextDouble() => _vals[_i++ % _vals.Length];
            public uint NextUInt() => 0;
            public int NextInt(int minInclusive, int maxExclusive) => minInclusive;
            public void NextBytes(Span<byte> buffer) => buffer.Clear();
            public ulong StateHash => 0;
        }

        /// <summary>
        /// Test to verify proper handling of out of range RNG values (from NextDouble).
        /// </summary>
        [Fact]
        public void Robust_To_OutOfRange_UnitUniform()
        {
            var tri = new Triangular(0.0, 0.25, 1.0);
            var rng = new SequenceDoubleRng(1.0, -0.1, 2.0, 0.5); // includes illegal values

            for (int i = 0; i < 4; i++)
            {
                var x = tri.Sample(rng);
                // With clamping of radicands, the value is guaranteed inside [Min, Max].
                // Could equal Max when u >= 1.
                Assert.InRange(x, tri.Min, tri.Max); // inclusive for robustness case
            }
        }
    }
}
