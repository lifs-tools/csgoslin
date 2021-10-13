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
    using ElementTable = System.Collections.Generic.Dictionary<Element, int>;
    
    public class Adduct
    {
        public string sum_formula;
        public string adduct_string;
        public int charge;
        public int charge_sign;
        
        public static readonly Dictionary<string, ElementTable> adducts = new Dictionary<string, ElementTable>{
            {"+H", new ElementTable(){{Element.H, 1}} },
            {"+2H", new ElementTable(){{Element.H, 2}} },
            {"+3H", new ElementTable(){{Element.H, 3}} },
            {"+4H", new ElementTable(){{Element.H, 4}} },
            {"-H", new ElementTable(){{Element.H, -1}} },
            {"-2H", new ElementTable(){{Element.H, -2}} },
            {"-3H", new ElementTable(){{Element.H, -3}} },
            {"-4H", new ElementTable(){{Element.H, -4}} },
            {"+H-H2O", new ElementTable(){{Element.H, -1}, {Element.O, -1}} },
            {"+NH4", new ElementTable(){{Element.N, 1}, {Element.H, 4}} },
            {"+Cl", new ElementTable(){{Element.Cl, 1}} },
            {"+HCOO", new ElementTable(){{Element.H, 1}, {Element.C, 1}, {Element.O, 2}} },
            {"+CH3COO", new ElementTable(){{Element.H, 3}, {Element.C, 2}, {Element.O, 2}} },
        };
        
        public static readonly Dictionary<string, int> adduct_charges = new Dictionary<string, int>{
            {"+H", 1},  {"+2H", 2}, {"+3H", 3}, {"+4H", 4},
            {"-H", -1}, {"-2H", -2}, {"-3H", -3}, {"-4H", -4},
            {"+H-H2O", 1}, {"+NH4", 1}, {"+Cl", -1}, {"+HCOO", -1}, {"+CH3COO", -1}
        };

        
        public Adduct(string _sum_formula, string _adduct_string, int _charge = 1, int _sign = 1)
        {
            sum_formula = _sum_formula;
            adduct_string = _adduct_string;
            charge = _charge;
            set_charge_sign(_sign);
            
        }


        public void set_charge_sign(int _sign)
        {
            if (-1 <= _sign && _sign <= 1){
                charge_sign = _sign;
            }
                
            else {
                throw new ConstraintViolationException("Sign can only be -1, 0, or 1");
            }
        }
                
        public string get_lipid_string()
        {
            if (charge == 0)
            {
                return "[M]";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("[M").Append(sum_formula).Append(adduct_string).Append("]").Append(charge).Append(((charge_sign > 0) ? "+" : "-"));
            
            return sb.ToString();
        }

        public ElementTable get_elements()
        {
            ElementTable elements = StringFunctions.create_empty_table();
            
            if (adduct_charges.ContainsKey(adduct_string))
            {
                if (adduct_charges[adduct_string] != get_charge())
                {
                    throw new ConstraintViolationException("Provided charge '" + Convert.ToString(get_charge()) + "' in contradiction to adduct '" + adduct_string + "' charge '" + Convert.ToString(adduct_charges[adduct_string]) + "'.");
                }
                foreach (KeyValuePair<Element, int> kv in adducts[adduct_string])
                {
                    elements[kv.Key] += kv.Value;
                }
            }
            else
            {
                throw new ConstraintViolationException("Adduct '" + adduct_string + "' is unknown.");
            }
            
            
            return elements;
        }



        public int get_charge()
        {
            return charge * charge_sign;
        }
    }
}
