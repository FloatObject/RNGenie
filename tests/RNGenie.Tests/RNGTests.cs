using RNGenie.Core.RNG;
using Xunit;

public class RngTests
{
    [Fact]
    public void NextDoubleLooksUniformish()
    {
        var rng = new Pcg32Source(123, 456);
        const int N = 200_000;
        double sum = 0;
        for (int i = 0; i < N; i++)
            sum += rng.NextDouble();

        double mean = sum / N;
        // Uniform[0,1) mean should be ~0.5, allow tolerance.
        Assert.InRange(mean, 0.495, 0.505);
    }
}
