namespace AggregateStatistics;

public class PDF {
    private readonly double _min, _max;
    private readonly double[] _pdf, _integral;

    public PDF(double min, double max, double[] pdf) {
        _min = min;
        _max = max;
        _pdf = pdf;
        _integral = new double[pdf.Length];

        var sum = 0.0;
        for (var i = 0; i < pdf.Length; i++) {
            sum += pdf[i];
            _integral[i] = sum;
        }
    }

    private static double Lerp(double a, double b, double n) {
        return a*(n-1) + b*n;
    }

    public double Sample() {
        var sample = new Random().NextDouble();
        for (var i = 0; i < _integral.Length; i++) {
            if (_integral[i] > sample) {
                return Lerp(_min, _max, (double) i/_integral.Length);
            }
        }
        return _max;
    }
}