namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestQuantile() {
        var pmf = new PMF(1, 3, new []{ 1.0, 1.0, 1.0 });
        Console.WriteLine(pmf.ToString());
        Console.WriteLine(pmf.Quantile(0.5));
    }

    [Test]
    public void TestConvolve() {
        double[] pmf = { 1, 1 };
        double[] expected = { 1, 2, 1 };
        Assert.AreEqual(expected, pmf.Convolve(pmf));
    }
    
    [Test]
    public void TestNonuniformConvolve() {
        double[] pmf = { 1, 2 };
        double[] expected = { 1, 4, 4 };
        Assert.AreEqual(expected, pmf.Convolve(pmf));
    }
}