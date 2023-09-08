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
    
    public class LipidSpecies
    {
        public Headgroup headgroup;
        public LipidSpeciesInfo info;
        
        public Dictionary<string, FattyAcid> fa = new Dictionary<string, FattyAcid>();
        public List<FattyAcid> fa_list = new List<FattyAcid>();
        
        public LipidSpecies(Headgroup _headgroup, List<FattyAcid> _fa = null)
        {
            headgroup = _headgroup;
            
            info = new LipidSpeciesInfo(headgroup.lipid_class);
            info.level = LipidLevel.SPECIES;
            
            // add fatty acids
            if (_fa != null){
                foreach (FattyAcid fatty_acid in _fa)
                {
                    info.add(fatty_acid);
                    fa_list.Add(fatty_acid);
                }
            }
            
            /*
            foreach (HeadgroupDecorator decorator in headgroup.decorators)
            {
                if (decorator.name.Equals("decorator_alkyl") || decorator.name.Equals("decorator_acyl"))
                {
                    ElementTable e = decorator.get_elements();
                    info.num_carbon += e[Element.C];
                    info.double_bonds.num_double_bonds += decorator.get_double_bonds();
                }
            }
            */  
        }
        
        public virtual void sort_fatty_acyl_chains(){
            
        }


        public virtual LipidLevel get_lipid_level()
        {
            return LipidLevel.SPECIES;
        }


        public virtual string get_lipid_string(LipidLevel level = LipidLevel.NO_LEVEL)
        {
            switch (level)
            {
                    
                default:
                    throw new RuntimeException("LipidSpecies does not know how to create a lipid string for level " + Convert.ToString(level));
                    
                case LipidLevel.UNDEFINED_LEVEL:
                    throw new RuntimeException("LipidSpecies does not know how to create a lipid string for level " + Convert.ToString(level));
                    
                case LipidLevel.CLASS:
                case LipidLevel.CATEGORY:
                    return headgroup.get_lipid_string(level);
                    
                case LipidLevel.NO_LEVEL:
                case LipidLevel.SPECIES:
                    StringBuilder lipid_string = new StringBuilder();
                    lipid_string.Append(headgroup.get_lipid_string(level));
                    
                    if (info.elements[Element.C] > 0 || info.num_carbon > 0)
                    {
                        LipidSpeciesInfo lsi = (LipidSpeciesInfo)info.copy();
                        foreach (HeadgroupDecorator decorator in headgroup.decorators)
                        {
                            if (decorator.name.Equals("decorator_alkyl") || decorator.name.Equals("decorator_acyl"))
                            {
                                ElementTable e = decorator.get_elements();
                                lsi.num_carbon += e[Element.C];
                                lsi.double_bonds.num_double_bonds += decorator.get_double_bonds();
                            }
                        }
                        lipid_string.Append(headgroup.lipid_category != LipidCategory.ST ? " " : "/").Append(lsi.to_string());
                    }
                    return lipid_string.ToString();
            }
        }



        public string get_extended_class()
        {
            bool special_case = (info.num_carbon > 0) ? (headgroup.lipid_category == LipidCategory.GP) : false;
            string class_name = headgroup.get_class_name();
            if (special_case && (info.extended_class == LipidFaBondType.ETHER_PLASMANYL || info.extended_class == LipidFaBondType.ETHER_UNSPECIFIED))
            {
                return class_name + "-O";
            }
            
            else if (special_case && info.extended_class == LipidFaBondType.ETHER_PLASMENYL)
            {
                return class_name + "-P";
            }
            
            return class_name;
        }


        public List<FattyAcid> get_fa_list()
        {
            return fa_list;
        }



        public virtual ElementTable get_elements()
        {
            ElementTable elements = StringFunctions.create_empty_table();
            
            switch(info.level)
            {
                case LipidLevel.COMPLETE_STRUCTURE:
                case LipidLevel.FULL_STRUCTURE:
                case LipidLevel.STRUCTURE_DEFINED:
                case LipidLevel.SN_POSITION:
                case LipidLevel.MOLECULAR_SPECIES:
                case LipidLevel.SPECIES:
                    break;

                default:    
                    throw new LipidException("Element table cannot be computed for lipid level " + Convert.ToString(info.level));
            }
            
            if (headgroup.use_headgroup)
            {
                throw new LipidException("Element table cannot be computed for lipid level " + Convert.ToString(info.level));
            }
            
            
            ElementTable hg_elements = headgroup.get_elements();
            foreach (KeyValuePair<Element, int> kv in hg_elements) elements[kv.Key] += kv.Value;
            
            ElementTable info_elements = info.get_elements();
            foreach (KeyValuePair<Element, int> kv in info_elements) elements[kv.Key] += kv.Value;
            
            // since only one FA info is provided, we have to treat this single information as
            // if we would have the complete information about all possible FAs in that lipid
            LipidClassMeta meta = LipidClasses.lipid_classes[headgroup.lipid_class];
            
            int additional_fa = meta.possible_num_fa;
            int remaining_H = meta.max_num_fa - additional_fa;
            int hydrochain = meta.special_cases.Contains("HC") ? 1 : 0;
            
            elements[Element.O] -= -additional_fa + info.num_ethers + (headgroup.sp_exception ? 1 : 0) + hydrochain;
            elements[Element.H] += -additional_fa + remaining_H + 2 * info.num_ethers + 2 * hydrochain;
            
            return elements;
        }
    }

}
