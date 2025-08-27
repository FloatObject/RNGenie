using RNGenie.Core.Abstractions;

namespace RNGenie.Core.Distributions
{
    /// <summary>
    /// Continuous uniform distribution over the interval <c>[0,1)</c>.
    /// </summary>
    public class Uniform01 : IDistribution<double>
    {
        /// <summary>
        /// Samples a value uniformly distributed in <c>[0,1)</c>.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <returns>A double <c>d</c> such that <c>0 &lt;= d &lt; 1</c>.</returns>
        public double Sample(IRandomSource rng) => rng.NextDouble(); // [0,1)
    }

    /// <summary>
    /// Triangular distribution defined by minimum, mode (peak), and maximum.
    /// <para>
    /// Useful as a lightweight, bell-shaped alternative when a full normal distribution is unnecessary.
    /// Parameters must satisfy <c>min &lt;= mode &lt;= max</c>.
    /// </para>
    /// </summary>
    public sealed class Triangular : IDistribution<double>
    {
        private readonly double _min, _mode, _max, _c;

        /// <summary>
        /// Creates a triangular distribution.
        /// </summary>
        /// <param name="min">Lower bound of the support.</param>
        /// <param name="mode">Location of the peak (must lie between <paramref name="min"/> and <paramref name="max"/>).</param>
        /// <param name="max">Upper bound of the support.</param>
        /// <exception cref="ArgumentException">Thrown when <c>min &lt;= mode &lt;= max</c> is violated.</exception>
        public Triangular(double min, double mode, double max)
        {
            if (!(min <= mode && mode <= max))
                throw new ArgumentException("min <= mode <= max violated.");

            _min = min;
            _mode = mode;
            _max = max;
            _c = (mode - min) / (max - min);
        }

        /// <summary>
        /// Samples a value from the triangular distribution.
        /// </summary>
        /// <param name="rng">Random source for sampling.</param>
        /// <returns>A double in <c>[min, max]</c> with peak density at <c>mode</c>.</returns>
        public double Sample(IRandomSource rng)
        {
            double u = rng.NextDouble();
            if (u < _c) return _min + Math.Sqrt(u * (_max - _min) * (_mode - _min));
            return _max - Math.Sqrt((1 - u) * (_max - _min) * (_max - _mode));
        }
    }

    /// <summary>
    /// Normal (Gaussian) distribution sampled via the Box-Muller transform.
    /// </summary>
    public sealed class NormalBoxMuller : IDistribution<double>
    {
        private readonly double _mean, _std;

        /// <summary>
        /// Creates a normal distribution with the specified mean and standard deviation.
        /// </summary>
        /// <param name="mean">The mean (μ).</param>
        /// <param name="std">The standard deviation (σ). Should be > 0.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="std"/> is less than or equal to 0.
        /// </exception>
        public NormalBoxMuller(double mean = 0, double std = 1)
        {
            if (std <= 0)
                throw new ArgumentOutOfRangeException(nameof(std), "Standard deviation must be greater than 0.");

            _mean = mean;
            _std = std;
        }

        /// <summary>
        /// Samples a value from <c>N(mean, std²)</c> using the Box-Muller transform.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <remarks>
        /// Uses one of the two Box-Muller outputs (<c>z0</c>). For performance-sensitive scenarios,
        /// consider caching the second variate (<c>z1</c>) for the next call.
        /// </remarks>
        /// <returns>A double distributed approximately normally with the configured mean and std.</returns>
        public double Sample(IRandomSource rng)
        {
            // avoid log(0) by shifting away from 0.0
            double u1 = 1.0 - rng.NextDouble();
            double u2 = 1.0 - rng.NextDouble();
            double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
            return _mean + z0 * _std;
        }
    }
}