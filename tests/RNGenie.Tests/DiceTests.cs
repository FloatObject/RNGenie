using RNGenie.Dice;
using RNGenie.RNG;
using Xunit;

public class DiceTests
{
    [Fact]
    public void ThreeDSixPlusTwoRange()
    {
        var rng = new Pcg32Source(42, 99);
        for (int i = 0; i < 1000; i++)
        {
            var (total, _, _) = Dice.Roll("3d6+2", rng);
            Assert.InRange(total, 5, 20);
        }
    }
}
