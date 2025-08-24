using RNGenie.Core.RNG;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of Pcg32 RNG forking functionality.
    /// </summary>
    public static class ForkingDemo
    {
        /// <summary>
        /// Run the forking demo.
        /// </summary>
        public static void Run()
        {
            var rng = new Pcg32Source(100);

            Console.WriteLine("Main RNG (3 draws):");
            for (int i = 0; i < 3; i++) Console.WriteLine($"  {rng.NextInt(0, 100)}");

            var fork = rng.Fork(1);                   // Branch from *current state*
            var stream = rng.NewStreamFromSeed(2);    // Independent stream from *original seed*

            Console.WriteLine("\nForked RNG (branch timeline):");
            for (int i = 0; i < 3; i++) Console.WriteLine($"  {fork.NextInt(0, 100)}");

            Console.WriteLine("\nNewStreamFromSeed (independent stream):");
            for (int i = 0; i < 3; i++) Console.WriteLine($"  {stream.NextInt(0, 100)}");
        }
    }
}
