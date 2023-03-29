namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestPercentile() {
        var pdf = new PDF(1, 3, new []{ 1.0, 1.0, 1.0 });
        Console.WriteLine(pdf.ToString());
        Console.WriteLine(pdf.Percentile(0.5));
    }

    [Test]
    public void TestConvolve() {
        double[] pdf = { 1, 1 };
        double[] expected = { 1, 2, 1 };
        Assert.AreEqual(expected, pdf.Convolve(pdf));
    }
    
    [Test]
    public void TestNonuniformConvolve() {
        double[] pdf = { 1, 2 };
        double[] expected = { 1, 4, 4 };
        Assert.AreEqual(expected, pdf.Convolve(pdf));
    }
}