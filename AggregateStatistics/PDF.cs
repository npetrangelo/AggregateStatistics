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

    /**
     * Finds sample by doing a binary search on the cdf
     */
    public double Sample() {
        return Percentile(new Random().NextDouble());
    }

    public double Percentile(double percent) {
        // TODO CDF indices should be in center of probability boxes, not on edge
        var index = _cdf.Length / 2;
        while (true) {
            if (index >= _cdf.Length) return _max;
            if (index <= 0) return _min;
            if (percent > _cdf[index - 1] && percent <= _cdf[index]) {
                Console.WriteLine($"index={index}");
                return Utils.Lerp(_min, _max, (double) index/(_cdf.Length - 1));
            }
            
            if (percent > _cdf[index]) {
                index += index / 2;
            } else {
                index /= 2;
            }
        }
    }

    public new string ToString() {
        var values = new double[_pdf.Length];
        for (var i = 0; i < values.Length; i++) {
            values[i] = Utils.Lerp(_min, _max, (double) i/(values.Length - 1));
        }
        return $"PDF={string.Join(", ", values.Zip(_pdf))} CDF={string.Join(", ", values.Zip(_cdf))}";
    }
}