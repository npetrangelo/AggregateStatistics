namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestConvolve() {
        double[] pdf = { 1.0/2, 1.0/2 };
        double[] expected = { 1.0/4, 2.0/4, 1.0/4 };
        Assert.AreEqual(expected, PDF.Convolve(pdf, pdf));
    }
}