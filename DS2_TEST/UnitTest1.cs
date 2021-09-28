using Xunit;
using System;
namespace DS2_TEST;

public class UnitTest1
{


    [Fact]
    public void TestChecker()
    {
        var Checker = new Checker();        
        Assert.Equal(true, Checker.isnumber("44") );
        Assert.Equal(false, Checker.isnumber("jfkjgbjfkjbgfb") );
        
    }
}