using RNGenie.RNG;
using RNGenie.Picks;
using RNGenie.Dice;
using RNGenie.Dist;

var rng = new Pcg32Source(seed: 123);

var rarity = new WeightedPicker<string>()
    .Add("Common", 0.75).Add("Rare", 0.20).Add("Epic", 0.05)
    .One(rng);

var (total, rolls, mod) = Dice.Roll("3d6+2", rng);

var damage = new Triangular(5, 12, 20).Sample(rng);

Console.WriteLine($"Rarity: {rarity}");
Console.WriteLine($"Dice: {total} (rolls: {string.Join(",", rolls)} {mod:+#;-#;0})");
Console.WriteLine($"Damage: {damage:F2}");
