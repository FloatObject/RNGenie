using RNGenie.Core.RNG;
using Xunit;

/// <summary>
/// Test suite for the System.Random RNG implementation.
/// </summary>
public class Rng_System_Tests
{
    /// <summary>
    /// Tests range and bound exceptions of NextInt.
    /// </summary>
    [Fact]
    public void NextInt_Range_And_Throws()
    {
        var rng = new SystemRandomSource(1);
        for (int i = 0; i < 10_000; i++)
        {
            int v = rng.NextInt(0, 10);
            Assert.InRange(v, 0, 9);
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(2, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt(5, 3));
    }

    /// <summary>
    /// Tests NextBytes functionality.
    /// </summary>
    [Fact]
    public void NextBytes_Fills_Buffer()
    {
        var rng = new SystemRandomSource(123);
        var buf = new byte[16];
        rng.NextBytes(buf);
        Assert.Contains(buf, b => b != 0);
    }
}
