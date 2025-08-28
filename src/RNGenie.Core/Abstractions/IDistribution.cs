namespace RNGenie.Core.Abstractions
{
    /// <summary>
    /// Abstraction for a probability distribution that can sample values using an <see cref="IRandomSource"/>.
    /// </summary>
    /// <remarks>
    /// This interface is defined in Core. Concrete implementations (e.g. Uniform, Gaussian, Poisson)
    /// are provided in the optional <c>RNGenie.Distributions</c> package. Determinism is
    /// guaranteed when the supplied <see cref="IRandomSource"/> is deterministic.
    /// </remarks>
    /// <typeparam name="T">The type of value produced by the distribution.</typeparam>
    public interface IDistribution<T>
    {
        /// <summary>
        /// Draws a single sample from the distribution using the given random source.
        /// </summary>
        /// <param name="rng">Random source used for sampling.</param>
        /// <returns>A value distributed according to this distribution.</returns>
        T Sample(IRandomSource rng);
    }
}
