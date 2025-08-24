using RNGenie.Core.Picks;
using RNGenie.Core.RNG;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of weighted picker functionality.
    /// </summary>
    public static class WeightedPickerDemo
    {
        /// <summary>
        /// Run the weighted picker demo.
        /// </summary>
        public static void Run()
        {
            var rng = new Pcg32Source(123);

            var picker = new WeightedPicker<string>()
                .Add("Common", 0.75)
                .Add("Rare", 0.20)
                .Add("Epic", 0.05);

            Console.WriteLine("Loot drops:");

            for (int i = 0; i < 10; i++)
                Console.WriteLine($" {i + 1,2}: {picker.One(rng)}");
        }
    }
}
