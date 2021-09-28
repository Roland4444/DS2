using System;
using System.Text;
using System.Collections.Generic;
namespace DS2_Abstractions{
public class DSLRole{
    public string ObjectName;
    public  List<Role> Roles;
    public DSLRole(string ObjectName, List<Role> Roles ){
        this.ObjectName = ObjectName;
        this.Roles = Roles;
    }
    public string toString() {
        var roles = new StringBuilder();
        Roles.ForEach(delegate(Role it)
        {
            roles.Append($"::{it},");
        });
        var res = $"'{this.ObjectName}' =>{roles.ToString()}";
        return res.Substring(0, res.Length-1)+".";
    }
}

}