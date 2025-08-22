using System.Security.Cryptography;

namespace RNGenie.RNG;

public sealed class SystemRandomSource : IRandomSource
{
    private readonly Random _r;

    public SystemRandomSource(int? seed = null)
        => _r = seed is null ? Random.Shared : new Random(seed.Value);

    public int NextInt(int minInclusive, int maxExclusive) => _r.Next(minInclusive, maxExclusive);
    public double NextDouble() => _r.NextDouble();
    public ulong StateHash => 0; // non-deterministic across runtimes; placeholder
}
