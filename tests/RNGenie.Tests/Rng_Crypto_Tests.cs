using RNGenie.Core.RNG;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for the System.Security.Cryptography RNG implementation.
    /// </summary>
    public class Rng_Crypto_Tests
    {
        /// <summary>
        /// Tests range and bound exceptions of NextInt.
        /// </summary>
        [Fact]
        public void NextInt_Range_And_Throws()
        {
            using var rng = new CryptoRandomSource();
            for (int i = 0; i < 10_000; i++)
            {
                int v = rng.NextInt(-3, 9);
                Assert.InRange(v, -3, 8);
            }
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(5, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(7, 6));
        }

        /// <summary>
        /// Tests the range of NextDouble with 50,000 samples.
        /// </summary>
        [Fact]
        public void NextDouble_In_Zero_One()
        {
            using var rng = new CryptoRandomSource();
            for (int i = 0; i < 50_000; i++)
            {
                double d = rng.NextDouble();
                Assert.True(d >= 0 && d < 1);
            }
        }

        /// <summary>
        /// Tests NextBytes functionality.
        /// </summary>
        [Fact]
        public void NextBytes_Fills_Buffer()
        {
            using var rng = new CryptoRandomSource();
            var buf = new byte[32];
            rng.NextBytes(buf);
            Assert.Contains(buf, b => b != 0);
        }
    }
}