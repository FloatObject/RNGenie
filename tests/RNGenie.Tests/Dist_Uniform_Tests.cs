using RNGenie.Core.Sources;
using RNGenie.Distributions.Continuous;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for uniform distribution.
    /// </summary>
    public class Dist_Uniform_Tests
    {
        /// <summary>
        /// Tests exceptions when given invalid parameters.
        /// </summary>
        [Fact]
        public void Constructor_Validates_Parameters()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Uniform(5, 5));  // max == min
            Assert.Throws<ArgumentOutOfRangeException>(() => new Uniform(10, 0)); // max < min
            Assert.Throws<ArgumentOutOfRangeException>(() => new Uniform(double.NaN, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Uniform(0, double.PositiveInfinity));
        }

        /// <summary>
        /// Samples stay in [Min, Max) and are deterministic for the same seed.
        /// </summary>
        [Fact]
        public void Samples_Are_In_Range_And_Deterministic()
        {
            var u = new Uniform(-2, 3);
            var a = new Pcg32Source(123);
            var b = new Pcg32Source(123);

            for (int i = 0; i < 10_000; i++)
            {
                double x1 = u.Sample(a);
                double x2 = u.Sample(b);

                Assert.Equal(x1, x2);                   // same-seed reproducibility
                Assert.True(x1 >= u.Min && x1 < u.Max); // half-open interval
            }
        }

        /// <summary>
        /// Mean and variance are reasonable on [0,1).
        /// </summary>
        [Fact]
        public void Moments_Are_Reasonable_UnitInterval()
        {
            var rng = new Pcg32Source(321);
            var u = new Uniform(); // [0,1)

            const int N = 100_000;
            double sum = 0, sum2 = 0;

            for (int i = 0; i < N; i++)
            {
                double x = u.Sample(rng);
                sum += x;
                sum2 += x * x;
            }

            double mean = sum / N;
            double var = sum2 / N - mean * mean;

            double expectedMean = 0.5;
            double expectedVar = 1.0 / 12.0;

            // ~5σ bound on the sample mean, σ_mean = sqrt(var / N)
            double tolMean = 5.0 * Math.Sqrt(expectedVar / N);
            Assert.InRange(mean, expectedMean - tolMean, expectedMean + tolMean);

            // Variance within ±5%
            Assert.InRange(var, expectedVar * 0.95, expectedVar * 1.05);
        }

        /// <summary>
        /// Mean and variance are reasonable on an arbitrary interval.
        /// </summary>
        [Fact]
        public void Moments_Are_Reasonable_ArbitraryInterval()
        {
            double min = -1.5, max = 2.7;
            var u = new Uniform(min, max);
            var rng = new Pcg32Source(42);

            const int N = 100_000;
            double sum = 0, sum2 = 0;

            for (int i = 0; i < N; i++)
            {
                double x = u.Sample(rng);
                sum += x;
                sum2 += x * x;
            }

            double mean = sum / N;
            double var = sum2 / N - mean * mean;

            double expectedMean = (min + max) / 2.0;
            double expectedVar = (max - min) * (max - min) / 12.0;

            double tolMean = 5.0 * Math.Sqrt(expectedVar / N);
            Assert.InRange(mean, expectedMean - tolMean, expectedMean + tolMean);
            Assert.InRange(var, expectedVar * 0.95, expectedVar * 1.05);
        }

        /// <summary>
        /// Static convenience method produces the same value as an instance with the same seed.
        /// </summary>
        [Fact]
        public void Static_Sample_Equals_Instance_Sample()
        {
            const double min = -2, max = 5;
            var r1 = new Pcg32Source(99);
            var r2 = new Pcg32Source(99);

            double a = Uniform.Sample(r1, min, max);
            double b = new Uniform(min, max).Sample(r2);

            Assert.Equal(a, b);
        }
    }
}
