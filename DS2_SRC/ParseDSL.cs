using System;
using DS2_Abstractions;
using System.Collections.Generic;
using System.Text;
using DS2_Abstractions.BNF;
using ExtensionMethods;

[Serializable]
public class ParseDSL{
   
    public Checker checker =  new Checker();
    public  DSLRole getDSLRulesfromString(string input) 
    {
        var objectName = input.sbstr(input.IndexOf("'")+1, input.LastIndexOf("'"));
        Console.WriteLine(string.Format(@"Loading rules for object <{0}>", objectName));
        return new DSLRole(objectName, parseRoles(input));
    }
    public Role parseRole(string input) 
    {
        if (input.IndexOf("{") == -1) 
            return null;
        var rolename= input.sbstr(input.IndexOf("::")+2, input.IndexOf("{"));
        var param____s="";
        if (input.IndexOf("{")<input.IndexOf("}")-1)
            param____s = input.sbstr(input.IndexOf("{") + 1, input.IndexOf("}"));
        else param____s="";
        if ((rolename.Length == 0) || (rolename ==null)) 
            return null;
        return new Role(rolename, param____s, this);
    }
    public List<Role> parseRoles(string input__) {
        var input = prepare(input__);
        List<Role> result  =new List<Role>();
        var initialString = input;
        var role = parseRole(initialString);
        while (role != null){
            result.Add(role);
            initialString = initialString.sbstr(initialString.IndexOf("}")+1);
            role  = parseRole(initialString);
        };
        return result;
    }

    public string ToSequence(string input__){
        var input = prepare(input__);
        switch (getType(input)){            
            case DS2_Abstractions.BNF.Atom.Tupple: return input.sbstr(1, input.Length-1);
        };
        return input;
    }
     public int countStringDelims(string input){
        var counter = 0;
        for (int i=0; i<=input.Length-1; i++){
            if (input[i]=='\'')
                counter++;
        }
        return counter;
    }
    public object getType(string input_){
        string input = prepare(input_);
        if (input.Equals(""))
            return DS2_Abstractions.BNF.Atom.Empty;
        if ((Head(input) != "") && (Tail(input)!=""))
            return DS2_Abstractions.BNF.Atom.Sequence;
        if ((input[0]=='[') && (input[input.Length-1]==']'))
            return DS2_Abstractions.BNF.Atom.Tupple;
        if ((input.IndexOf("'")>=0) && (input.IndexOf(":")<0)&&(countStringDelims(input)==2))
            return DS2_Abstractions.BNF.Atom.String;
        if ((input.IndexOf("'")>=0) && (input.IndexOf(":")>0) && (Tail(input)=="")) 
            return DS2_Abstractions.BNF.Atom.KeyValue;
        if  (checker.isnumber(input))
            return DS2_Abstractions.BNF.Atom.Number;
        return DS2_Abstractions.BNF.Atom.None;
    }
    public object getTypeExpression(string input){
        if (input.Length==0) return Expression.Empty;
        if ((Head(input)!="") && (Tail(input)=="")) return Expression.One;
        if (Tail(input)!="") return Expression.Many;
        return Expression.Empty;
    }

    public string Head(string input) {
        Console.WriteLine("INTO HEAD");
        Console.WriteLine("input"+input);
        var p = getnumberopencolon(input);
        Console.WriteLine("P::"+p);
        if (p>0)
            return input.sbstr(0, p);
        return input;
    }

    public string Tail(string input){
        Console.WriteLine("INTO Tail");
        Console.WriteLine("input\n"+Tail);
        var p =getnumberopencolon(input);
        if (p>0){

            return input.sbstr(p+1, input.Length);
        }
        return "";
    }

    public int getnumberopencolon(string input){
      ///  Console.WriteLine(input);
        var colonbuf = loadcolons(input);
        bool mustret = false;
        int retvalue=0;
        if (colonbuf.Capacity<=0)
            return -1;

        for (int i=0; i<colonbuf.Count; i++){
            int a = colonbuf[i];
             var index = a;
            var closetupple = 0;
            var opentupple = 0;
            while (index>=0){
                if (input[index]==']')
                    closetupple++;          
                if (input[index]=='[')
                    opentupple++;             
                index--;
            }
            if (opentupple==closetupple)
                return a;  
        }    
       
        return -1;
    }




