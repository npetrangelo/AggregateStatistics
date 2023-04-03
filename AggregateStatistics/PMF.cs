namespace AggregateStatistics;

public class PMF {
    private readonly double _min, _max, _dx;
    private readonly double[] _pmf, _cdf;
    private readonly Random _random = new Random();

    public PMF(double min, double max, double[] pmf) {
        _min = min;
        _max = max;
        _dx = (max - min) / (pmf.Length - 1);
        
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
        // TODO CDF indices should be in center of probability boxes, not on edge
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

    // public PDF DownSample(int scaleFactor) {
    //     // TODO Combine adjacent buckets together to avoid memory leaks
    // }

    public PMF Sum(PMF that) {
        return new PMF(_min + that._min, _max + that._max, _pmf.Convolve(that._pmf));
    }

    public PMF Average(PMF that) {
        return new PMF((_min + that._min)/2, (_max + that._max)/2, _pmf.Convolve(that._pmf));
    }

    public new string ToString() {
        var pmfValues = new double[_pmf.Length];
        var cdfValues = new string[_cdf.Length];
        for (var i = 0; i < pmfValues.Length; i++) {
            pmfValues[i] = Utils.Lerp(_min, _max, (double) i/(pmfValues.Length - 1));
            var lower = i > 0 ? $"{pmfValues[i-1]} ≤ " : "";
            cdfValues[i] = $"{lower}x < {pmfValues[i]}";
        }
        cdfValues[^1] = $"x ≥ {pmfValues[^1]}";
        return $"PMF={string.Join(", ", pmfValues.Zip(_pmf))}\nCDF={string.Join(", ", cdfValues.Zip(_cdf))}";
    }
}