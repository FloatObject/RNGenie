using RNGenie.Core.Abstractions;

namespace RNGenie.Dice
{
    /// <summary>
    /// Fluent helpers on IRandomSource for rolling dice notations.
    /// </summary>
    public static class DiceExtensions
    {
        /// <summary>
        /// Rolls the given dice notation with this RNG.
        /// Example: rng.Roll("3d6+2")
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="notation"></param>
        /// <returns></returns>
        public static (int total, int[] rolls, int modifier) Roll(this IRandomSource rng, string notation) => DiceRoller.Roll(notation, rng);

    }
}
