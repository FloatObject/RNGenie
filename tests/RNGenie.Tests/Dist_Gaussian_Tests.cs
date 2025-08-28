using RNGenie.Core.Abstractions;
using RNGenie.Core.Sources;
using RNGenie.Distributions.Continuous;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for Gaussian distribution.
    /// </summary>
    public class Dist_Gaussian_Tests
    {
        /// <summary>
        /// Tests exception with invalid standard deviations.
        /// </summary>
        [Fact]
        public void Constructor_Guards_Std()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Gaussian(0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Gaussian(0, -1));
        }

        /// <summary>
        /// Tests exception with null IRandomSource object.
        /// </summary>

        [Fact]
        public void Sample_Throws_On_Null_Rng()
        {
            var g = new Gaussian();
            Assert.Throws<ArgumentNullException>(() => g.Sample(null!));
        }

        /// <summary>
        /// Tests mean and standard deviation ranges with 200,000 samples.
        /// </summary>
        [Fact]
        public void Mean_And_Std_Are_Reasonable()
        {
            var rng = new Pcg32Source(7);
            var nrm = new Gaussian(mean: 3, stdDev: 2);

            const int N = 200_000;
            double sum = 0;
            double sumSq = 0;

            // Pair sampling encourages using both Box-Muller outputs if implemented with caching.
            for (int i = 0; i < N / 2; i++)
            {
                double x1 = nrm.Sample(rng);
                double x2 = nrm.Sample(rng);

                sum += x1 + x2;
                sumSq += x1 * x1 + x2 * x2;
            }

            // If N was odd, take one more sample here (not needed).
            //if ((N & 1) == 1)
            //{
            //    var x = nrm.Sample(rng);
            //    sum += x;
            //    sumSq += x * x;
            //}

            double mean = sum / N;
            double varianceUnbiased = (sumSq - N * mean * mean) / (N - 1);
            double std = Math.Sqrt(varianceUnbiased);

            // Statistical tolerance: mean should be within z * (sigma / sqrt(N)).
            double mu = 3.0;
            double sigma = 2.0;
            double tolMean = 5.0 * sigma / Math.Sqrt(N); // ~5-sigma on the mean-of-samples
            double tolStd = 0.02;                        // 1-2% is plenty at this N

            Assert.InRange(mean, mu - tolMean, mu + tolMean);
            Assert.InRange(std, sigma * (1 - tolStd), sigma * (1 + tolStd));
        }

        /// <summary>
        /// Tests Gaussian same-seed reproducibility.
        /// </summary>
        [Fact]
        public void Gaussian_ReproducibleWithSameSeed()
        {
            var a = new Pcg32Source(123);
            var b = new Pcg32Source(123);
            var n = new Gaussian(0, 1);

            var seqA = Enumerable.Range(0, 10).Select(_ => n.Sample(a)).ToArray();
            var seqB = Enumerable.Range(0, 10).Select(_ => n.Sample(b)).ToArray();

            Assert.Equal(seqA, seqB);
        }

        /// <summary>
        /// Simple IRandomSource implementation that allows for testing with bad RNG values.
        /// Used for the next test to ensure we resample until we get a value higher than 0.
        /// </summary>
        private sealed class SequenceDoubleRng : IRandomSource
        {
            private readonly double[] _vals;
            private int _i;

            public SequenceDoubleRng(params double[] v) => _vals = v;
            public double NextDouble() => _vals[_i++ % _vals.Length];
            public uint NextUInt() => 0;
            public int NextInt(int minInclusive, int maxExclusive) => minInclusive;
            public void NextBytes(Span<byte> buffer) => buffer.Clear();
            public ulong StateHash => 0;
        }

        /// <summary>
        /// Tests Gaussian resampling when the U1 value is 0.
        /// </summary>
        [Fact]
        public void Gaussian_Resamples_When_U1_Is_Zero()
        {
            var g = new Gaussian(0, 1);
            var rng = new SequenceDoubleRng(0.0, 0.42, 0.77, 0.33); // u1 == 0 forces resample.
            var x = g.Sample(rng);
            Assert.False(double.IsNaN(x) || double.IsInfinity(x));
        }
    }
}
