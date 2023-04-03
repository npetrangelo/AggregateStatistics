namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestQuantile() {
        var pmf = new PMF(0, 2, new []{ 0.25, 0.5, 0.25 });
        Console.WriteLine(pmf.ToString());
        Assert.AreEqual(1, pmf.Quantile(0.5));
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