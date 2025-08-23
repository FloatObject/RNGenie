namespace RNGenie.Core.RNG;

/// <summary>
/// RNG adapter that wraps the built-in <see cref="System.Random"/>
/// and implements <see cref="IRandomSource"/>.
/// <para>
/// Useful for casual scenarios where determinism and reproducibility are not required.
/// Provides quick integration with RPGenie APIs using familiar <see cref="Random"/> semantics.
/// </para>
/// <para>
/// Limitations:
/// - Not guaranteed to be reproducible across .NET versions or platforms.
/// - Lower statistical quality compared to modern RNGs like PCG32.
/// - Not suitable for cryptographics use.
/// </para>
/// </summary>
public sealed class SystemRandomSource : IRandomSource
{
    private readonly Random _r;

    /// <summary>
    /// Creates a new <see cref="SystemRandomSource"/>.
    /// <para>
    /// If <paramref name="seed"/> is <c>null</c>, uses <see cref="Random.Shared"/> (global singleton).
    /// If a seed is provided, creates a dedicated <see cref="Random"/> instance initialized with that seed.
    /// </para>
    /// </summary>
    /// <param name="seed">
    /// Optional seed for determinism.
    /// Note that sequences produced by <see cref="System.Random"/> are not stable across runtime versions or platforms.
    /// </param>
    public SystemRandomSource(int? seed = null)
        => _r = seed is null ? Random.Shared : new Random(seed.Value);

    /// <summary>
    /// Returns a uniformly distributed integer in the half-open interval <c>[minInclusive, maxExclusive)</c>.
    /// </summary>
    /// <param name="minInclusive">Inclusive lower bound.</param>
    /// <param name="maxExclusive">Exclusive upper bound.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="maxExclusive"/> is less than or equal to <paramref name="minInclusive"/>.
    /// </exception>
    /// <returns>
    /// An integer <c>x</c> such that <c>minInclusive <= x < maxExclusive</c>.
    /// </returns>
    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive), "maxExclusive must be greater than minInclusive.");

        return _r.Next(minInclusive, maxExclusive);
    }

    /// <summary>
    /// Returns a uniformly distributed double in the interval <c>[0,1)</c>.
    /// </summary>
    /// <returns>
    /// A double <c>d</c> such that <c>0 <= d < 1</c>.
    /// </returns>
    public double NextDouble() => _r.NextDouble();

    /// <summary>
    /// A hash-like view of the current internal state, useful for debugging or sanity checks.
    /// <value>
    /// Always <c>0</c>. <see cref="System.Random"/> is not deterministic across runtimes,
    /// so a stable state hash cannot be provided.
    /// </value>
    /// </summary>
    public ulong StateHash => 0;
}
