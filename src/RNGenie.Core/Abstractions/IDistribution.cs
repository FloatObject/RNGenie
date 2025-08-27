namespace RNGenie.Core.Abstractions
{
    /// <summary>
    /// Abstraction for a probability distribution that can sample values using an <see cref="IRandomSource"/>.
    /// <remarks>
    /// Core provides minimal distributions (e.g. <see cref="Distributions.Uniform01"/>, <c>Bernoulli</c>).
    /// Additional distributions (e.g. <c>RNGenie.Distributions.Gaussian</c>, <c>Poisson</c>)
    /// are available in the optional <c>RNGenie.Distributions</c> package.
    /// </remarks>
    /// </summary>
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
