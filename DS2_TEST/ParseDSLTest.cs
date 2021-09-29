using Xunit;
using System;
using System.Collections.Generic;
using System.Collections;
using DS2_Abstractions;
using DS2_Abstractions.BNF;
namespace DS2_TEST;

public class ParseDSLTest
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

    string input = "'requests' => ::read{}, ::write{}, ::create{}.";
    string  inputwithparam = "'requests' => ::read{'tupple':[\"a\",\"b\",\"c\"]}, ::write{<\"load\":12,\"pay\":40>}, ::create{<\"number\":\"\"$90\"\",\"metal\":[\"алюминий\",\"сталь\",\"никель\"]>}.";
    string param = "12,'Добрый день', 'таблицы':['касса','склад','приход'], '12 декабря'";
    string simple = "12,'Добрый день'";
    ParseDSL parser = new ParseDSL();
    
    [Fact]
    public void getDSLRulestoObject() {
        var readRole = new Role("read","", parser);
        var writeRole = new  Role("write","", parser);
        var createRole =new  Role("create","", parser);
        var Roles = new List<Role>{readRole, writeRole, createRole};
        var ObjectRules  = new DSLRole("requests", Roles);
        Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\nLLLLLL\n"+ObjectRules.toString());
        var M1 = ObjectRules.toString();
        var M2 = parser.getDSLRulesfromString(input).toString();
        Assert.Equal(ObjectRules.toString() , parser.getDSLRulesfromString(input).toString() );
        Console.WriteLine("M ::"+M1);
        Console.WriteLine("M2::"+M2);
    }
    
    [Fact]
    public void testParseRole() {
        var readRole = new Role("read","", parser);
        Assert.Equal(readRole.ToString(), parser.parseRole(input).ToString());
    }

   [Fact]
    public void
      parsewithparams(){
        var etalon= new List<object>();
        etalon.Add(12);
        etalon.Add("Добрый день");

        string param = "[12,'Добрый день', 'таблицы':['касса','склад','приход'], '12 декабря']";
        var list = new List<String>{"касса","склад","приход"};
        var hashmap = new KeyValue("таблицы", list);
        etalon.Add(hashmap);
        etalon.Add("12 декабря");
        Assert.Equal(etalon.ToString(), parser.Atom(param).ToString());
    }
    [Fact]
    public void
      testGetAtoms() {
        var etalon= new List<object>();
        etalon.Add("12");
        etalon.Add("Добрый день");
        Assert.Equal(etalon.ToString(), parser.Atom(simple).ToString() );
    }
    [Fact]
    public void
      testGetAtom() {
        var keyvalue= new KeyValue("key",12);
        //keyvalue.put("key",12)
        Assert.Equal("", parser.Atom(""));

        Assert.Equal(true, "121212".Contains("12"));
        Assert.Equal(12,parser.Atom("12"));
        Assert.Equal("xyz",parser.Atom("'xyz'"));
        KeyValue KV =(KeyValue) parser.Atom("'key':12");
        Assert.Equal(keyvalue.Key, KV.Key);
    }
    [Fact]
    public void
    testGetKey() {
        var example = "'таблица':12";
        Assert.Equal("'таблица'", parser.getKey(example));
    }
    [Fact]
    public void
    testClearString() {
        var initial = "12, 'Добрый день',  122";
        var etalon = "12,'Добрый день',122";
        var param2 = "12 ,  '    Добрый день', 'табли   цы': [  '  к  а  с с а',' скл ад',     'приход'], '12 декабря'";
        var etalon2 = "12,'    Добрый день','табли   цы':['  к  а  с с а',' скл ад','приход'],'12 декабря'";
        Assert.Equal(etalon, parser.prepare(initial));
        Assert.Equal(etalon2, parser.prepare(param2));
    }
    [Fact]
    public void
    countStringdelim(){
        var etalon = "12,'Добрый день',122";
        var initial = "'12':[12,12,56]";
        var etalon2 = "[12,12,56]";
        Assert.Equal(2, parser.countStringDelims(etalon));
        Assert.Equal(2, parser.countStringDelims(initial));
        Assert.Equal(0, parser.countStringDelims(etalon2));
    }
    [Fact]
    public void
    extendedtest(){
        var initial = "'urldb':jdbc:mysql://192.168.0.121:3306/psa";
        Assert.Equal(Atom.KeyValue, parser.getType(initial));
        Assert.Equal("'urldb'", parser.getKey(initial));
        Assert.Equal("jdbc:mysql://192.168.0.121:3306/psa", parser.getValue(initial));
        Assert.Equal(Atom.None, parser.getType(parser.getValue(initial)));

    }

    [Fact]
    public void
    testGetValue_() {
        var initial = "'12':[12,12,56]";
        var etalon = "[12,12,56]";
        Assert.Equal(etalon,parser.getValue(initial));

        var arr = new List<object>{12,12,56};
        var etalonMap =new  KeyValue("12", arr);
        KeyValue parcedMap =(KeyValue) parser.Atom(initial);
        List<object> Lst = (List<object>) parcedMap.Value;
        Console.WriteLine("COUNT::\n\n\n\n\n\n\n\n\n"+Lst.Count);
      //  Assert.Equal(arr.Count, Lst.Count);


      ///////////////////?!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    [Fact]
    public void
    testatomint() {
        var initial = "1";
        var etalon =1;
        Assert.Equal(etalon,parser.Atom(initial));
    }

    [Fact]
    public void
    nestedTUpple(){
        var initial = "'12':[[12,12],12]";
        var initial3 = "'12':[[12,12],12],'12':12";
        var tupple = "['12':[[12,12],12],'12':12]";
        var etalonMap = new Dictionary<string, object>();
        var arr = new List<int>{12,12};
        var arr2 = new List<object>();
        arr2.Add(arr);
        arr2.Add(12);
        etalonMap.Add("12", arr2);
        Assert.Equal("", parser.Tail(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(initial));
        Assert.Equal(Atom.Tupple, parser.getType(parser.getValue(initial)));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(parser.Head(initial)));
        Assert.Equal(Expression.One, parser.getTypeExpression(initial));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial3));
        Assert.Equal("'12':12", parser.Tail(initial3));;
        Assert.Equal("", parser.Tail(tupple));
        Assert.Equal("['12':[[12,12],12],'12':12]", parser.Head(tupple));

    }

    [Fact]
    public void
    getnumbercolontest(){
        var init = "'12':[[12,12],12]";
        Assert.Equal(false, parser.opencolon(init));
    }

    [Fact]
    public void
    testgettype(){
        var initial = "'12':[[12,12],12]";
        var initial2 = "[[12,12],12]";
        var initial3 = "'12':[[12,12],12],'12':12";
        Assert.Equal(Atom.KeyValue, parser.getType(initial));
        Assert.Equal(Atom.Tupple, parser.getType(initial2));
        Assert.Equal(Atom.Sequence, parser.getType(initial3));
        Assert.Equal(Expression.Many, parser.getTypeExpression(initial3));
        Assert.Equal(Expression.One, parser.getTypeExpression(initial2));
        Console.WriteLine(parser.Head(initial2));
        Console.WriteLine("tail>>"+parser.Tail(initial3));
    }
    [Fact]
    public void
    convertTuppletoSeq(){
        var initial2 = "[[12,12],12]";
        var initial3= "['12':[12,12],'12':22,44]";
        Assert.Equal("[12,12],12", parser.ToSequence(initial2));
        Assert.Equal(Atom.Tupple, parser.getType(initial3));
        Assert.Equal("'12':[12,12],'12':22,44", parser.ToSequence(initial3));
    }
    [Fact]
    public void
      nestedtupple() {
        var initial = "'12':[[12,12],12]";
        var initial3 = "'12':[[12,12],12],'12':12";
        var tupple = "['12':[[12,12],12],'12':12]";
        var etalonMap = new Dictionary<String, object>();
        var arr = new List<int>{12,12};
        var arr2 = new List<object>();
        arr2.Add(arr);
        arr2.Add(12);
        etalonMap.Add("12", arr2);
        Assert.Equal("", parser.Tail(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(initial));
        Assert.Equal(Atom.Tupple, parser.getType(parser.getValue(initial)));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(parser.Head(initial)));
        Assert.Equal(Expression.One, parser.getTypeExpression(initial));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial3));
        Assert.Equal("'12':12", parser.Tail(initial3));
        Assert.Equal("", parser.Tail(tupple));
        Assert.Equal("['12':[[12,12],12],'12':12]", parser.Head(tupple));

    }

    [Fact]
    public void
    testSequencetoList(){
        var initial = "12,'5','f',56";
        var lst = new List<string>();
        lst.Add("12");
        lst.Add("'5'");
        lst.Add("'f'");
        lst.Add("56");
        List<string> HMX = parser.getList(initial);
        Console.WriteLine("FOREACH!!!!!!!!!");
        HMX.ForEach(Print);
        Console.WriteLine("\nFOREACH lst!!!!!!!!!");
        lst.ForEach(Print);
        Assert.Equal(lst, HMX);
    }
