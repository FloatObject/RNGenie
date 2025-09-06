namespace RNGenie.Samples
{
    internal static class Program
    {
        private static readonly Dictionary<string, (string Title, Action run)> _demos =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["dice"] = ("Dice: roll notations (e.g. 3d6+2)", DiceDemo.Run),
                ["cards"] = ("Cards: playing card functionalities", CardDemo.Run),
                ["picker"] = ("Weighted Picker: loot rarity sampler", WeightedPickerDemo.Run),
                ["dist"] = ("Distributions: Uniform, Triangular, Normal", DistributionsDemo.Run),
                ["fork"] = ("Forking: Fork() vs NewStreamFromSeed()", ForkingDemo.Run),
                ["crypto"] = ("Crypto RNG: secure ints/doubles/token", CryptoDemo.Run),
            };

        private static void Main(string[] args)
        {
            if (args.Length > 0 && _demos.TryGetValue(args[0], out var quick))
            {
                quick.run();
                return;
            }

            ConsoleMenu.Run(
                header: "RNGenie Samples",
                subtitle: "Choose a demo (arrow keys or number, Enter to run, Q to quit):",
                items: _demos);
        }
    }
}