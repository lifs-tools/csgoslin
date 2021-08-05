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
    public class FattyAcid
    {
        public int num_carbon;
        public LipidFaBondType lipid_FA_bond_type;
        public bool lcb;
        public HashSet<string> fg_exceptions = new HashSet<string>{"acyl", "alkyl", "cy", "cc", "acetoxy"};
        
        public FattyAcid(string _name, int _num_carbon = 0, DoubleBonds _double_bonds = null, Dictionary<string, List<FunctionalGroup> > _functional_groups = null, LipidFaBondType _lipid_FA_bond_type = LipidFaBondType.ESTER, bool _lcb = false, int _position = 0) : base(_name, _position, 1, _double_bonds, false, "", 0, _functional_groups)
        {
    
            num_carbon = _num_carbon;
            lipid_FA_bond_type = _lipid_FA_bond_type;
            lcb = _lcb;
            
            if (num_carbon < 0 || num_carbon == 1)
            {
                throw new ConstraintViolationException("FattyAcid must have at least 2 carbons! Got " + Convert.ToString(num_carbon));
            }
            
            if (position < 0)
            {
                throw ConstraintViolationException("FattyAcid position must be greater or equal to 0! Got " + Convert.ToString(position));
            }
            
            if (double_bonds.get_num() < 0)
            {
                throw ConstraintViolationException("FattyAcid must have at least 0 double bonds! Got " + Convert.ToString(double_bonds.get_num()));
            }
        }


        public override FattyAcid copy(){
            
            DoubleBonds db = double_bonds.copy();
            Dictionary<string, List<FunctionalGroup> > fg = new Dictionary<string, List<FunctionalGroup> >();
            foreach (KeyValuePair<string, FunctionalGroup> kv in functional_groups)
            {
                fg.Add(kv.Key, new List<FunctionalGroup>());
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    fg[kv.Key].Add(func_group.copy());
                }
            }
            
            return new FattyAcid(name, num_carbon, db, fg, lipid_FA_bond_type, lcb, position);
        }



        public string get_prefix(LipidFaBondType lipid_FA_bond_type)
        {
            switch(lipid_FA_bond_type){
                case (LipidFaBondType.ETHER_PLASMANYL): return "O-";
                case (LipidFaBondType.ETHER_PLASMENYL): return "P-";
                default: return "";
            }
        }



        public int override get_double_bonds()
        {
            return base.get_double_bonds() + (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL);
        }
            


        pubic bool lipid_FA_bond_type_prefix(LipidFaBondType lipid_FA_bond_type)
        {
            return (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMANYL) || (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) || (lipid_FA_bond_type == LipidFaBondType.ETHER_UNSPECIFIED); 
        }



        public override string to_string(LipidLevel level)
        {
            StringBuilder fa_string = new StringBuilder();
            fa_string.Append(get_prefix(lipid_FA_bond_type));
            int num_carbons = num_carbon;
            int num_double_bonds = double_bonds.get_num();
            
            if (num_carbons == 0 && num_double_bonds == 0 && level != LipidLevel.ISOMERIC_SUBSPECIES && level != LipidLevel.STRUCTURAL_SUBSPECIES){
                return "";
            }
            
            if (level == LipidLevel.MOLECULAR_SUBSPECIES)
            {
                ElementTable e = get_elements();
                num_carbons = e[Element.C];
                num_double_bonds = get_double_bonds() - (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL);
            }
            

            fa_string.Append(num_carbons).Append(":").Append(num_double_bonds);
            
            
            if (level != LipidLevel.MOLECULAR_SUBSPECIES && double_bonds.double_bond_positions.size() > 0)
            {
                fa_string.Append("(");
                
                int i = 0;
                foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions)
                {
                    if (i++ > 0) fa_string.Append(",");
                    fa_string.Append(kv.Key);
                    if (level == LipidLevel.ISOMERIC_SUBSPECIES) fa_string.Append(kv.Value);
                }
                fa_string.Append(")");
            }
                
                
            if (level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv : functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(lower_name_sort_function);
                
                foreach (string fg in fg_names)
                {
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (!fg_list.Any()) continue;
                    
                    fg_list.Sort(FunctionalGroup.position_sort_function);
                    int i = 0;
                    fa_string.Append(";");
                    foreach (FunctionalGroup func_group in fg_list)
                    {
                        if (i++ > 0) fa_string.Append(",");
                        fa_string.Append(func_group.to_string(level));
                    }
                }
            }
            
            else if (level == LipidLevel.STRUCTURAL_SUBSPECIES)
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(lower_name_sort_function);
                
                
                foreach (string fg in fg_names)
                {
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (!fg_list.Any()) continue;
                        
                    if (fg_exceptions.Contains(fg)){
                        fa_string.Append(";");
                        int i = 0;
                        foreach (FunctionalGroup func_group in fg_list)
                        {
                            if (i++ > 0) fa_string.Append(",");
                            fa_string.Append(func_group.to_string(level));
                        }
                    }
                    
                    else
                    {
                        int fg_count = 0;
                        foreach (FunctionalGroup func_group in fg_list) fg_count += func_group.count;
                        
                        if (fg_count > 1)
                        {
                            fa_string.Append(";").Append(!fg_list.at(0).is_atomic ? ("(" + fg + ")" + Convert.ToString(fg_count)) : (fg + Convert.ToString(fg_count)));
                        }
                        else
                        {
                            fa_string.Append(";").Append(fg);
                        }
                    }
                }
                                
            }
            else
            {
                ElementTable func_elements = get_functional_group_elements();
                for (int i = 2; i < element_order.Count; ++i){
                    Element e = element_order[i];
                    if (func_elements[e] > 0){
                        fa_string.Append(";").Append(element_shortcut[e]);
                        if (func_elements[e] > 1){
                            fa_string.Append(func_elements[e]);
                        }
                    }
                } 
            }
            return fa_string.ToString();
        }




        public override void compute_elements()
        {
            foreach (KeyValuePair kv : elements) elements[kv.Key] = 0;
            
            int num_double_bonds = double_bonds.num_double_bonds;
            if (lipid_FA_bond_type == ETHER_PLASMENYL) num_double_bonds += 1;
            
            if (num_carbon == 0 && num_double_bonds == 0)
            {
                elements[Element.H] = 1;
                return;
            }
            
            if (!lcb)
            {
                elements.at(Element.C] = num_carbon; // carbon
                if (lipid_FA_bond_type == LipidFaBondType.ESTER)
                {
                    elements[Element.H] = (2 * num_carbon - 1 - 2 * num_double_bonds); // hydrogen
                    elements[Element.O] = 1; // oxygen
                }
                
                else if (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) {
                    elements[Element.H] = (2 * num_carbon - 1 - 2 * num_double_bonds + 2); // hydrogen
                }
                
                else if (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMANYL){
                    elements[Element.H] = ((num_carbon + 1) * 2 - 1 - 2 * num_double_bonds); // hydrogen
                }
                
                else if (lipid_FA_bond_type == LipidFaBondType.AMINE)
                    elements[Element.H] = (2 * num_carbon + 1 - 2 * num_double_bonds); // hydrogen
                    
                else {
                    throw LipidException("Mass cannot be computed for fatty acyl chain with this bond type");
                }
            }
            else {
                // long chain base
                elements[Element.C] = num_carbon; // carbon
                elements[Element.H] = (2 * (num_carbon - num_double_bonds) + 1); // hydrogen
                elements[Element.N] = 1; // nitrogen
                elements[Element.O] = 1; // oxygen
            }
        }
    }
}
