namespace RNGenie.RNG;

// Minimal PCG32 implementation (XSH RR)
public sealed class Pcg32Source : IRandomSource
{
    private ulong _state;
    private readonly ulong _inc;

    public Pcg32Source(ulong seed = 0x853C49E6748FEA9B, ulong seq = 0xDA3E39CB94B95BDB)
    {
        _state = 0;
        _inc = (seq << 1) | 1UL;
        NextUInt(); // warm up
        _state += seed;
        NextUInt();
    }

    private uint NextUInt()
    {
        ulong oldstate = _state;
        _state = unchecked(oldstate * 6364136223846793005UL + _inc);
        uint xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);
        int rot = (int)(oldstate >> 59);
        return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
    }

    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive) throw new ArgumentOutOfRangeException();
        uint range = (uint)(maxExclusive - minInclusive);
        // bounded
        uint threshold = (uint)(-range) % range;
        uint r;
        do { r = NextUInt(); } while (r < threshold);
        return (int)(r % range) + minInclusive;
    }

    public double NextDouble() => (NextUInt() >> 11) * (1.0 / (1UL << 53));
    public ulong StateHash => _state;
}
