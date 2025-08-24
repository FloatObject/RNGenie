namespace RNGenie.Core.RNG;

/// <summary>
/// Abstraction for a random number generator used by RNGenie components.
/// <para>
/// This interface allows different RNG implementations (e.g. <see cref="Pcg32Source"/>, <see cref="SystemRandomSource"/>, <see cref="CryptoRandomSource"/>)
/// to be plugged in interchangeably. Consumers may also implement this interface to integrate custom RNGs with RNGenie.
/// </para>
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Returns a uniformly distributed integer in the half-open interval <c>[minInclusive, maxExclusive)</c>.
    /// </summary>
    /// <param name="minInclusive">Inclusive lower bound.</param>
    /// <param name="maxExclusive">Exclusive upper bound.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="maxExclusive"/> is less than or equal to <paramref name="minInclusive"/>.
    /// </exception>
    /// <returns>
    /// An integer <c>x</c> such that <c>minInclusive &lt;= x &lt; maxExclusive</c>.
    /// </returns>
    int NextInt(int minInclusive, int maxExclusive);

    /// <summary>
    /// Returns a uniformly distributed double in the interval <c>[0,1)</c>.
    /// </summary>
    /// <returns>
    /// A double <c>d</c> such that <c>0 &lt;= d &lt; 1</c>.
    /// </returns>
    double NextDouble();

    /// <summary>
    /// Fills the provided buffer with random bytes.
    /// </summary>
    /// <param name="buffer">Destination span to fill with random data.</param>
    void NextBytes(Span<byte> buffer);

    /// <summary>
    /// Gets a hash-like view of the current internal state of the RNG,
    /// useful for debugging or sanity checks in deterministic scenarios.
    /// <para>
    /// Implementations that cannot provide a stable state hash (e.g. <see cref="SystemRandomSource"/>) may return <c>0</c> or another placeholder.
    /// </para>
    /// </summary>
    /// <value>
    /// A <see cref="ulong"/> representing the RNG's current state or a placeholder if not supported.
    /// </value>
    ulong StateHash { get; }
}