        public List<int> loadcolons(string input)
        {
            var colonbuff = new List<int>();
            for (int i= 0; i<=input.Length-1; i++)
                if (input[i]==',')
                    colonbuff.Add(i);
            return colonbuff;
        }


    public bool opencolon(string input)
    {
        bool mustret =false;
        var colonbuf = loadcolons(input);
        if (colonbuf.Count<=0)
            return false;
        colonbuf.ForEach(delegate(int a)
        {    
            var index = a;
            var closetupple = 0;
            var opentupple = 0;
            while (index>=0){
                if (input[index]==']')
                    closetupple++;
                if (input[index]=='[')
                    opentupple++;                
                index--;
            }
             if (opentupple==closetupple){
                 mustret = true;
                 
            }
        });
        if (mustret)
            return mustret;
        return false;

    }


    public List<string> getList(string input) {
        Console.WriteLine("input::"+input);
        var lst = new List<string>();
        var head = Head(input);
        var tail = Tail(input);
        Console.WriteLine(string.Format(@"HEAD::{0}  TAIL::{1}", head, tail));
        while ((head!="") ){
            lst.Add(head);
            head = Head(tail);
            tail = Tail(tail);
            Console.WriteLine(string.Format(@"HEAD::{0}  TAIL::{1}", head, tail));
        }
        return lst;
    }
    public object Atom(string input){
        var type = getType(input);
        Console.WriteLine($"TYPE @ {input}  = {type}");
        var map = new Dictionary<String, object>();
        var lst = new List<object>();
        switch (type){
            case DS2_Abstractions.BNF.Atom.String:return (input.Replace("'","")); 
            case DS2_Abstractions.BNF.Atom.Number:{
                if (!input.Contains("."))
                    return Int32.Parse(input);
                return float.Parse(input);                
            };
            case DS2_Abstractions.BNF.Atom.KeyValue:{
                var key = Atom(getKey(input)).ToString();
                var value = Atom(getValue(input));
                if (value != null) {
                    map.Add(key, value);
                };
                return new KeyValue(key, value);
            };
            case DS2_Abstractions.BNF.Atom.Sequence:{
                var lst2 = getList(input);
                lst2.ForEach(delegate(string a)
                {    
                    lst.Add(Atom(a)); 
                });
                return lst;
            };
            case DS2_Abstractions.BNF.Atom.Tupple:
                return Atom(ToSequence(input));
            case DS2_Abstractions.BNF.Atom.None:
                return input;
            
        }
        return "";
    }
    
    
    public string getValue(string input){
        var index = input.IndexOf(":");
        return prepare(input.sbstr(index+1, input.Length));
    }

    public string prepare(string input__){
        Console.WriteLine(input__);
        var buffer = new StringBuilder();
        var appendWhite = false;
        var currentString = input__;
        if (currentString.IndexOf("'")<0)
            return currentString.Replace(" ","");
        for (int i= 0; i<=input__.Length-1; i++){
            if ((input__[i]==" "[0]) && !appendWhite)
                continue;
            if ((input__[i]=="'"[0]) && !appendWhite)
                appendWhite = true;
            else if ((input__[i]=="'"[0]) && appendWhite)
                appendWhite = false;
            buffer.Append(input__[i]);
        }
        return buffer.ToString();
    }  

    public string getKey(string input___){
        var index = input___.IndexOf(":");
        var key = input___.sbstr(0, index).Replace(" ","");
        return key;
    }


    public string removeRolefromStringDSL(string inputDSL, string  RoleName)
    {
        var Template = "::"+RoleName+"{";
        var Index = inputDSL.IndexOf(Template);
        if (Index<0)
            return inputDSL;
        var OutPut = new StringBuilder();
        OutPut.Append(inputDSL.sbstr(0, Index));
        var RemainingString = inputDSL.sbstr(Index);
        var Index2 = RemainingString.IndexOf("}");
        OutPut.Append(RemainingString.sbstr(Index2));
        return OutPut.ToString().Replace("=>},","=>").Replace(",}.","." ).Replace("},},::", "},::");
    }

    public string getRawDSLForRole(string inputDSL, string RoleName){
        var index = prepare(inputDSL).IndexOf(string.Format(@"::{0}", RoleName))+3+RoleName.Length;
        var str = prepare(inputDSL).sbstr(index);
        var index2 = str.IndexOf("},::");
        if (index2<0)
            index2  = str.IndexOf("}.");
        return str.sbstr(0,index2);
    }
}

