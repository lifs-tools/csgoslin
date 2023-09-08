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
        public string sum_formula;
        
        public LipidAdduct()
        {
            lipid = null;
            adduct = null;
            sum_formula = "";
        }
        
        public LipidAdduct(LipidAdduct la)
        {
            if (la == null){
                lipid = null;
                adduct = null;
                sum_formula = "";
                return;
            }
            if (la.lipid != null && la.lipid.info != null && la.lipid.info.level >= LipidLevel.SPECIES)
            {
                List<FattyAcid> fa_list = new List<FattyAcid>();
                foreach (FattyAcid fa in la.lipid.fa_list){
                    FattyAcid fa_copy = (FattyAcid)fa.copy();
                    fa_copy.position = 0;
                    fa_list.Add(fa_copy);
                }
                Headgroup headgroup = new Headgroup(la.lipid.headgroup);
                
                switch (la.lipid.info.level)
                {
                    case LipidLevel.COMPLETE_STRUCTURE: lipid = new LipidCompleteStructure(headgroup, fa_list); break;
                    case LipidLevel.FULL_STRUCTURE: lipid = new LipidFullStructure(headgroup, fa_list); break;
                    case LipidLevel.STRUCTURE_DEFINED: lipid = new LipidStructureDefined(headgroup, fa_list); break;
                    case LipidLevel.SN_POSITION: lipid = new LipidSnPosition(headgroup, fa_list); break;
                    case LipidLevel.MOLECULAR_SPECIES: lipid = new LipidMolecularSpecies(headgroup, fa_list); break;
                    case LipidLevel.SPECIES: lipid = new LipidSpecies(headgroup, fa_list); break;
                    default: break;
                }
            }
            else
            {
                lipid = null;
            }
            adduct = (la.adduct != null) ? new Adduct(la.adduct) : null;
            sum_formula = la.sum_formula;
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
        
        public LipidLevel get_lipid_level()
        {
            return (lipid != null) ? lipid.get_lipid_level() : LipidLevel.NO_LEVEL;
        }


            
        public string get_extended_class()
        {
            return (lipid != null) ? lipid.get_extended_class() : "";
        }
        
        
        public void sort_fatty_acyl_chains(){
            if (lipid != null) lipid.sort_fatty_acyl_chains();
        }

        
        public bool is_lyso()
        {
            if (lipid == null) return false;
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Lyso") : false;
        }

        
        public bool is_cardio_lipin()
        {
            if (lipid == null) return false;
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Cardio") : false;
        }

        
        public bool contains_sugar()
        {
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Sugar") : false;
        }

        
        public bool contains_ester()
        {
            if (lipid == null) return false;
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("Ester") : false;
        }

        
        public bool is_sp_exception()
        {
            if (lipid == null) return false;
            return LipidClasses.lipid_classes.ContainsKey(lipid.headgroup.lipid_class) ? LipidClasses.lipid_classes[lipid.headgroup.lipid_class].special_cases.Contains("SP_Exception") : false;
        }



        public double get_mass()
        {
            ElementTable elements = get_elements();
            int charge = (adduct != null) ? adduct.get_charge() : 0;
            double mass = StringFunctions.get_mass(elements);
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
