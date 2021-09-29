namespace DS2_Processor{
abstract public class DSLProcessor {
    public delegate void  RoleHandler();
    public const string TRUE_ATOM = "true";
    public const string FALSE_ATOM = "false";
    public const string OK = "OK";
    public const string EMPTY_ATOM = "";
    public string enabled = "false";

    public DSLProcessor(){
        mapper = new Dictionary<Role, Action>();
        enable = delegate()
        {
        foreach(KeyValuePair<Role, Action > entry in mapper)
        {
            Role R = entry.Key;
            if (R.Name=="enabled") 
                enabled = (string)R.param ;            
        }     
        }; 
    }
    public ParseDSL parser;

    public  Dictionary<Role, Action> mapper;

    abstract public object render(string DSL);

    // abstract public List<Role> parseRoles(string DSL);

    public string outtemplate= "";

    public Action enable;
}
}