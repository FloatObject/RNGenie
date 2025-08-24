using System.Text.RegularExpressions;

using RNGenie.Core.RNG;

namespace RNGenie.Core.Dice
{
    /// <summary>
    /// Utility for parsing and rolling common RPG-style dice notations such as <c>3d6+2</c>.
    /// <para>
    /// Syntax: <c>{count}d{sides}[+/-modifier]</c>
    /// </para>
    /// <list type="bullet">
    ///     <item><description><c>count</c> = number of dice (must be >= 1)</description></item>
    ///     <item><description><c>sides</c> = number of sides per die (must be >= 2)</description></item>
    ///     <item><description><c>modifier</c> = optional constant added/subtracted to the result</description></item>
    /// </list>
    /// <para>
    /// Examples: <c>1d20</c>, <c>3d6+2</c>, <c>2d10-1</c>.
    /// </para>
    /// Determinism: reproducible if you supply a deterministic RNG (e.g. <see cref="Pcg32Source"/>).
    /// </summary>
    public static class Dice
    {
        private static readonly Regex Rx = new(@"^\s*(\d+)d(\d+)([+-]\d+)?\s*$",
                                               RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Parses a dice notation string and performs the roll using the given RNG.
        /// </summary>
        /// <param name="notation">Dice notation string to parse (e.g. <c>"3d6+2"</c> or <c>"1d20"</c>).</param>
        /// <param name="rng">Random source used to perform the rolls.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the notation string is invalid.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the number of dice &lt;= 0 or the number of sides &lt; 2.
        /// </exception>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///     <item><description><c>total</c> = sum of all rolls plus modifier</description></item>
        ///     <item><description><c>rolls</c> = individual die rolls</description></item>
        ///     <item><description><c>modifier</c> = constant added or subtracted from the total</description></item>
        /// </list>
        /// </returns>
        public static (int total, int[] rolls, int modifier) Roll(string notation, IRandomSource rng)
        {
            var m = Rx.Match(notation);
            if (!m.Success)
                throw new ArgumentException("Bad dice notation.", nameof(notation));

            int count = int.Parse(m.Groups[1].Value);
            int sides = int.Parse(m.Groups[2].Value);
            int modifier = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0;

            if (count <= 0 || sides < 2)
                throw new ArgumentOutOfRangeException(nameof(notation), "Invalid dice count or sides.");

            var rolls = new int[count];
            int sum = 0;

            for (int i = 0; i < count; i++)
            {
                int roll = rng.NextInt(1, sides + 1);
                rolls[i] = roll;
                sum += roll;
            }

            sum += modifier;
            return (sum, rolls, modifier);
        }
    }
}
