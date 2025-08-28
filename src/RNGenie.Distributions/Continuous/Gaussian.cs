using RNGenie.Core.Abstractions;
using RNGenie.Distributions.Internal.Algorithms;

// Change this line to swap the current algorithm.
using NormalAlgo = RNGenie.Distributions.Internal.Algorithms.BoxMuller;

namespace RNGenie.Distributions.Continuous
{
    /// <summary>
    /// Normal (Gaussian) distribution <c>N(mean, stdDev²)</c>.
    /// </summary>
    /// <remarks>
    /// This implementation is deterministic when driven by a deterministic <see cref="IRandomSource"/>.
    /// Internally it uses a normal variate algorithm (e.g. Box–Muller) but the specific method is
    /// considered an implementation detail and may change in future versions without breaking API.
    /// If you need bit-for-bit reproducibility across RNGenie versions and runtimes, pin your
    /// package version in your application.
    /// </remarks>
    public sealed class Gaussian : IDistribution<double>
    {
        private readonly double _mean;
        private readonly double _stdDev;

        // Per-instance cache for the second variate (kept here to avoid cross-call shared state).
        private StandardNormalState _state;

        /// <summary>Gets the mean μ.</summary>
        public double Mean => _mean;

        /// <summary>Gets the standard deviation σ (&gt; 0).</summary>
        public double StdDev => _stdDev;

        /// <summary>
        /// Creates a normal distribution with the specified mean and standard deviation.
        /// </summary>
        /// <param name="mean">The mean (μ). Must be finite.</param>
        /// <param name="stdDev">The standard deviation (σ). Must be &gt; 0 and finite.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="mean"/> is not finite,
        /// or when <paramref name="stdDev"/> &lt;= 0 or not finite.
        /// </exception>
        public Gaussian(double mean = 0, double stdDev = 1)
        {
            if (!(stdDev > 0) || double.IsNaN(stdDev) || double.IsInfinity(stdDev))
                throw new ArgumentOutOfRangeException(nameof(stdDev), "Standard deviation must be a finite value > 0.");
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentOutOfRangeException(nameof(mean), "Mean must be finite.");

            _mean = mean;
            _stdDev = stdDev;
        }

        /// <summary>
        /// Samples a value from <c>N(mean, stdDev²)</c>.
        /// </summary>
        /// <remarks>
        /// This type is not thread-safe due to per-instance caching of an extra variate. Use a separate
        /// instance per thread if sampling concurrently.
        /// </remarks>
        /// <param name="rng">Random source used for sampling.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="rng"/> is null.
        /// </exception>
        /// <returns>A double distributed approximately normally with the configured mean and stdDev.</returns>
        public double Sample(IRandomSource rng)
        {
            ArgumentNullException.ThrowIfNull(rng);

            double z = NormalAlgo.Next(rng, ref _state);
            return _mean + _stdDev * z;
        }

        /// <summary>
        /// Convenience method to sample from a normal distribution without explicitly constructing an instance.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="mean">Mean (μ), must be finite.</param>
        /// <param name="stdDev">Standard deviation (σ), must be &gt; 0 and finite.</param>
        public static double Sample(IRandomSource rng, double mean = 0, double stdDev = 1) =>
            new Gaussian(mean, stdDev).Sample(rng);
    }
}
