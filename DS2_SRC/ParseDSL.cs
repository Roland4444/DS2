using System;
using DS2_Abstractions;
using System.Collections.Generic;
using System.Text;
using DS2_Abstractions.BNF;
public class ParseDSL{
   
    public Checker checker =  new Checker();
    public  DSLRole getDSLRulesfromString(string input) {
        var objectName = input.Substring(input.IndexOf("'")+1, input.LastIndexOf("'"));
        Console.WriteLine($"Loading rules for object <{objectName}>");
        return new DSLRole(objectName, parseRoles(input));
    }
    public Role parseRole(string input) {
        if (input.IndexOf("{") == -1) return null;
        var rolename = input.Substring(input.IndexOf("::")+2, input.IndexOf("{"));
        string params___ = "";
        if (input.IndexOf("{")<input.IndexOf("}")-2)
            params___ = input.Substring(input.IndexOf("{")+1, input.IndexOf("}")); 
        else params___="";
        if ((rolename.Length == 0) || (rolename ==null)) return null;
        return new Role(rolename, params___, this);
    }
    public List<Role> parseRoles(string input__) {
        var input = prepare(input__);
        List<Role> result  = new List<Role>();
        var initialString = input;
        var role = parseRole(initialString);
        while (role != null){
            result.Add(role);
            initialString = initialString.Substring(initialString.IndexOf("}")+1);
            role  = parseRole(initialString);
        };
        return result;
    }

    public string ToSequence(string input__){
        var input = prepare(input__);
        switch (getType(input)){            
            case DS2_Abstractions.BNF.Atom.Tupple: return input.Substring(1, input.Length-1);
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
        var p = getnumberopencolon(input);
        if (p>0)
            return input.Substring(0, p);
        return input;
    }

    public string Tail(string input){
        var p =getnumberopencolon(input);
        if (p>0)
            return input.Substring(p+1, input.Length);
        return "";
    }

    public int getnumberopencolon(string input){
        Console.WriteLine(input);
        bool mustret = false;
        int retvalue=0;
        var colonbuf = loadcolons(input);
        if (colonbuf.Capacity<=0)
            return -1;
        colonbuf.ForEach(delegate(int a)
        {
            var index = a;
            var closetupple = 0;
            var opentupple = 0;
            while (index>=0){
                if (input[index]==']'){
                    closetupple++;
                }
                if (input[index]=='['){
                    opentupple++;
                }
                index--;
            }
            if (opentupple==closetupple){
                mustret=true;
                retvalue = a;            
        }
        });
        if (mustret)
            return retvalue;
        return -1;
    }
    public List<int> loadcolons(string input) {
        var colonbuff = new List<int>();
        for (int i=0;i<=input.Length-1; i++)
            if (input[i]==',')
                colonbuff.Add(i);
        return colonbuff;
    }
    public bool opencolon(string input){
        Console.WriteLine(input);
        bool mustret =false;
        bool retvalue=false;
        var colonbuf = loadcolons(input);
        if (colonbuf.Capacity<=0)
            return false;
        colonbuf.ForEach(delegate(int a)
        {    
            var index = a;
            var closetupple = 0;
            var opentupple = 0;
            while (index>=0){
                if (input[index]==']'){
                    closetupple++;
                }
                if (input[index]=='['){
                    opentupple++;
                }
                index--;
            }
            if (opentupple==closetupple){
                 mustret = true;
                 retvalue= true;
            }
                
        });
        if (mustret)
            return retvalue;

        return false;
    }


    public List<String> getList(string input) {
        var lst = new List<String>();
        var head = Head(input);
        var tail = Tail(input);
        while ((head!="") ){
            lst.Add(head);
            head = Head(tail);
            tail = Tail(tail);
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
        return prepare(input.Substring(index+1, input.Length));
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
        var key = input___.Substring(0, index).Replace(" ","");
        return key;
    }
}

