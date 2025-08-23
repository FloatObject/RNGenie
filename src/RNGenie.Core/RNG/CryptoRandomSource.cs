using System;
using System.Security.Cryptography;

namespace RNGenie.Core.RNG
{
    /// <summary>
    /// RNG adapter that wraps <see cref="RandomNumberGenerator"/>
    /// from <see cref="System.Security.Cryptography"/> and implements <see cref="IRandomSource"/>.
    /// <para>
    /// Provides cryptographically strong random values suitable for security-sensitive scenarios
    /// (e.g. tokens, shuffles, lotteries). Unlike <see cref="SystemRandomSource"/> or <see cref="Pcg32Source"/>,
    /// it is not designed for reproducibility or deterministic replays.
    /// </para>
    /// </summary>
    public sealed class CryptoRandomSource : IRandomSource, IDisposable
    {
        private static readonly double InvTwo64 = 1.0 / 18446744073709551616.0; // 1 / 2^64
        private readonly RandomNumberGenerator _rng;

        /// <summary>
        /// Creates a new <see cref="CryptoRandomSource"/> that generates cryptographically strong random values.
        /// </summary>
        public CryptoRandomSource() => _rng = RandomNumberGenerator.Create();

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
        public int NextInt(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive), "maxExclusive must be greater than minInclusive.");

            // System.Security.Cryptography.RandomNumberGenerator has NextInt in .NET 6+
            return RandomNumberGenerator.GetInt32(minInclusive, maxExclusive);
        }

        /// <summary>
        /// Returns a uniformly distributed double in the interval <c>[0,1)</c>.
        /// </summary>
        /// <returns>
        /// A double <c>d</c> such that <c>0 &lt;= d &lt; 1</c>.
        /// </returns>
        public double NextDouble()
        {
            Span<byte> buffer = stackalloc byte[8];
            _rng.GetBytes(buffer);
            ulong value = BitConverter.ToUInt64(buffer);
            // Map to [0,1): multiply by 1/2^64 (so the max maps to just under 1.0).
            return value * InvTwo64;
        }

        /// <summary>
        /// Gets a hash-like view of the current internal state.
        /// </summary>
        /// <value>
        /// Always <c>0</c>. Cryptographic RNGs do not expose stable internal state.
        /// </value>
        public ulong StateHash => 0;

        /// <summary>
        /// Releases the underlying <see cref="RandomNumberGenerator"/>.
        /// </summary>
        public void Dispose() => _rng.Dispose();
    }
}
