namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class ProbDistTests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestQuantile() {
        var pmf = new ProbDist(0, 2, new Fraction[]{ 1, 2, 1 });
        Console.WriteLine(pmf.ToString());
        Assert.True(1 == pmf.Quantile(0.5));
    }
    
    [Test]
    public void TestDownSample() {
        var pmf = new ProbDist(2, 12, new Fraction[]{ 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 });
        var expected = new ProbDist(2, 12, new Fraction[]{ 3, 7, 11, 9, 5, 1 });
        pmf.printPMF();
        pmf.DownSample(2);
        expected.printPMF();
        Assert.True(expected == pmf);
    }
    
    [Test]
    public void TestAddition() {
        var pmf = new ProbDist(1, 6, new Fraction[]{ 1, 1, 1, 1, 1, 1 });
        var expected = new ProbDist(2, 12, new Fraction[]{ 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 });
        expected.printPMF();
        (pmf + pmf).printPMF();
        Assert.True(expected == pmf + pmf);
    }

    [Test]
    public void TestSubtraction() {
        var pmf = new ProbDist(1, 6, new Fraction[]{ 1, 1, 1, 1, 1, 1 });
        var expected = new ProbDist(-5, 5, new Fraction[]{ 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1 });
        Assert.True(expected == pmf - pmf);
    }
    
    [Test]
    public void TestMultiplication() {
        var pmf = new ProbDist(1, 3, new Fraction[]{ 1, 1, 1 });
        var expected = new ProbDist(new Fraction[]{ 1, 2, 3, 4, 6, 9 }, new Fraction[]{ 1, 2, 2, 1, 2, 1 });
        expected.printPMF();
        (pmf * pmf).printPMF();
        Assert.True(expected == pmf * pmf);
    }
    
    [Test]
    public void TestMean() {
        var pmf = new ProbDist(1, 6, new Fraction[]{ 1, 1, 1, 1, 1, 1 });
        Assert.AreEqual(new Fraction(7, 2), ProbDist.E(pmf));
    }

    [Test]
    public void TestVariance() {
        var pmf = new ProbDist(1, 6, new Fraction[]{ 1, 1, 1, 1, 1, 1 });
        var d = pmf - ProbDist.E(pmf);
        pmf.printPMF();
        d.printPMF();
        (d^2).printPMF();
        Console.WriteLine(ProbDist.Var(pmf).ToString());
    }
}