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
    
    public class LipidAdduct
    {
        public LipidSpecies lipid;
        public Adduct adduct;
        
        public LipidAdduct()
        {
            lipid = null;
            adduct = null;
        }
            
            
        public string get_lipid_string(LipidLevel level = LipidLevel.NO_LEVEL)
        {
            StringBuilder sb = new StringBuilder();
            if (lipid != null) sb.Append(lipid.get_lipid_string(level));
            else return "";
            
            
            switch (level)
            {
                case LipidLevel.CLASS:
                case LipidLevel.CATEGORY:
                    break;
                    
                default:
                    if (adduct != null) sb.Append(adduct.get_lipid_string());
                    break;
            }
            
            return sb.ToString();
        }


        public string get_class_name()
        {
            return (lipid != null) ? lipid.headgroup.get_class_name() : "";
        }

        
        public bool is_lyso()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Lyso") : false;
        }

        
        public bool is_cardio_lipin()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Cardio") : false;
        }

        
        public bool contains_sugar()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Sugar") : false;
        }

        
        public bool contains_ester()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Ester") : false;
        }

        
        public bool is_sp_exception()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("SP_Exception") : false;
        }


        public LipidLevel get_lipid_level()
        {
            return lipid.get_lipid_level();
        }


            
        public string get_extended_class()
        {
            return (lipid != null) ? lipid.get_extended_class() : "";
        }



        public double get_mass()
        {
            ElementTable elements = get_elements();
            int charge = 0;
            double mass = 0;
                
            if (adduct != null)
            {
                charge = adduct.get_charge();
            }
            
            foreach( KeyValuePair<Element, int> kvp in elements)
            {
                mass += Elements.element_masses[kvp.Key] * kvp.Value;
            }
            
            if (charge != 0) mass = (mass - charge * Elements.ELECTRON_REST_MASS) / Math.Abs(charge);
            
            return mass;
        }


        public ElementTable get_elements()
        {
            ElementTable elements = StringFunctions.create_empty_table();
            
            if (lipid != null)
            {
                ElementTable lipid_elements = lipid.get_elements();
                foreach( KeyValuePair<Element, int> kvp in lipid_elements)
                {
                    elements[kvp.Key] += kvp.Value;
                }
            }
                    
            if (adduct != null){
                ElementTable adduct_elements = adduct.get_elements();
                foreach( KeyValuePair<Element, int> kvp in adduct_elements)
                {
                    elements[kvp.Key] += kvp.Value;
                }
            }
            return elements;
        }


        public string get_sum_formula()
        {
            return StringFunctions.compute_sum_formula(get_elements());
        }
    }
}
