namespace AggregateStatistics;

public static class Utils {
    public static int Pow(this int a, int b) {
        for (var i = 1; i < b; i++) {
            a *= a;
        }
        return a;
    }
    public static double Lerp(double a, double b, double n) {
        return a*(1-n) + b*n;
    }
    
    public static Fraction Lerp(Fraction a, Fraction b, Fraction n) {
        return a*(1-n) + b*n;
    }

    public static Fraction Sum(this IEnumerable<Fraction> v) => v.Aggregate((a, b) => a + b);
    
    public static Fraction[] Normalize(this IEnumerable<Fraction> enumerable) {
        var v = enumerable.ToArray();
        var sum = v.Sum();
        var normalized = new Fraction[v.Length];
        if (sum == 1) {
            v.CopyTo(normalized, 0);
            return normalized;
        }
        if (sum == 0) throw new DivideByZeroException();
        Parallel.For(0, v.Length, i => {
            normalized[i] = v[i] / sum;
        });
        return normalized;
    }

    public static int GCD(int a, int b) {
        while (b != 0) {
            var t = b;
            b = a % b;
            a = t;
        }
        return a;
    }
    
    public static Fraction[] Convolve(this IEnumerable<Fraction> e1, IEnumerable<Fraction> e2) {
        var f1 = e1.ToArray();
        var f2 = e2.ToArray();
        var newF = new Fraction[f1.Length + f2.Length - 1];
        Parallel.For(0, newF.Length, i => {
            newF[i] = 0;
            for (var j = i - (f2.Length - 1); j <= i; j++) {
                if (j < 0 || j >= f1.Length) continue;
                newF[i] += f1[j] * f2[i-j];
            }
        });
        return newF;
    }
}