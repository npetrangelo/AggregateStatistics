namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestDot() {
        double[] v1 = { 1, 1 };
        Assert.AreEqual(2.0, PDF.Dot(v1, v1));
    }

    [Test]
    public void TestConvolve() {
        double[] pdf = { 1, 1 };
        double[] expected = { 1, 2, 1 };
        Assert.AreEqual(expected, PDF.Convolve(pdf, pdf));
    }
    
    [Test]
    public void TestNonuniformConvolve() {
        double[] pdf = { 1, 2 };
        double[] expected = { 1, 4, 4 };
        Assert.AreEqual(expected, PDF.Convolve(pdf, pdf));
    }
}