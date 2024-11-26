using F3H.ProfileShark.Shared;

namespace F3H.ProfileShark.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestReadProfilesC()
    {
        using var sr = File.OpenRead("76.bin");
        using var br = new BinaryReader(sr);
        var list = DataReader.ReadProfilesC(br); 
    }
}