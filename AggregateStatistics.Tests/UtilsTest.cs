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
        Fraction[] arr = { 1, 2, 1 };
        Fraction[] expected = { new (1,4), new (1,2), new (1,4) };
        Assert.AreEqual(expected, arr.Normalize());
    }

    [Test]
    public void TestGCD() {
        Assert.AreEqual(10, Utils.GCD(20, 30));
    }

    [Test]
    public void TestConvolve() {
        Fraction[] pmf = { 1, 1 };
        Fraction[] expected = { 1, 2, 1 };
        Assert.AreEqual(expected, pmf.Convolve(pmf));
    }
    
    [Test]
    public void TestNonuniformConvolve() {
        Fraction[] pmf = { 1, 2 };
        Fraction[] expected = { 1, 4, 4 };
        Assert.AreEqual(expected, pmf.Convolve(pmf));
    }
}