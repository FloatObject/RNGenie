using System;

namespace RNGenie.Core.RNG;

/// <summary>
/// Minimal <b>PCG32 (XSH-RR)</b> random number generator.
/// <para>
/// Deterministic and fast. Suitable for simulations and games. Not thread-safe.
/// </para>
/// <para>
/// Reference: O’Neill, “PCG: A Family of Simple Fast Space-Efficient Statistically Good Algorithms
/// for Random Number Generation.”
/// </para>
/// </summary>
public sealed class Pcg32Source : IRandomSource
{
    private ulong _state;
    private readonly ulong _inc;

    // Used with forking options.
    private readonly ulong _initialSeed;
    private readonly ulong _initialSeq;

    // Helper to derive increment from stream/sequence id.
    private static ulong SeqToInc(ulong seq) => (seq << 1) | 1UL;

    // Direct state/inc constructor (to avoid the usual warm-up).
    // Used for forking operations.
    private Pcg32Source(ulong state, ulong inc, bool direct)
    {
        _state = state;
        _inc = inc;
    }

    /// <summary>
    /// Creates a new PCG32 RNG with the given <paramref name="seed"/> and stream/sequence id <paramref name="seq"/>.
    /// <para>
    /// Different <paramref name="seq"/> values produce statistically independent streams.
    /// </para>
    /// </summary>
    /// <param name="seed">Initial 64-bit seed for the internal state.</param>
    /// <param name="seq">
    /// Stream/sequence identifier. Each distinct value defines an independent random stream.
    /// Internally transformed to an odd increment per the PCG algorithm.
    /// </param>
    public Pcg32Source(ulong seed = 0x853C49E6748FEA9B, ulong seq = 0xDA3E39CB94B95BDB)
    {
        _initialSeed = seed;
        _initialSeq = seq;

        _state = 0;
        _inc = SeqToInc(seq);
        NextUInt();           // warm-up
        _state += seed;
        NextUInt();
    }

    /// <summary>
    /// Convenience overload that creates a new RNG with the specified <paramref name="seed"/> and a default stream id.
    /// </summary>
    /// <param name="seed">Initial 64-bit seed for the internal state.</param>
    public Pcg32Source(ulong seed) : this(seed, 0xDA3E39CB94B95BDB) { }

