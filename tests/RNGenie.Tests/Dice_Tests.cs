using RNGenie.Core.Dice;
using RNGenie.Core.RNG;

namespace RNGenie.Tests
{
    /// <summary>
    /// Test suite for Dice.
    /// </summary>
    public class Dice_Tests
    {
        /// <summary>
        /// Tests that rolls work properly with common RPG notations.
        /// </summary>
        [Fact]
        public void Parses_And_Rolls_Common_Notations()
        {
            var rng = new Pcg32Source(99);

            var (t1, rolls1, mod1) = Dice.Roll("3d6+2", rng);
            Assert.Equal(3, rolls1.Length);
            Assert.Equal(2, mod1);
            Assert.InRange(t1, 5, 20);

            var (t2, rolls2, _) = Dice.Roll("1d20", rng);
            Assert.Single(rolls2);
            Assert.InRange(rolls2[0], 1, 20);
            Assert.InRange(t2, 1, 20);
        }

        /// <summary>
        /// Tests exception with bad notation.
        /// </summary>
        /// <param name="s"></param>
        [Theory]
        [InlineData("bad")]
        [InlineData("d6")]
        [InlineData("3d")]
        [InlineData("0d6")]
        [InlineData("2d1")]
        public void Throws_On_Invalid_Notation(string s)
        {
            var rng = new Pcg32Source(1);
            Assert.ThrowsAny<Exception>(() => Dice.Roll(s, rng));
        }
    }
}
