namespace AggregateStatistics;

public class PDF {
    private readonly double _min, _max, _dx;
    private readonly double[] _pdf, _cdf;

    public PDF(double min, double max, double[] pdf) {
        _min = min;
        _max = max;
        _dx = (max - min) / (pdf.Length - 1);
        
        _pdf = pdf.Normalize(); // All PDFs must sum to 1
        _cdf = new double[pdf.Length+1];

        var sum = 0.0;
        _cdf[0] = sum;
        for (var i = 0; i < _pdf.Length; i++) {
            sum += _pdf[i];
            _cdf[i+1] = sum;
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
        var pdfValues = new double[_pdf.Length];
        var cdfValues = new double[_cdf.Length];
        for (var i = 0; i < pdfValues.Length; i++) {
            pdfValues[i] = Utils.Lerp(_min, _max, (double) i/(pdfValues.Length - 1));
            cdfValues[i] = pdfValues[i] - _dx / 2;
        }
        cdfValues[^1] = pdfValues[^1] + _dx / 2;
        return $"PDF={string.Join(", ", pdfValues.Zip(_pdf))} CDF={string.Join(", ", cdfValues.Zip(_cdf))}";
    }
}