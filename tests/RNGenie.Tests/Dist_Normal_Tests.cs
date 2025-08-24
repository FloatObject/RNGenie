using RNGenie.Core.Dist;
using RNGenie.Core.RNG;
using Xunit;

/// <summary>
/// Test suite for normal distribution.
/// </summary>
public class Dist_Normal_Tests
{
    /// <summary>
    /// Tests exception with invalid standard deviations.
    /// </summary>
    [Fact]
    public void Constructor_Guards_Std()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NormalBoxMuller(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NormalBoxMuller(0, -1));
    }

    /// <summary>
    /// Tests mean and standard deviation ranges with 200,000 samples.
    /// </summary>
    [Fact]
    public void Mean_And_Std_Are_Reasonable()
    {
        var rng = new Pcg32Source(7);
        var n = new NormalBoxMuller(3, 2);
        const int N = 200_000;

        double sum = 0, sum2 = 0;
        for (int i = 0; i < N; i++)
        {
            double x = n.Sample(rng);
            sum += x;
            sum2 += x * x;
        }
        double mean = sum / N;
        double variance = (sum2 / N) - mean * mean;
        double std = Math.Sqrt(variance);

        Assert.InRange(mean, 2.9, 3.1);
        Assert.InRange(std, 1.9, 2.1);
    }
}
