namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class Tests {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestQuantile() {
        var pmf = new ProbDist(0, 2, new []{ 0.25, 0.5, 0.25 });
        Console.WriteLine(pmf.ToString());
        Assert.True(1 == pmf.Quantile(0.5));
    }
    
    [Test]
    public void TestDownSample() {
        var pmf = new ProbDist(2, 12, new []{ 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0 });
        var expected = new ProbDist(2, 12, new []{ 3.0, 7.0, 11.0, 9.0, 5.0, 1.0 });
        pmf.DownSample(2);
        pmf.printPMF();
        expected.printPMF();
        Assert.True(expected == pmf);
    }
    
    [Test]
    public void TestAddition() {
        var pmf = new ProbDist(1, 6, new []{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
        var expected = new ProbDist(2, 12, new []{ 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0 });
        expected.printPMF();
        (pmf + pmf).printPMF();
        Assert.True(expected == pmf + pmf);
    }
    
    [Test]
    public void TestSubtraction() {
        var pmf = new ProbDist(1, 6, new []{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
        var expected = new ProbDist(-5, 5, new []{ 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0 });
        Assert.True(expected == pmf - pmf);
    }
    
    [Test]
    public void TestMean() {
        var pmf = new ProbDist(1, 6, new []{ 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 });
        Assert.AreEqual(3.5, pmf.Mean());
    }
}