using System.Collections.Immutable;

namespace AggregateStatistics;

public class ProbDist {
    private SortedDictionary<Fraction, double> _pmf;
    private double[] _cdf;
    
    private readonly Random _random = new ();
    private const double Tolerance = 0.00000001;
    
    private ProbDist(SortedDictionary<Fraction, double> pmf) {
        _pmf = pmf;
        _cdf = new double[_pmf.Count];
        RecalculateCDF();
    }
    
    private ProbDist(IEnumerable<(Fraction, double)> pmf) {
        _pmf = new SortedDictionary<Fraction, double>();
        foreach (var data in pmf) {
            _pmf[data.Item1] = data.Item2;
        }
        _cdf = new double[_pmf.Count];
        RecalculateCDF();
    }

    public ProbDist(Fraction min, Fraction max, double[] pmf) {
        var normalized = pmf.Normalize();
        _pmf = new SortedDictionary<Fraction, double>();
        _cdf = new double[pmf.Length];
        for (var i = 0; i < pmf.Length; i++) {
            var value = Utils.Lerp(min, max, new Fraction(i, pmf.Length - 1));
            _pmf[value] = normalized[i];
        }
        RecalculateCDF();
    }

    private void RecalculateCDF() {
        _cdf = new double[_pmf.Count];
        var c = 0.0;
        var i = 0;
        foreach (var p in _pmf.Values) {
            c += p;
            _cdf[i++] = c;
        }
    }

    public Fraction Min() => _pmf.Keys.Min();
    public Fraction Max() => _pmf.Keys.Max();
    
    /**
     * Finds the quantile by doing a binary search on the CDF.
     * 
     * First quartile -> y=0.25
     * Median -> y=0.5
     * Second quartile -> y=0.75
     */
    public Fraction Quantile(double y) {
        var index = _cdf.Length / 2;
        while (true) {
            if (index >= _cdf.Length) return Max();
            if (index <= 0) return Min();
            if (_cdf[index - 1] < y && y <= _cdf[index]) {
                return _pmf.Keys.ToArray()[index];
            }
            
            if (y > _cdf[index]) {
                index += index / 2;
            } else {
                index /= 2;
            }
        }
    }
    
    /**
     * Combines probability buckets to save memory
     */
    public void DownSample(int scaleFactor) {
        // Down sampled pmf has extra slot for remainder, if there is one
        var newData = new SortedDictionary<Fraction, double>();
        var length = _pmf.Count % scaleFactor == 0 ? _pmf.Count / scaleFactor : _pmf.Count / scaleFactor + 1;
        var keys = _pmf.Keys.ToArray();
        var values = _pmf.Values.ToArray();
        Parallel.For(0, length, i => {
            var sum = 0.0;
            for (var j = 0; j < scaleFactor; j++) {
                if (i * scaleFactor + j >= _pmf.Count) break;
                sum += values[i * scaleFactor + j];
                lock (newData) {
                    newData[keys[i * scaleFactor]] = sum;
                }
            }
        });
        _pmf = newData;
        RecalculateCDF();
    }
    
    /**
     * Finds sample by passing a uniform random number between 0 and 1 to the quantile function
     */
    public Fraction Sample() => Quantile(_random.NextDouble());
    
    public static bool operator ==(ProbDist a, ProbDist b) => a._pmf.Count == b._pmf.Count && 
                                                                      a._pmf.Keys.SequenceEqual(b._pmf.Keys, new Fraction.Equality()) &&
                                                                      a._pmf.Values.SequenceEqual(b._pmf.Values, new DoubleEquality()) &&
                                                                      a._cdf.SequenceEqual(b._cdf, new DoubleEquality());
    public static bool operator !=(ProbDist a, ProbDist b) => !(a==b);
    public static ProbDist operator +(ProbDist d) => d;
    public static ProbDist operator -(ProbDist d) => new (d._pmf.Select(p => (-p.Key, p.Value)));
    public static ProbDist operator +(ProbDist a, ProbDist b) => Operate(a, b, (d, d1) => d+d1);
    public static ProbDist operator -(ProbDist a, ProbDist b) => a + -b;
    public static ProbDist operator +(ProbDist d, Fraction s) => new (d._pmf.Select(p => (p.Key+s, p.Value)));
    public static ProbDist operator -(ProbDist d, Fraction s) => d + -s;
    public static ProbDist operator *(ProbDist d, Fraction s) => new (d._pmf.Select(p => (p.Key*s, p.Value)));
    public static ProbDist operator /(ProbDist d, Fraction s) => d * (1/s);

    public static ProbDist Operate(ProbDist a, ProbDist b, Func<Fraction, Fraction, Fraction> f) {
        var pmf = new SortedDictionary<Fraction, double>();
        foreach (var entryA in a._pmf) {
            foreach (var entryB in b._pmf) {
                var key = f(entryA.Key, entryB.Key);
                if (pmf.ContainsKey(key)) {
                    pmf[f(entryA.Key, entryB.Key)] += entryA.Value * entryB.Value;
                } else {
                    pmf[f(entryA.Key, entryB.Key)] = entryA.Value * entryB.Value;
                }
            }
        }
        return new ProbDist(pmf);
    }
    
    public double Mean() {
        var sum = 0.0;
        foreach (var pair in _pmf) {
            sum += (double) pair.Key * pair.Value;
        }
        return sum;
    }
    
    private class DoubleEquality : IEqualityComparer<double> {
        public bool Equals(double a, double b) {
            return a.Equals(b, Tolerance);
        }

        public int GetHashCode(double d) {
            return d.GetHashCode();
        }
    }

    public void printPMF() {
        Console.WriteLine($"[{String.Join(", ", _pmf.Select(p => $"{p.Key}: {p.Value}"))}]");
    }
}