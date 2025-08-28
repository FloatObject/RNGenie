using RNGenie.Core.Abstractions;

namespace RNGenie.Distributions.Continuous
{
    /// <summary>
    /// Triangular distribution on <c>[Min, Max)</c> with peak (mode) at <see cref="Mode"/>.
    /// </summary>
    /// <remarks>
    /// Implemented via inverse CDF. Sampling is deterministic when driven by a deterministic <see cref="IRandomSource"/>.
    /// This type is immutable and thread-safe.
    /// </remarks>
    public sealed class Triangular : IDistribution<double>
    {
        /// <summary>Lower bound of the support (often noted <c>a</c>).</summary>
        public double Min { get; }

        /// <summary>
        /// Mode (location of the peak, often noted <c>c</c>).
        /// </summary>
        public double Mode { get; }

        /// <summary>Upper bound of the support (often noted <c>b</c>).</summary>
        public double Max { get; }

        // precomputed constants
        private readonly double _range;   // (max - min)
        private readonly double _left;    // (mode - min)
        private readonly double _right;   // (max - mode)
        private readonly double _fc;      // split point

        /// <summary>
        /// Creates a triangular distribution on <c>[min, max)</c> with mode at <paramref name="mode"/>.
        /// </summary>
        /// <param name="min">Lower bound of the support.</param>
        /// <param name="mode">Mode (peak) — must lie within <c>[min, max]</c>.</param>
        /// <param name="max">Upper bound of the support. Must be greater than <paramref name="min"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="max"/> is not greater than <paramref name="min"/>,
        /// when <paramref name="mode"/> lies outside <c>[min, max]</c>,
        /// or when any parameter is not finite.
        /// </exception>
        public Triangular(double min, double mode, double max)
        {
            if (!(max > min))
                throw new ArgumentOutOfRangeException(nameof(max), "max must be > min");
            if (mode < min || mode > max)
                throw new ArgumentOutOfRangeException(nameof(mode), "mode must be in [min, max]");
            if (double.IsNaN(min) || double.IsNaN(mode) || double.IsNaN(max) ||
                double.IsInfinity(min) || double.IsInfinity(mode) || double.IsInfinity(max))
                throw new ArgumentOutOfRangeException("Parameters must be finite.");

            Min = min; Mode = mode; Max = max;

            _range = Max - Min;
            _left = Mode - Min;
            _right = Max - Mode;
            _fc = Math.Clamp(_left / _range, 0.0, 1.0);
        }

        /// <summary>
        /// Samples a value from this triangular distribution.
        /// </summary>
        /// <remarks>Implements inverse transform sampling.</remarks>
        /// <param name="rng">Random source used for sampling.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="rng"/> is null.
        /// </exception>
        /// <returns>
        /// A <see cref="double"/> in <c>[Min, Max)</c> with peak density at <see cref="Mode"/>.
        /// </returns>
        public double Sample(IRandomSource rng)
        {
            ArgumentNullException.ThrowIfNull(rng);

            double u = rng.NextDouble();   // U ~ [0,1)

            if (u <= _fc)  // left branch (up to mode)
            {
                double rad = u * _range * _left;
                return Min + Math.Sqrt(Math.Max(0.0, rad));
            }

            double rad2 = (1.0 - u) * _range * _right; // right branch
            return Max - Math.Sqrt(Math.Max(0.0, rad2));
        }

        /// <summary>
        /// Convenience helper to sample once without explicitly constructing an instance.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="min">Lower bound.</param>
        /// <param name="mode">Mode (peak) — must lie within <c>[min, max]</c>.</param>
        /// <param name="max">Upper bound. Must be greater than <paramref name="min"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// See <see cref="Triangular(double, double, double)"/>.
        /// </exception>
        /// <returns>
        /// A <see cref="double"/> in <c>[min, max)</c> with peak density at <paramref name="mode"/>.
        /// </returns>
        public static double Sample(IRandomSource rng, double min, double mode, double max) =>
            new Triangular(min, mode, max).Sample(rng);
    }
}