    private uint NextUInt()
    {
        ulong oldstate = _state;
        _state = unchecked(oldstate * 6364136223846793005UL + _inc);
        uint xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);
        int rot = (int)(oldstate >> 59);
        return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
    }

    /// <inheritdoc />
    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "maxExclusive must be greater than minInclusive.");

        uint range = (uint)(maxExclusive - minInclusive);

        // Bounded rejection sampling (unbiased).
        uint threshold = (uint)(-range) % range;
        uint r;
        do 
        { 
            r = NextUInt(); 
        } 
        while (r < threshold);

        return (int)(r % range) + minInclusive;
    }

    /// <inheritdoc />
    public double NextDouble()
    {
        ulong hi = NextUInt();
        ulong lo = NextUInt() & 0x1FFFFF;   // 21 bits
        ulong m = (hi << 21) | lo;          // 53 bits
        return m * (1.0 / (1UL << 53));     // [0,1)
    }

    /// <inheritdoc />
    public void NextBytes(Span<byte> buffer)
    {
        int i = 0;
        while (i + 4 <= buffer.Length)
        {
            uint r = NextUInt();
            buffer[i++] = (byte)r;
            buffer[i++] = (byte)(r >> 8);
            buffer[i++] = (byte)(r >> 16);
            buffer[i++] = (byte)(r >> 24);
        }
        if (i < buffer.Length)
        {
            uint r = NextUInt();
            for (int b = 0; i < buffer.Length; i++, b += 8)
                buffer[i] = (byte)(r >> b);
        }
    }

    /// <summary>
    /// Serializes the current internal state (16 bytes: 8 for state, 8 for stream increment).
    /// <remarks>
    /// Useful for deterministic save/restore and replay systems.
    /// </remarks>
    /// </summary>
    /// <returns>Byte array containing the serialized state.</returns>
    public byte[] Save()
    {
        var bytes = new byte[16];
        BitConverter.TryWriteBytes(bytes.AsSpan(0, 8), _state);
        BitConverter.TryWriteBytes(bytes.AsSpan(8, 8), _inc);
        return bytes;
    }

    /// <summary>
    /// Restores the internal state from the given serialized bytes produced by <see cref="Save"/>.
    /// Note: this restores the state component only. The stream increment (<c>_inc</c>) is immutable
    /// after construction to preserve the original stream identity.
    /// </summary>
    /// <param name="state">A 16-byte span from <see cref="Save"/> (only the first 8 are read).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="state"/> is too short.</exception>
    public void Restore(ReadOnlySpan<byte> state)
    {
        if (state.Length < 8)
            throw new ArgumentException("State buffer must be at least 8 bytes.", nameof(state));

        _state = BitConverter.ToUInt64(state[..8]);
    }

    /// <summary>
    /// Creates a new RNG <b>forked from the current state</b>,
    /// using the generator's current internal state as the seed and the original sequence id as the stream.
    /// <remarks>
    /// Use this when you want to <b>branch the timeline</b>: the fork will reproduce
    /// the same next draws as the main generator would, diverging only after.
    /// <seealso href="https://github.com/FloatObject/RNGenie/blob/master/docs/forking-streams.md">Forking Guide</seealso>
    /// </remarks>
    /// </summary>
    /// <returns>A new <see cref="Pcg32Source"/> that continues deterministically from this point.</returns>
    public Pcg32Source Fork() => new Pcg32Source(_state, _inc, direct: true);

    /// <summary>
    /// Creates a new RNG <b>forked from the current state</b>, using the generator's
    /// current internal state as the seed and the specified <paramref name="streamId"/> as the stream.
    /// <remarks>
    /// Use this when you want to <b>branch the timeline into a distinct stream</b>:
    /// the fork will evolve independently from this exact point, producing a different sequence than the main generator or other streams.
    /// <seealso href="https://github.com/FloatObject/RNGenie/blob/master/docs/forking-streams.md">Forking Guide</seealso>
    /// </remarks>
    /// <para>
    /// Note: because PCG32 outputs are derived from the <i>old state</i>, the fork's <b>first draw will always match the main generator's next draw</b>.
    /// Divergence begins on the <b>second draw</b>.
    /// </para>
    /// </summary>
    /// <param name="streamId">Stream/sequence identifier for the forked RNG.</param>
    /// <returns>A new <see cref="Pcg32Source"/> that evolves independently from this point.</returns>
    public Pcg32Source Fork(ulong streamId) => new Pcg32Source(_state, SeqToInc(streamId), direct: true);

    /// <summary>
    /// Creates a new RNG <b>derived from the original seed</b>, using the same initial seed as this generator
    /// and the provided <paramref name="streamId"/> as the stream selector.
    /// <remarks>
    /// Use this when you want <b>independent, call-order-stable streams</b> (e.g. one stream for gameplay logic, another for visual effects).
    /// Each stream produces reproducible results regardless of how other streams are used.
    /// <seealso href="https://github.com/FloatObject/RNGenie/blob/master/docs/forking-streams.md">Forking Guide</seealso>
    /// </remarks>
    /// </summary>
    /// <param name="streamId">Stream/sequence identifier for the new independent RNG stream.</param>
    /// <returns>A new <see cref="Pcg32Source"/> seeded from the original seed.</returns>
    public Pcg32Source NewStreamFromSeed(ulong streamId) => new Pcg32Source(_initialSeed, streamId);

    /// <summary>
    /// Creates a new RNG using the original seed and the original sequence/stream id,
    /// effectively recreating the original stream regardless of how this instance has advanced.
    /// </summary>
    /// <returns></returns>
    public Pcg32Source NewOriginalStream() => new Pcg32Source(_initialSeed, _initialSeq);

    /// <inheritdoc />
    public ulong StateHash => _state;
}
