namespace AggregateStatistics;

public class PDF {
    private readonly double _min, _max;
    private readonly double[] _pdf, _cdf;

    public PDF(double min, double max, double[] pdf) {
        _min = min;
        _max = max;
        _pdf = pdf.Normalize(); // All PDFs must sum to 1
        _cdf = new double[pdf.Length];

        var sum = 0.0;
        for (var i = 0; i < _pdf.Length; i++) {
            sum += _pdf[i];
            _cdf[i] = sum;
        }
    }

    private static double Lerp(double a, double b, double n) {
        return a*(n-1) + b*n;
    }

    /**
     * Finds sample by doing a binary search on the cdf
     */
    public double Sample() {
        var sample = new Random().NextDouble();
        var index = _cdf.Length / 2;
        while (true) {
            if (index >= _cdf.Length) return _max;
            if (index <= 0) return _min;
            if (sample >= _cdf[index] && sample < _cdf[index + 1]) {
                return Lerp(_min, _max, (double) index/_cdf.Length);
            }
            
            if (sample > _cdf[index + 1]) {
                index += index / 2;
            } else {
                index /= 2;
            }
        }
    }
}