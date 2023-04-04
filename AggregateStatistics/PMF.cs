﻿namespace AggregateStatistics;

public class PMF {
    private readonly double _min, _max;
    public double[] _pmf { get; private set; }
    private readonly double[] _cdf;
    private readonly Random _random = new ();
    private const double Tolerance = 0.00000001;

    public PMF(double min, double max, double[] pmf) {
        _min = min;
        _max = max;
        
        _pmf = pmf.Normalize(); // All PDFs must sum to 1
        _cdf = new double[pmf.Length+1];

        var sum = 0.0;
        _cdf[0] = 0.0;
        for (var i = 0; i < _pmf.Length; i++) {
            sum += _pmf[i];
            _cdf[i+1] = sum;
        }
    }

    /**
     * Finds sample by passing a uniform random number between 0 and 1 to the quantile function
     */
    public double Sample() {
        return Quantile(_random.NextDouble());
    }

    /**
     * Finds the quantile by doing a binary search on the CDF.
     * 
     * First quartile -> y=0.25
     * Median -> y=0.5
     * Second quartile -> y=0.75
     */
    public double Quantile(double y) {
        var index = _cdf.Length / 2;
        while (true) {
            if (index >= _cdf.Length) return _max;
            if (index <= 0) return _min;
            if (_cdf[index - 1] < y && y <= _cdf[index]) {
                return Utils.Lerp(_min, _max, (double) (index - 1)/(_pmf.Length - 1));
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
        var newPmf = new double[_pmf.Length % scaleFactor == 0 ? _pmf.Length/scaleFactor : _pmf.Length/scaleFactor + 1];
        for (var i = 0; i < newPmf.Length; i++) {
            newPmf[i] = 0.0;
            for (var j = 0; j < scaleFactor; j++) {
                if (i*scaleFactor + j >= _pmf.Length) break;
                newPmf[i] += _pmf[i*scaleFactor + j];
            }
        }
        _pmf = newPmf;
    }
    
    public static bool operator ==(PMF a, PMF b) => a._min.Equals(b._min, Tolerance) &&
                                                    a._max.Equals(b._max, Tolerance) &&
                                                    a._pmf.Length == b._pmf.Length &&
                                                    a._pmf.SequenceEqual(b._pmf, new DoubleEquality());
    public static bool operator !=(PMF a, PMF b) => !(a==b);
    public static PMF operator +(PMF pmf) => pmf;
    public static PMF operator -(PMF pmf) => new (-pmf._max, -pmf._min, pmf._pmf.Reverse().ToArray());
    public static PMF operator +(PMF a, PMF b) => new (a._min+b._min, a._max+b._max, a._pmf.Convolve(b._pmf));
    public static PMF operator -(PMF a, PMF b) => a + -b;
    public static PMF operator +(PMF pmf, double s) => new (pmf._min + s, pmf._max + s, pmf._pmf);
    public static PMF operator -(PMF pmf, double s) => pmf + -s;
    public static PMF operator *(PMF pmf, double s) => new (pmf._min * s, pmf._max * s, pmf._pmf);
    public static PMF operator /(PMF pmf, double s) => pmf * (1.0/s);
    
    public static PMF Average(PMF a, PMF b) {
        return new PMF((a._min + b._min)/2, (a._max + b._max)/2, a._pmf.Convolve(b._pmf));
    }

    public new string ToString() {
        var pmfValues = new double[_pmf.Length];
        var cdfValues = new string[_cdf.Length];
        for (var i = 0; i < pmfValues.Length; i++) {
            pmfValues[i] = Utils.Lerp(_min, _max, (double) i/(pmfValues.Length - 1));
            var lower = i > 0 ? $"{pmfValues[i-1]}≤" : "";
            cdfValues[i] = $"{lower}x<{pmfValues[i]}";
        }
        cdfValues[^1] = $"x≥{pmfValues[^1]}";
        return $"PMF={string.Join(", ", pmfValues.Zip(_pmf))}";
    }

    private class DoubleEquality : IEqualityComparer<double> {
        public bool Equals(double a, double b) {
            return a.Equals(b, Tolerance);
        }

        public int GetHashCode(double d) {
            return d.GetHashCode();
        }
    }
}