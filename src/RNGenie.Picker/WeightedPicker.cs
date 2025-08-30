using RNGenie.Core.Abstractions;
using RNGenie.Core.Sources;

namespace RNGenie.Picker;

/// <summary>
/// Simple weighted picker.
/// <para>
/// Add items with positive weights, then call <see cref="One(IRandomSource)"/>
/// to draw a single item according to their relative weights.
/// Weights do not need to be normalized; the picker uses the running total internally.
/// </para>
/// <para>
/// Determinism: results are reproducible if you supply a deterministic RNG (e.g. <see cref="Pcg32Source"/>).
/// With <see cref="SystemRandomSource"/> or <see cref="CryptoRandomSource"/> the sequence is not reproducible across runs.
/// </para>
/// </summary>
/// <typeparam name="T">The item type being selected.</typeparam>
public sealed class WeightedPicker<T>
{
    private readonly List<(T item, double weight)> _items = new();
    private double _total;

    /// <summary>
    /// The total number of items.
    /// </summary>
    public int Count => _items.Count;
    /// <summary>
    /// The total weight of all items.
    /// </summary>
    public double TotalWeight => _total;

    /// <summary>
    /// Adds an item with the specified positive <paramref name="weight"/>.
    /// </summary>
    /// <param name="item">The item to include in the picker.</param>
    /// <param name="weight">The relative weight for the item. Higher weights increase the chance of the item being chosen.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="weight"/> is less than or equal to 0.
    /// </exception>
    /// <returns>The picker instance (for fluent chaining).</returns>
    public WeightedPicker<T> Add(T item, double weight)
    {
        if (!double.IsFinite(weight) || weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be a finite number > 0.");

        _items.Add((item, weight));
        _total += weight;
        return this;
    }

    /// <summary>
    /// Removes all items.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        _total = 0;
    }

    /// <summary>
    /// Draws a single item according to the relative weights of all items added so far.
    /// </summary>
    /// <param name="rng">Random source used for sampling.</param>
    /// <remarks>
    /// If floating-point rounding prevents a match, the last item is returned.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when no items have been added.</exception>
    /// <returns>
    /// One item sampled from the configured set, 
    /// where the probability of each item is its weight divided by the sum of all weights.
    /// </returns>
    public T One(IRandomSource rng)
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("No items added.");

        double r = rng.NextDouble() * _total;
        double acc = 0.0;

        foreach (var (item, w) in _items)
        {
            acc += w;
            if (r < acc) return item;
        }

        return _items[^1].item; // precision edge
    }
}
