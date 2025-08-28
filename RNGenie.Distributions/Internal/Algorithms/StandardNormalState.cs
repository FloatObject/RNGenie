
namespace RNGenie.Distributions.Internal.Algorithms
{
    /// <summary>
    /// State for standard-normal samplers (pair caching, etc.).
    /// </summary>
    internal struct StandardNormalState
    {
        internal bool HasSpare;
        internal double Spare;
    }
}
