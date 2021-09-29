using Xunit;
using System;
using DS2_Abstractions;
using System.Collections;
using System.Collections.Generic;
using ExtensionMethods;
namespace DS2_TEST;

public class DSLRoleTest
{


    [Fact]
    public void TestChecker()
    {
        var Checker = new Checker();        
        Assert.Equal(true, Checker.isnumber("44") );
        Assert.Equal(false, Checker.isnumber("jfkjgbjfkjbgfb") );
        
    }


    ParseDSL parser = new ParseDSL();
    Saver Saver = new Saver();
    [Fact]
    public void testTestToString() {
         var readRole = new Role("read","{}", parser);
         var writeRole = new Role("write","{}", parser);
         var createRole = new Role("create","{}", parser);
         var Roles = new List<Role>{readRole, writeRole, createRole};
         var dsl = new DSLRole("requests", (List<Role>)Roles);
         Assert.NotNull( dsl.toString());
         Console.WriteLine(dsl.toString());
     }

     [Fact]
     public void testextensions(){
         string r = "substring".sbstr(2,4);
         Assert.Equal("bs", r);

     }
    
    [Fact]
    public void testsave(){
        var readRole = new Role("read","{}", parser);
        var writeRole = new Role("write","{}", parser);
        var createRole = new Role("create","{}", parser);
        var Roles = new List<Role>{readRole, writeRole, createRole};
        var dsl = new DSLRole("requests", Roles);
        var bytes =  Saver.savedToBLOB(dsl);
        var restored =  (DSLRole) Saver.restored(bytes);
        Assert.Equal(dsl.toString(), restored.toString());
    }


    // @Test
    // fun testjson(){
    //     var JSONArra = JsonArray()

    // }

}