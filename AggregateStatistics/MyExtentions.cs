namespace AggregateStatistics;

public static class MyExtentions {
    public static double[] Normalize(this double[] v) {
        var sum = v.Sum();
        var normalized = new double[v.Length];
        Parallel.For(0, v.Length, i => {
            normalized[i] = v[i] / sum;
        });
        return normalized;
    }
    
    public static double[] Convolve(this double[] f1, double[] f2) {
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