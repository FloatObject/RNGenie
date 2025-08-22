using RNGenie.Picks;
using RNGenie.RNG;
using Xunit;

public class WeightedPickerTests
{
    [Fact]
    public void PicksRoughlyFollowWeights()
    {
        var rng = new Pcg32Source(123, 456);
        var p = new WeightedPicker<string>()
            .Add("A", 0.8)
            .Add("B", 0.2);

        int a = 0, b = 0;
        for (int i = 0; i < 10000; i++)
            if (p.One(rng) == "A") a++; else b++;

        double ratio = b / (double)(a + b);
        Assert.InRange(ratio, 0.15, 0.25); // loose bounds
    }
}
