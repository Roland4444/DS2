using Xunit;
using DS2_SRC;
using System;
namespace DS2_TEST;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var Class1 = new FirstClass();
        int res = Class1.summ(5,7);
        Assert.Equal(12, res );
        
    }
}