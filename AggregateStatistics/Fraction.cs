namespace AggregateStatistics;

public struct Fraction : IComparable<Fraction> {
    private readonly int _n, _d;

    public Fraction(int numerator, int denominator = 1) {
        _n = numerator * Math.Sign(denominator);
        _d = denominator * Math.Sign(denominator);
        var gcd = Utils.GCD(_n, _d);
        _n /= gcd;
        _d /= gcd;
    }
    
    public static explicit operator double(Fraction f) => (double) f._n / f._d;
    public static implicit operator Fraction(int n) => new (n);

    public static bool operator ==(Fraction a, Fraction b) => a._n == b._n && a._d == b._d;
    public static bool operator !=(Fraction a, Fraction b) => !(a == b);
    public static Fraction operator +(Fraction f) => f;
    public static Fraction operator -(Fraction f) => new (-f._n, f._d);
    public static Fraction operator +(Fraction a, Fraction b) => new (a._n*b._d + b._n*a._d, a._d*b._d);
    public static Fraction operator -(Fraction a, Fraction b) => a + -b;
    public static Fraction operator *(Fraction a, Fraction b) => new (a._n*b._n, a._d*b._d);
    public static Fraction operator /(Fraction a, Fraction b) => new (a._n*b._d, a._d*b._n);

    public int CompareTo(Fraction that) => ((double) this).CompareTo((double) that);

    public override string ToString() => _d == 1 ? $"{_n}" : $"{_n}/{_d}";

    public class Equality : IEqualityComparer<Fraction> {
        public bool Equals(Fraction a, Fraction b) {
            return a == b;
        }

        public int GetHashCode(Fraction d) {
            return d.GetHashCode();
        }
    }
}