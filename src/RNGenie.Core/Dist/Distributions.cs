using RNGenie.RNG;

namespace RNGenie.Dist;

public interface IDistribution<T> { T Sample(IRandomSource rng); }

public sealed class Uniform01 : IDistribution<double>
{
    public double Sample(IRandomSource rng) => rng.NextDouble(); // [0,1)
}

public sealed class Triangular : IDistribution<double>
{
    private readonly double _min, _mode, _max, _c;
    public Triangular(double min, double mode, double max)
    {
        if (!(min <= mode && mode <= max)) throw new ArgumentException("min <= mode <= max violated.");
        _min = min; _mode = mode; _max = max;
        _c = (mode - min) / (max - min);
    }
    public double Sample(IRandomSource rng)
    {
        double u = rng.NextDouble();
        if (u < _c) return _min + Math.Sqrt(u * (_max - _min) * (_mode - _min));
        return _max - Math.Sqrt((1 - u) * (_max - _min) * (_max - _mode));
    }
}

public sealed class NormalBoxMuller : IDistribution<double>
{
    private readonly double _mean, _std;
    public NormalBoxMuller(double mean = 0, double std = 1) { _mean = mean; _std = std; }
    public double Sample(IRandomSource rng)
    {
        // avoid log(0)
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double z0 = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        return _mean + z0 * _std;
    }
}
