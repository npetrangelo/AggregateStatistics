namespace AggregateStatistics;

public static class Utils {
    public static double Lerp(double a, double b, double n) {
        return a*(1-n) + b*n;
    }
    
    public static Fraction Lerp(Fraction a, Fraction b, Fraction n) {
        return a*(1-n) + b*n;
    }
    
    public static double[] Normalize(this double[] v) {
        var sum = v.Sum();
        var normalized = new double[v.Length];
        if (sum == 1.0) {
            v.CopyTo(normalized, 0);
            return normalized;
        }
        if (sum == 0.0) throw new DivideByZeroException();
        Parallel.For(0, v.Length, i => {
            normalized[i] = v[i] / sum;
        });
        return normalized;
    }
    
    public static bool Equals(this double a, double b, double tolerance) {
        return Math.Abs(a - b) < tolerance;
    }

    public static int GCD(int a, int b) {
        while (b != 0) {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
    
    public static double[] Convolve(this IEnumerable<double> e1, IEnumerable<double> e2) {
        var f1 = e1.ToArray();
        var f2 = e2.ToArray();
        var newF = new double[f1.Length + f2.Length - 1];
        Parallel.For(0, newF.Length, i => {
            newF[i] = 0.0;
            for (var j = i - (f2.Length - 1); j <= i; j++) {
                if (j < 0 || j >= f1.Length) continue;
                newF[i] += f1[j] * f2[i-j];
            }
        });
        return newF;
    }
}