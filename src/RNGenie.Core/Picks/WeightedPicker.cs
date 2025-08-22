namespace RNGenie.Picks;
using RNGenie.RNG;

public sealed class WeightedPicker<T>
{
    private readonly List<(T item, double weight)> _items = new();
    private double _total;

    public WeightedPicker<T> Add(T item, double weight)
    {
        if (weight <= 0) throw new ArgumentOutOfRangeException(nameof(weight));
        _items.Add((item, weight));
        _total += weight;
        return this;
    }

    public T One(IRandomSource rng)
    {
        if (_items.Count == 0) throw new InvalidOperationException("No items added.");
        double r = rng.NextDouble() * _total;
        double acc = 0;
        foreach (var (item, w) in _items) { acc += w; if (r < acc) return item; }
        return _items[^1].item; // edge case
    }
}
