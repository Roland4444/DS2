using DS2_Processor;
public class PSAConnector: DSLProcessor{
    public string  login="";
    public string pass="";
    public string urldb ="";
    public int delay = 3600;
    public PSAConnector(){
        parser = new ParseDSL();
    }

    public override object render(string DSL) {
        parseRoles(DSL);
        loadRoles(parseRoles(DSL));
        foreach(KeyValuePair<Role, Action > entry in mapper)
        {
           Action A = entry.Value;
           A();
        }     
        return "OK";
    }
    

    public  List<Role> parseRoles(string DSL) {
        return parser.parseRoles(DSL);
    }

    public void loadRoles( List<Role> D){
      foreach(Role entry in D)
        {
           appendRole(entry);
        }     
    }

    Action A = delegate() {
            Console.WriteLine("CH3COOH");
             
    };

    public void  appendRole(Role R){    
        switch (R.Name){
            case "test"     : mapper.Add(R, A );  break;
            case "enabled"  : mapper.Add(R, enable );  break;
            
        }
      
    }
}