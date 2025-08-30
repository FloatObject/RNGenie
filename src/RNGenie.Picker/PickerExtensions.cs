using RNGenie.Core.Abstractions;

namespace RNGenie.Picker
{
    /// <summary>
    /// Fluent helper extensions for <see cref="IRandomSource"/> to perform
    /// uniform and weighted selections. These forward to the core <see cref="WeightedPicker{T}"/>
    /// implementation or perform one-off picks for convenience.
    /// </summary>
    public static class PickerExtensions
    {
        /// <summary>
        /// Uniformly selects a single element from <paramref name="items"/> using this RNG.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="items">Collection to sample from (must not be empty).</param>
        /// <returns>One uniformly sampled element from <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> is empty.</exception>
        public static T PickOne<T>(this IRandomSource rng, IReadOnlyList<T> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (items.Count == 0)
                throw new ArgumentException("Sequence is empty.", nameof(items));

            int i = rng.NextInt(0, items.Count);
            return items[i];
        }

        /// <summary>
        /// Uniformly selects a single element from <paramref name="items"/> using this RNG.
        /// Convenience overload for arrays.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="items">Array to sample from (must not be empty).</param>
        /// <returns>One uniformly sampled element from <paramref name="items"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="items"/> is empty.</exception>
        public static T PickOne<T>(this IRandomSource rng, T[] items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));
            if (items.Length == 0)
                throw new ArgumentException("Sequence is empty.", nameof(items));

            int i = rng.NextInt(0, items.Length);
            return items[i];
        }

        /// <summary>
        /// Performs a weighted selection from an existing <see cref="WeightedPicker{T}"/>.
        /// This is a fluent wrapper around <see cref="WeightedPicker{T}.One(IRandomSource)"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="picker">Configured weighted picker (must contain at least one item).</param>
        /// <returns>One element sampled according to the configured weights.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="picker"/> is <c>null</c>.</exception>
        public static T PickWeighted<T>(this IRandomSource rng, WeightedPicker<T> picker)
        {
            if (picker is null)
                throw new ArgumentNullException(nameof(picker));
            return picker.One(rng);
        }

        /// <summary>
        /// Performs a one-off weighted selection from a list of (item, weight) pairs.
        /// Positive, finite weights contribute to the selection. Non-positive or NaN/Infinity weights are ignored.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="rng">Random source used for sampling.</param>
        /// <param name="items">List of (item, weight) pairs.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown when all provided weights are non-positive or non-finite (NaN/Infinity).
        /// </exception>
        /// <returns>One element sampled in proportion to its weight.</returns>
        public static T PickWeighted<T>(this IRandomSource rng, IReadOnlyList<(T item, double weight)> items)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));

            // Compute total of valid weights (finite and > 0).
            double total = 0.0;
            foreach (var (_, w) in items)
            {
                if (double.IsFinite(w) && w > 0) total += w;
            }
            if (total <= 0.0)
                throw new ArgumentException("All weights are non-positive or invalid.", nameof(items));

            // Draw r in [0, total) and walk cumulative weights.
            double r = rng.NextDouble() * total;
            double acc = 0.0;
            foreach (var (item, w) in items)
            {
                if (!double.IsFinite(w) || w <= 0) continue;
                acc += w;
                if (r < acc) return item;
            }

            // Precision fallback: return the last valid-weight item.
            for (int i = items.Count - 1; i >= 0; i--)
            {
                var (item, w) = items[i];
                if (double.IsFinite(w) && w > 0) return item;
            }

            // Should be unreachable given the earlier total>0 check.
            throw new InvalidOperationException("No selectable item found.");
        }
    }
}
