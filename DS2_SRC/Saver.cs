
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
public class Saver{
    public static byte[] savedToBLOB(object input)
    {
        if(input == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, input);
            return ms.ToArray();
        }
    }

    public static void write(byte[] data, string filename) 
    {
        File.WriteAllBytes (filename, data);
    }

    public static byte[] readBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    public object restored(byte[] input){
        using (MemoryStream ms = new MemoryStream(input))
        {
            IFormatter br = new BinaryFormatter();
            return br.Deserialize(ms);
        }
    }

}