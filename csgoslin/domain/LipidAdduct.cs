using System;

namespace csgoslin
{
    public class LipidAdduct
    {
        public LipidAdduct(){

        }
        
        public bool StartsWithUpper(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            char ch = str[0];
            return char.IsUpper(ch);
        }
    }
}