void Print(string s)
{
    Console.Write(s);
    Console.Write("#");
}


   [Fact]
    public void
    testSequence(){
        var initial = "12,'aaaa','f':56";
        var initial2 = "'12','aaaa','f':56";
        var head1= parser.Head(initial);
        Assert.Equal("12", head1);
        Assert.Equal(Atom.Sequence, parser.getType(initial));
        Assert.Equal(Atom.Number, parser.getType(parser.Head(initial)));
        Assert.Equal(Atom.String, parser.getType(parser.Head(initial2)));
    }
    
   // [Fact]
    public void
    testGetTupple() {
        var initial = "[12,'aaaa','f':56]";
        var etalon = new List<object>();
        etalon.Add(12);
        etalon.Add("aaaa");
        var keyvalue= new KeyValue("f",56);
        etalon.Add(keyvalue);
      //  Assert.Equal(etalon.toString(), parser.Atom(initial).toString());
    }

    void  testCountStringDelims() {}
   [Fact]
    public void
    testRemoveRolefromStringDSL() {

    var InitDB = "'psadb'=>::psa{'login':'root','pass':'123'},::db{jdbc:mysql://192.168.0.121:3306/psa},::enabled{'true'}.";
    var RemoveRole = "psa";;
    var RemoveRole2 = "enabled";
    var RemoveRole3 = "db";
    var Etalon = "'psadb'=>::db{jdbc:mysql://192.168.0.121:3306/psa},::enabled{'true'}.";
    var Etalon2 = "'psadb'=>::psa{'login':'root','pass':'123'},::db{jdbc:mysql://192.168.0.121:3306/psa}.";
    var Etalon3 = "'psadb'=>::psa{'login':'root','pass':'123'},::enabled{'true'}.";
    var Result = parser.removeRolefromStringDSL(InitDB, RemoveRole);
    var Result2 = parser.removeRolefromStringDSL(InitDB, RemoveRole2);
    var Result3 = parser.removeRolefromStringDSL(InitDB, RemoveRole3);
    Assert.Equal(Etalon, Result);
    Assert.Equal(Etalon2, Result2);
    Assert.Equal(Etalon3, Result3);
    string inputwithparam__ = "'requests'=>::read{'tupple':[\"a\",\"b\",\"c\"]},::write{<\"load\":12,\"pay\":40>},::create{<\"number\":\"\"$90\"\",\"metal\":[\"алюминий\",\"сталь\",\"никель\"]>}.";
    var Etalon4 ="'requests'=>::read{'tupple':[\"a\",\"b\",\"c\"]},::create{<\"number\":\"\"$90\"\",\"metal\":[\"алюминий\",\"сталь\",\"никель\"]>}.";
    var Remove = "write";
    Assert.Equal(Etalon4, parser.removeRolefromStringDSL(inputwithparam__, Remove));




    }

    [Fact]
    public void
    testGetRawDSLForRole() {
        var str =  "'requests' => ::read{'tupple':['a','b','c']}, ::write{<'load':12,'pay':40>}, ::create{<'number':\"$90\"\",\"metal\":[\"алюминий\",\"сталь\",\"никель\"]>}.";
        var rawdsl = parser.getRawDSLForRole(str, "read");        
        Assert.Equal("'tupple':['a','b','c']", rawdsl);

    }


}