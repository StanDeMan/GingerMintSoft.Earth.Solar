namespace GingerMintSoft.Earth.Location.Tests;

[TestClass]
public class ShortGuidTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var shortGuid = Guid.NewGuid().ToString("N").Substring(0, 12);

        Assert.AreEqual(12, shortGuid.Length);
        Console.WriteLine(shortGuid);
    }
}
