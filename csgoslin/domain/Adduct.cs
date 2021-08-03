/*
MIT License

Copyright (c) the authors (listed in global LICENSE file)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/



using System;
using System.Text;
using System.Collections.Generic;

namespace csgoslin
{
    public class Adduct
    {
        public string sum_formula;
        public string adduct_string;
        public int charge;
        public int sign;
        
        public Adduct(string _sum_formula, string _adduct_string, int _charge, int _sign)
        {
            sum_formula = _sum_formula;
            adduct_string = _adduct_string;
            charge = _charge;
            set_charge_sign(_sign);
            
        }


        public void set_charge_sign(int sign)
        {
            if (-1 <= sign && sign <= 1){
                charge_sign = sign;
            }
                
            else {
                throw new IllegalArgumentException("Sign can only be -1, 0, or 1");
            }
        }
                
        public string get_lipid_string()
        {
            if (charge == 0)
            {
                return "[M]";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(
            sb.Append("[M").Append(sum_formula).Append(adduct_string).Append("]").Append(charge).Append().Append(((charge_sign > 0) ? "+" : "-"));
            
            return sb.ToString();
        }

        public ElementTable get_elements()
        {
            ElementTable elements = create_empty_table();
            try{
                
                string adduct_name = adduct_string.Substring(1);
                ElementTable adduct_elements = SumFormulaParser.get_instance().parse(adduct_name);
                foreach (KeyValuePair<Element, int> kvp in adduct_elements)
                {
                    elements[e.Key] += e.Value;
                }
                
            }
            catch (Exception e)
            {
                return elements;
            }
            
            if (adduct_string.Length() > 0 && adduct_string[0] == '-')
            {
                foreach (KeyValuePair<Element, int> kvp in element_order)
                {
                    elements[e] *= -1;
                }
            }
            
            return elements;
        }



        public int get_charge()
        {
            return charge * charge_sign;
        }
}
