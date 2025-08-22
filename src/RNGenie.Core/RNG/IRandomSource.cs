namespace RNGenie.Core.RNG;

public interface IRandomSource
{
    int NextInt(int minInclusive, int maxExclusive);
    double NextDouble(); // [0,1)
    ulong StateHash { get; } // optional sanity/value for determinism checks
}
