using RNGenie.Core.Abstractions;

namespace RNGenie.Distributions.Continuous
{
    /// <summary>Uniform distribution on <c>[Min, Max)</c>.</summary>
    public sealed class Uniform : IDistribution<double>
    {
        /// <summary>Lower bound (inclusive).</summary>
        public double Min { get; }

        /// <summary>Upper bound (exclusive).</summary>
        public double Max { get; }

        private readonly double _range;

        /// <summary>
        /// Creates a uniform distribution on <c>[min, max)</c>.
        /// </summary>
        /// <param name="min">Lower bound (inclusive).</param>
        /// <param name="max">Upper bound (exclusive).</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="max"/> &lt;= <paramref name="min"/> or any parameter is non-finite.
        /// </exception>
        public Uniform(double min = 0, double max = 1)
        {
            if (!(max > min))
                throw new ArgumentOutOfRangeException(nameof(max), "max must be > min");
            if (double.IsNaN(min) || double.IsNaN(max) || double.IsInfinity(min) || double.IsInfinity(max))
                throw new ArgumentOutOfRangeException("Parameters must be finite.");

            Min = min; Max = max;
            _range = Max - Min;
        }

        /// <summary>
        /// Samples a value from this uniform distribution.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="rng"/> is null.
        /// </exception>
        public double Sample(IRandomSource rng)
        {
            ArgumentNullException.ThrowIfNull(rng);
            double u = rng.NextDouble(); // expect [0,1)

            if (u >= 1.0) u = Math.BitDecrement(1.0);
            if (u < 0.0) u = 0.0;

            return Min + _range * u;
        }

        /// <summary>
        /// Convenience helper to sample once without constructing an instance.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="min">Lower bound (inclusive).</param>
        /// <param name="max">Upper bound (exclusive).</param>
        public static double Sample(IRandomSource rng, double min = 0, double max = 1) =>
            new Uniform(min, max).Sample(rng);
    }
}
