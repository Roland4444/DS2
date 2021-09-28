using System.Collections.Generic;
public class Checker  {
    public Dictionary<char, bool> dictionary = new Dictionary<char, bool>();

    public Checker(){
        initDict();
    }

    public bool isnumber(string input){
        if (input.Length<1)
            return false;
        for (int i=0; i<input.Length;i++){
            if (!dictionary.ContainsKey(input[i])) 
                return false;
            if (dictionary[input[i]] != true)
                return false;
        }
        return true;
    }


    public void initDict(){
        this.dictionary.Add('.', true);
        this.dictionary.Add('0', true);
        this.dictionary.Add('1', true);
        this.dictionary.Add('2', true);
        this.dictionary.Add('3', true);
        this.dictionary.Add('4', true);
        this.dictionary.Add('5', true);
        this.dictionary.Add('6', true);
        this.dictionary.Add('7', true);
        this.dictionary.Add('8', true);
        this.dictionary.Add('9', true);
    }
}
