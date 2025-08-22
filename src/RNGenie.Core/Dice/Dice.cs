using System.Text.RegularExpressions;
using RNGenie.Core.RNG;

namespace RNGenie.Core.Dice;

public static class Dice
{
    private static readonly Regex Rx = new(@"^\s*(\d+)d(\d+)([+-]\d+)?\s*$",
                                           RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static (int total, int[] rolls, int modifier) Roll(string notation, IRandomSource rng)
    {
        var m = Rx.Match(notation);
        if (!m.Success) throw new ArgumentException("Bad dice notation.", nameof(notation));

        int count = int.Parse(m.Groups[1].Value);
        int sides = int.Parse(m.Groups[2].Value);
        int modifier = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0;

        if (count <= 0 || sides < 2) throw new ArgumentOutOfRangeException(nameof(notation));

        var rolls = new int[count];
        int sum = 0;
        for (int i = 0; i < count; i++) { int roll = rng.NextInt(1, sides + 1); rolls[i] = roll; sum += roll; }
        sum += modifier;
        return (sum, rolls, modifier);
    }
}
