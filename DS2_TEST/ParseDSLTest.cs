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
        Assert.Equal(ObjectRules, parser.getDSLRulesfromString(input));
    }
    
    [Fact]
    public void testParseRole() {
        var readRole = new Role("read","", parser);
        Assert.Equal(readRole.toString(), parser.parseRole(input).toString());
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
        var etalon= mutableListOf<Any>();
        etalon.add("12");
        etalon.add("Добрый день");
        Assert.Equal(etalon.toString(), parser.Atom(simple).toString() );
    }
    [Fact]
    public void
      testGetAtom() {
        var keyvalue= KeyValue("key",12);
        //keyvalue.put("key",12)
        Assert.Equal("", parser.Atom(""));
        Assert.Equal(true, "121212".contains("12"));
        Assert.Equal(12,parser.Atom("12"));
        Assert.Equal("xyz",parser.Atom("'xyz'"));
        Assert.Equal(keyvalue, parser.Atom("'key':12"));
    }
    [Fact]
    public void
    testGetKey() {
        var example = "'таблица':12";
        Assert.Equal("'таблица'", parser.getKey_(example));
    }
    [Fact]
    public void
    testClearString() {
        var initial = "12, 'Добрый день',  122";
        var etalon = "12,'Добрый день',122";
        var param2 = "12 ,  '    Добрый день', 'табли   цы': [  '  к  а  с с а',' скл ад',     'приход'], '12 декабря'";
        var etalon2 = "12,'    Добрый день','табли   цы':['  к  а  с с а',' скл ад','приход'],'12 декабря'";
        Assert.Equal(etalon, parser.prepare_(initial));
        Assert.Equal(etalon2, parser.prepare_(param2));
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
        Assert.Equal("'urldb'", parser.getKey_(initial));
        Assert.Equal("jdbc:mysql://192.168.0.121:3306/psa", parser.getValue_(initial));
        Assert.Equal(Atom.None, parser.getType(parser.getValue_(initial)));

    }

    [Fact]
    public void
    testGetValue_() {
        var initial = "'12':[12,12,56]";
        var etalon = "[12,12,56]";
        Assert.Equal(etalon,parser.getValue_(initial));

        var arr = mutableListOf(12,12,56);
        var etalonMap = KeyValue("12", arr);
        Assert.Equal(etalonMap, parser.Atom(initial));
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
        Assert.Equal(initial, parser.Head(initial));;
        Assert.Equal("", parser.Tail(initial));
        println(parser.getType(""));;
        Assert.Equal(Atom.KeyValue, parser.getType(initial));
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
        print(parser.Head(initial2));
        print("tail>>"+parser.Tail(initial3));
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
        var tupple = "[$initial3]";
        var etalonMap = mutableMapOf<String, Any>();
        var arr = mutableListOf(12,12);
        var arr2 = mutableListOf<Any>();
        arr2.add(arr);
        arr2.add(12);
        etalonMap.put("12", arr2);
        Assert.Equal("", parser.Tail(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(initial));
        Assert.Equal(Atom.Tupple, parser.getType(parser.getValue_(initial)));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial));

        Assert.Equal(Atom.KeyValue, parser.getType(parser.Head(initial)));
        Assert.Equal(Expression.One, parser.getTypeExpression(initial));
        Assert.Equal("'12':[[12,12],12]", parser.Head(initial3));
        Assert.Equal("'12':12", parser.Tail(initial3));
        Assert.Equal("", parser.Tail(tupple));
        Assert.Equal("[$initial3]", parser.Head(tupple));

    }

    [Fact]
    public void
    testSequencetoList(){
        var initial = "12,'5','f',56";
        var lst = mutableListOf<String>();
        lst.add("12");
        lst.add("'5'");
        lst.add("'f'");
        lst.add("56");
        Assert.Equal(lst, parser.getList(initial));
    }

    [Fact]
    public void
    testSequence(){
        var initial = "12,'aaaa','f':56";
        var initial2 = "'12','aaaa','f':56";
        Assert.Equal(Atom.Sequence, parser.getType(initial));
        Assert.Equal(Atom.Number, parser.getType(parser.Head(initial)));
        Assert.Equal(Atom.String, parser.getType(parser.Head(initial2)));
    }
    [Fact]
    public void
    testGetTupple() {
        var initial = "[12,'aaaa','f':56]";
        var etalon = mutableListOf<Any>();
        etalon.add(12);
        etalon.add("aaaa");
        var keyvalue= KeyValue("f",56);
        etalon.add(keyvalue);
        Assert.Equal(etalon, parser.Atom(initial));
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
        var str =  "'requests' => ::read{'tupple':[\"a\",\"b\",\"c\"]}, ::write{<\"load\":12,\"pay\":40>}, ::create{<\"number\":\"\"$90\"\",\"metal\":[\"алюминий\",\"сталь\",\"никель\"]>}.";
        var rawdsl = parser.getRawDSLForRole(str, "read");        
        Assert.Equal("'tupple':[\"a\",\"b\",\"c\"]", rawdsl);

    }


}