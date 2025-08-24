using RNGenie.Core.RNG;

namespace RNGenie.Samples
{
    /// <summary>
    /// Demonstration of cryptographically secure RNG functionality.
    /// </summary>
    public static class CryptoDemo
    {
        /// <summary>
        /// Run the crypto demo.
        /// </summary>
        public static void Run()
        {
            using var rng = new CryptoRandomSource();

            Console.WriteLine("Crypto ints [0,100):");
            for (int i = 0; i < 5; i++) Console.WriteLine($"  {rng.NextInt(0, 100)}");

            Console.WriteLine("\nCrypto doubles [0,1):");
            for (int i = 0; i < 5; i++) Console.WriteLine($"  {rng.NextDouble():F10}");

            Console.WriteLine("\nSecure token (16 bytes hex):");
            var buf = new byte[16];
            rng.NextBytes(buf);
            Console.WriteLine($"  {Convert.ToHexString(buf)}");
        }
    }
}
