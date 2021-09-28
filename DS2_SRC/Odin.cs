using System;
namespace DS2_Abstractions
{
    public class Odin
    { 
        Func<string, string, string> marinafunc=(foo,bar) =>"Σ="+(Int32.Parse(foo)+Int32.Parse(bar)).ToString();
        Func<string, string, string> guest=(a, b)=>"";
        Func<string, string, Func<string, string, string>, string> result = (foo, bar, F) => $"<h1>{foo}</h1><br>h1>{bar}</h1><br>{F(foo, bar)}";
        public string callguest(){
            var foo = "3";
            var bar = "7";
            return (result(foo,bar, guest));
        }
        public string callmarina(){
            var foo = "3";
            var bar = "7";
            return (result(foo,bar, marinafunc));
        }
    }
}
