using RNGenie.Core.Abstractions;

namespace RNGenie.Distributions.Internal.Algorithms
{
    /// <summary>
    /// Internal Box–Muller generator for standard normal variates (N(0,1)).
    /// Produces two independent samples per iteration, caches one in <see cref="StandardNormalState"/>.
    /// </summary>
    internal static class BoxMuller
    {
        /// <summary>
        /// Returns a single N(0,1) sample. If a cached sample exists in <paramref name="state"/>,
        /// it is returned and the cache is cleared, otherwise a new pair is generated and one is cached.
        /// </summary>
        internal static double Next(IRandomSource rng, ref StandardNormalState state)
        {
            if (state.HasSpare)
            {
                state.HasSpare = false;
                return state.Spare;
            }

            // Box–Muller (polar variant with trig for clarity and determinism across platforms).
            // Ensure u1 ∈ (0,1] to avoid log(0). NextDouble() is assumed ∈ [0,1).
            double u1;
            do
            {
                u1 = rng.NextDouble();
            } while (u1 <= double.Epsilon);

            double u2 = rng.NextDouble(); // [0,1)
            const double TwoPi = 2.0 * Math.PI;

            double r = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = TwoPi * u2;

            double z0 = r * Math.Cos(theta);
            double z1 = r * Math.Sin(theta);

            state.Spare = z1;
            state.HasSpare = true;

            return z0;
        }
    }
}
