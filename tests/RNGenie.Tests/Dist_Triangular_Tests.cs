using RNGenie.Core.Dist;
using RNGenie.Core.RNG;
using Xunit;

/// <summary>
/// Test suite for triangular distribution.
/// </summary>
public class Dist_Triangular_Tests
{
    /// <summary>
    /// Tests exceptions with invalid min/mode/max.
    /// </summary>
    [Fact]
    public void Constructor_Validates_Parameters()
    {
        Assert.Throws<ArgumentException>(() => new Triangular(5, 2, 4)); // mode < min
        Assert.Throws<ArgumentException>(() => new Triangular(0, 10, 5)); // mode > max
    }

    /// <summary>
    /// Tests range with 50,000 samples.
    /// </summary>
    [Fact]
    public void Samples_Are_In_Range()
    {
        var rng = new Pcg32Source(11);
        var tri = new Triangular(0, 5, 10);
        for (int i = 0; i < 50_000; i++)
        {
            double x = tri.Sample(rng);
            Assert.InRange(x, 0, 10);
        }
    }
}
