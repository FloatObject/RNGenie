using RNGenie.Core.Dice;
using RNGenie.Core.Sources;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of dice RNG functionality.
    /// </summary>
    public static class DiceDemo
    {
        /// <summary>
        /// Run the dice demo.
        /// </summary>
        public static void Run()
        {
            var rng = new Pcg32Source(123);

            var (t1, rolls1, mod1) = Dice.Roll("3d6+2", rng);
            Console.WriteLine($"Rolled 3d6+2 => total={t1}, rolls=[{string.Join(",", rolls1)}, mod={mod1}]");

            var (t2, rolls2, _) = Dice.Roll("1d20", rng);
            Console.WriteLine($"Rolled 1d20 => total={t2}, roll={rolls2[0]}");
        }
    }
}
