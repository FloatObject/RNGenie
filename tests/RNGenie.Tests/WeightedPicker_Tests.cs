using RNGenie.Core.Picks;
using RNGenie.Core.Sources;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for the Weighted Picker.
    /// </summary>
    public class WeightedPicker_Tests
    {
        /// <summary>
        /// Tests exception when no items are provided.
        /// </summary>
        [Fact]
        public void Throws_When_No_Items()
        {
            var rng = new Pcg32Source(1);
            var p = new WeightedPicker<string>();
            Assert.Throws<InvalidOperationException>(() => p.One(rng));
        }

        /// <summary>
        /// Tests exception when weight is not greater than 0.
        /// </summary>
        [Fact]
        public void Throws_When_Weight_NonPositive()
        {
            var p = new WeightedPicker<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Add(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Add(2, -1));
        }

        /// <summary>
        /// Tests weight effectiveness.
        /// </summary>
        [Fact]
        public void Frequencies_Roughly_Match_Weights()
        {
            var rng = new Pcg32Source(123);
            var p = new WeightedPicker<string>()
                .Add("A", 0.8)
                .Add("B", 0.2);

            int a = 0, b = 0;
            for (int i = 0; i < 100_000; i++)
                if (p.One(rng) == "A") a++; else b++;

            double propB = b / (double)(a + b);
            Assert.InRange(propB, 0.17, 0.23); // loose, not too tight
        }
    }
}
