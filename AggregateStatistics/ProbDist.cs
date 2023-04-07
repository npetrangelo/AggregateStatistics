using System.Collections.Immutable;

namespace AggregateStatistics;

public class ProbDist {
    private SortedDictionary<Fraction, Fraction> _pmf;
    private Fraction[] _cdf;
    
    private readonly Random _random = new ();
    
    private ProbDist(SortedDictionary<Fraction, Fraction> pmf) {
        _pmf = pmf;
        _cdf = new Fraction[_pmf.Count];
        RecalculateCDF();
    }
    
    private ProbDist(IEnumerable<(Fraction, Fraction)> pmf) {
        _pmf = new SortedDictionary<Fraction, Fraction>();
        foreach (var data in pmf) {
            if (_pmf.ContainsKey(data.Item1)) {
                _pmf[data.Item1] += data.Item2;
            } else {
                _pmf[data.Item1] = data.Item2;
            }
        }
        _cdf = new Fraction[_pmf.Count];
        RecalculateCDF();
    }
    
    public ProbDist(IEnumerable<Fraction> possibilities, IEnumerable<Fraction> probabilities) {
        var normalized = probabilities.ToArray().Normalize();
        _pmf = new SortedDictionary<Fraction, Fraction>();
        foreach (var data in possibilities.Zip(normalized)) {
            _pmf[data.Item1] = data.Item2;
        }
        _cdf = new Fraction[_pmf.Count];
        RecalculateCDF();
    }

    public ProbDist(Fraction min, Fraction max, Fraction[] pmf) {
        var normalized = pmf.Normalize();
        _pmf = new SortedDictionary<Fraction, Fraction>();
        _cdf = new Fraction[pmf.Length];
        for (var i = 0; i < pmf.Length; i++) {
            var value = Utils.Lerp(min, max, new Fraction(i, pmf.Length - 1));
            _pmf[value] = normalized[i];
        }
        RecalculateCDF();
    }

    private void RecalculateCDF() {
        _cdf = new Fraction[_pmf.Count];
        Fraction c = 0;
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
     * Finds sample by passing a uniform random number between 0 and 1 to the quantile function
     */
    public Fraction Sample() => Quantile(_random.NextDouble());
    
    /**
     * Combines probability buckets to save memory
     */
    public void DownSample(int scaleFactor) {
        // Down sampled pmf has extra slot for remainder, if there is one
        var newData = new SortedDictionary<Fraction, Fraction>();
        var length = _pmf.Count % scaleFactor == 0 ? _pmf.Count / scaleFactor : _pmf.Count / scaleFactor + 1;
        var keys = _pmf.Keys.ToArray();
        var values = _pmf.Values.ToArray();
        Parallel.For(0, length, i => {
            Fraction sum = 0;
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
     * Calculates the expected value of probability distribution d.
     * Written this way to resemble the mathematical notation E[x].
     */
    public static Fraction E(ProbDist d) {
        Fraction mean = 0;
        foreach (var pair in d._pmf) {
            mean += pair.Key * pair.Value;
        }
        return mean;
    }

    /**
     * Calculates the variance of probability distribution d.
     */
    public static Fraction Var(ProbDist d) => E(d^2) - (E(d)^2);

    public static bool operator ==(ProbDist a, ProbDist b) => a._pmf.Count == b._pmf.Count && 
                                                              a._pmf.Keys.SequenceEqual(b._pmf.Keys, new Fraction.Equality()) &&
                                                              a._pmf.Values.SequenceEqual(b._pmf.Values, new Fraction.Equality()) &&
                                                              a._cdf.SequenceEqual(b._cdf, new Fraction.Equality());
    public static bool operator !=(ProbDist a, ProbDist b) => !(a==b);
    public static ProbDist operator +(ProbDist d) => d;
    public static ProbDist operator -(ProbDist d) => new (d._pmf.Select(p => (-p.Key, p.Value)));
    public static ProbDist operator +(ProbDist a, ProbDist b) => Operate(a, b, (d, d1) => d+d1);
    public static ProbDist operator -(ProbDist a, ProbDist b) => a + -b;
    public static ProbDist operator *(ProbDist a, ProbDist b) => Operate(a, b, (d, d1) => d*d1);
    public static ProbDist operator /(ProbDist a, ProbDist b) => Operate(a, b, (d, d1) => d/d1);
    public static ProbDist operator +(ProbDist d, Fraction s) => new (d._pmf.Select(p => (p.Key+s, p.Value)));
    public static ProbDist operator -(ProbDist d, Fraction s) => d + -s;
    public static ProbDist operator *(ProbDist d, Fraction s) => new (d._pmf.Select(p => (p.Key*s, p.Value)));
    public static ProbDist operator /(ProbDist d, Fraction s) => d * (1/s);
    public static ProbDist operator ^(ProbDist d, int pow) => new (d._pmf.Select(p => (p.Key^pow, p.Value)));

    private static ProbDist Operate(ProbDist a, ProbDist b, Func<Fraction, Fraction, Fraction> f) {
        var pmf = new SortedDictionary<Fraction, Fraction>();
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

    public void printPMF() {
        Console.WriteLine($"[{String.Join(", ", _pmf.Select(p => $"{p.Key}: {p.Value}"))}]");
    }
}