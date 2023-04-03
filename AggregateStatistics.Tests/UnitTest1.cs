namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
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
    
    [Test]
    public void TestQuantile() {
        var pmf = new PMF(0, 2, new []{ 0.25, 0.5, 0.25 });
        Console.WriteLine(pmf.ToString());
        Assert.AreEqual(1, pmf.Quantile(0.5));
    }
    
    [Test]
    public void TestAddition() {
        var pmf = new PMF(1, 6, new []{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
        var expected = new PMF(2, 12, new []{ 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0 });
        Console.WriteLine(expected.ToString());
        Console.WriteLine((pmf + pmf).ToString());
        Assert.That(expected._pmf, Is.EqualTo((pmf + pmf)._pmf).Within(0.0000001d));
    }
}