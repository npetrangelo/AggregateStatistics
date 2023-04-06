namespace AggregateStatistics.Tests;

using AggregateStatistics;
public class FractionTest {
    [SetUp]
    public void Setup() {
    }

    [Test]
    public void TestConversion() {
        Assert.AreEqual(0.5, (double) new Fraction(1, 2));
    }

    [Test]
    public void TestEquals() {
        Assert.True(new Fraction(1, 2) == new Fraction(2, 4));
    }
    
    [Test]
    public void TestNegate() {
        Assert.AreEqual(-new Fraction(1, 6), new Fraction(-1, 6));
    }

    [Test]
    public void TestAdd() {
        var sum = new Fraction(1, 2) + new Fraction(1, 3);
        Assert.AreEqual(new Fraction(5, 6), sum);
        sum = new Fraction(1, 2) + 1;
        Assert.AreEqual(new Fraction(3, 2), sum);
    }
    
    [Test]
    public void TestSubtract() {
        var diff = new Fraction(1, 2) - new Fraction(1, 3);
        Assert.AreEqual(new Fraction(1, 6), diff);
    }
    
    [Test]
    public void TestMultiply() {
        var product = new Fraction(1, 2) * new Fraction(2, 3);
        Assert.AreEqual(new Fraction(1, 3), product);
    }
    
    [Test]
    public void TestDivide() {
        var qotient = new Fraction(2, 3) / 2;
        Assert.AreEqual(new Fraction(1, 3), qotient);
    }
}