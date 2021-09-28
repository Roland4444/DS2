using Xunit;
using System;
using System.Collections;
namespace DS2_TEST;

public class SaverTest1
{


    [Fact]
    public void TestSaver()
    {
        string filename = "temp";
        ArrayList Arr = new ArrayList();
        Arr.Add("6");
        Arr.Add("66");
        Saver Saver = new Saver();
        Saver.write(Saver.savedToBLOB(Arr), filename);
        ArrayList Restored = (ArrayList) Saver.restored(Saver.readBytes(filename));
        Assert.Equal(2, Restored.Count );
        
        
        
    }
}