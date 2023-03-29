namespace AggregateStatistics;

public class PDF {
    private double min, max;
    private double[] pdf;
    private double[] integral;

    public PDF(double min, double max, double[] pdf) {
        this.min = min;
        this.max = max;
        this.pdf = pdf;
        integral = new double[pdf.Length];

        var sum = 0.0;
        for (var i = 0; i < pdf.Length; i++) {
            sum += pdf[i];
            integral[i] = sum;
        }
    }
    
    public static double Dot(double[] v1, double[] v2) {
        var dot = 0.0;
        for (var i = 0; i < Math.Min(v1.Length, v2.Length); i++) {
            dot += v1[i] * v2[i];
        }

        return dot;
    }
    
    public static double[] Convolve(double[] f1, double[] f2) {
        var newF = new double[f1.Length + f2.Length - 1];
        for (var i = 0; i < newF.Length; i++) {
            newF[i] = 0.0;
            for (var j = i - (f2.Length - 1); j <= i; j++) {
                if (j < 0 || j >= f1.Length) continue;
                newF[i] += f1[j] * f2[i-j];
            }
        }
        return newF;
    }
}