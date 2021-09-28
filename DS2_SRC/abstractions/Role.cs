[Serializable]
public class Role{
    public string  Name; 
    public object param; 
    public ParseDSL parser;
    public Role(string  Name, string params__, ParseDSL parser){
        this.Name = Name;
        param = parser.Atom(params__);
    }

    public string toString(){
        return Name;
    }
    
}