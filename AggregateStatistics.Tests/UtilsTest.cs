namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class UtilsTest {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestLerp() {
        Assert.AreEqual(10.0, Utils.Lerp(0.0, 20.0, 0.5));
    }

    [Test]
    public void TestNormalize() {
        double[] arr = { 1, 2, 1 };
        double[] expected = { 0.25, 0.5, 0.25 };
        Assert.AreEqual(expected, arr.Normalize());
    }

    [Test]
    public void TestDoubleEquals() {
        Assert.True(6.0.Equals(5.9999999999, 0.00001));
        Assert.False(6.0.Equals(5.999, 0.00000001));
    }

    [Test]
    public void TestGCD() {
        Assert.AreEqual(10, Utils.GCD(20, 30));
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