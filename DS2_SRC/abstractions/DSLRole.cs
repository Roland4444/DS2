using System;
using System.Text;
using System.Collections.Generic;
namespace DS2_Abstractions{
[Serializable]
public class DSLRole{

    public string toString() 
    {
        var roles = new StringBuilder();
        foreach (Role _ in Roles)        
            roles.Append(string.Format(@""" ::{0},""", _));        
        string res =string.Format(@""" '{0}' =>{1}""", ObjectName, roles.ToString());
        return res.Substring(0, res.Length-1)+".";
    }
    public string ObjectName;
    public  List<Role> Roles;
    public DSLRole(string ObjectName, List<Role> Roles ){
        this.ObjectName = ObjectName;
        this.Roles = Roles;
    }

}

}