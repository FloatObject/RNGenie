using RNGenie.Core.Dist;
using RNGenie.Core.RNG;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for uniform distribution.
    /// </summary>
    public class Dist_Uniform_Tests
    {
        /// <summary>
        /// Tests the mean and range with 100,000 samples.
        /// </summary>
        [Fact]
        public void Uniform01_Within_Zero_One_And_Mean_Half()
        {
            var rng = new Pcg32Source(321);
            var u = new Uniform01();

            const int N = 100_000;
            double sum = 0;
            for (int i = 0; i < N; i++)
            {
                double d = u.Sample(rng);
                Assert.True(d >= 0 && d < 1);
                sum += d;
            }
            double mean = sum / N;
            Assert.InRange(mean, 0.495, 0.505);
        }
    }
}
