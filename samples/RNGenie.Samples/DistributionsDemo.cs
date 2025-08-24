using RNGenie.Core.Dist;
using RNGenie.Core.RNG;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of distribution functionality.
    /// </summary>
    public static class DistributionsDemo
    {
        /// <summary>
        /// Run the distribution demo.
        /// </summary>
        public static void Run()
        {
            var rng = new Pcg32Source(42);

            var u = new Uniform01();
            var t = new Triangular(0, 5, 10);
            var n = new NormalBoxMuller(0, 1);

            Console.WriteLine("Uniform01 samples:");
            for (int i = 0; i < 5; i++) Console.WriteLine($"  {u.Sample(rng):F6}");

            Console.WriteLine("\nTriangular(0,5,10) samples:");
            for (int i = 0; i < 5; i++) Console.WriteLine($"  {t.Sample(rng):F6}");

            Console.WriteLine("\nNormal(0,1) samples:");
            for (int i = 0; i < 5; i++) Console.WriteLine($"  {n.Sample(rng):F6}");
        }
    }
}
