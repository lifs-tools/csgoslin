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
    using static LevelFunctions;
    
    public class FattyAcid : FunctionalGroup
    {
        public int num_carbon;
        public LipidFaBondType lipid_FA_bond_type;
        public HashSet<string> fg_exceptions = new HashSet<string>{"acyl", "alkyl", "cy", "cc", "acetoxy"};
        
        public FattyAcid(string _name, int _num_carbon = 0, DoubleBonds _double_bonds = null, Dictionary<string, List<FunctionalGroup> > _functional_groups = null, LipidFaBondType _lipid_FA_bond_type = LipidFaBondType.ESTER, int _position = 0) : base(_name, _position, 1, _double_bonds, false, "", null, _functional_groups)
        {
    
            num_carbon = _num_carbon;
            lipid_FA_bond_type = _lipid_FA_bond_type;
            
            if (lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR)
            {
                functional_groups.Add("[X]", new List<FunctionalGroup>(){KnownFunctionalGroups.get_functional_group("X")});
            }
            
            if (num_carbon < 0 || num_carbon == 1)
            {
                throw new ConstraintViolationException("FattyAcid must have at least 2 carbons! Got " + Convert.ToString(num_carbon));
            }
            
            if (position < 0)
            {
                throw new ConstraintViolationException("FattyAcid position must be greater or equal to 0! Got " + Convert.ToString(position));
            }
            
            if (double_bonds.get_num() < 0)
            {
                throw new ConstraintViolationException("FattyAcid must have at least 0 double bonds! Got " + Convert.ToString(double_bonds.get_num()));
            }
        }


        public override FunctionalGroup copy()
        {
            DoubleBonds db = double_bonds.copy();
            Dictionary<string, List<FunctionalGroup> > fg = new Dictionary<string, List<FunctionalGroup> >();
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                fg.Add(kv.Key, new List<FunctionalGroup>());
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    fg[kv.Key].Add(func_group.copy());
                }
            }
            
            return new FattyAcid(name, num_carbon, db, fg, lipid_FA_bond_type, position);
        }
        
        
        
        public void set_type(LipidFaBondType _lipid_FA_bond_type)
        {
            lipid_FA_bond_type = _lipid_FA_bond_type;
            if (lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR && !functional_groups.ContainsKey("[X]"))
            {
                functional_groups.Add("[X]", new List<FunctionalGroup>(){KnownFunctionalGroups.get_functional_group("X")});
            }
                
            else if (functional_groups.ContainsKey("[X]"))
            {
                functional_groups.Remove("[X]");
            }
                
            name = (lipid_FA_bond_type != LipidFaBondType.LCB_EXCEPTION && lipid_FA_bond_type != LipidFaBondType.LCB_REGULAR) ? "FA" : "LCB";
        }



        public string get_prefix(LipidFaBondType lipid_FA_bond_type)
        {
            switch(lipid_FA_bond_type){
                case (LipidFaBondType.ETHER_PLASMANYL): return "O-";
                case (LipidFaBondType.ETHER_PLASMENYL): return "P-";
                default: return "";
            }
        }



        public override int get_double_bonds()
        {
            return base.get_double_bonds() + ((lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) ? 1 : 0);
        }
            


        public bool lipid_FA_bond_type_prefix(LipidFaBondType lipid_FA_bond_type)
        {
            return (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMANYL) || (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) || (lipid_FA_bond_type == LipidFaBondType.ETHER_UNSPECIFIED); 
        }



        public override string to_string(LipidLevel level)
        {
            StringBuilder fa_string = new StringBuilder();
            fa_string.Append(get_prefix(lipid_FA_bond_type));
            int num_carbons = num_carbon;
            int num_double_bonds = double_bonds.get_num();
            
            if (num_carbons == 0 && num_double_bonds == 0 && !is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE | LipidLevel.STRUCTURE_DEFINED | LipidLevel.SN_POSITION))
            {
                return "";
            }
            
            if (is_level(level, LipidLevel.SN_POSITION | LipidLevel.MOLECULAR_SPECIES))
            {
                ElementTable e = get_elements();
                num_carbons = e[Element.C];
                num_double_bonds = get_double_bonds() - ((lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) ? 1 : 0);
            }
            

            fa_string.Append(num_carbons).Append(":").Append(num_double_bonds);
            
            
            if (!is_level(level, LipidLevel.SN_POSITION | LipidLevel.MOLECULAR_SPECIES) && double_bonds.double_bond_positions.Count > 0)
            {
                fa_string.Append("(");
                
                int i = 0;
                List<int> sorted_db = new List<int>();
                foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions) sorted_db.Add(kv.Key);
                sorted_db.Sort();
                foreach (int db_pos in sorted_db)
                {
                    if (i++ > 0) fa_string.Append(",");
                    fa_string.Append(db_pos);
                    if (is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE)) fa_string.Append(double_bonds.double_bond_positions[db_pos]);
                }
                fa_string.Append(")");
            }
                
                
            if (is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE))
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(delegate(string x, string y)
                {
                    return x.ToLower().CompareTo(y.ToLower());
                });
                
                foreach (string fg in fg_names)
                {
                    if (fg.Equals("[X]")) continue;
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (fg_list.Count == 0) continue;
                    
                    
                    fg_list.Sort(delegate(FunctionalGroup x, FunctionalGroup y)
                    {
                        return x.position - y.position;
                    });
                    int i = 0;
                    fa_string.Append(";");
                    foreach (FunctionalGroup func_group in fg_list)
                    {
                        if (i++ > 0) fa_string.Append(",");
                        fa_string.Append(func_group.to_string(level));
                    }
                }
            }
            
            else if (level == LipidLevel.STRUCTURE_DEFINED)
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(delegate(string x, string y)
                {
                    return x.ToLower().CompareTo(y.ToLower());
                });
                
                
                foreach (string fg in fg_names)
                {
                    if (fg.Equals("[X]")) continue;
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (fg_list.Count == 0) continue;
                        
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
                            fa_string.Append(";").Append(!fg_list[0].is_atomic ? ("(" + fg + ")" + Convert.ToString(fg_count)) : (fg + Convert.ToString(fg_count)));
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
                for (int i = 2; i < Elements.element_order.Count; ++i){
                    Element e = Elements.element_order[i];
                    if (func_elements[e] > 0){
                        fa_string.Append(";").Append(Elements.element_shortcut[e]);
                        if (func_elements[e] > 1){
                            fa_string.Append(func_elements[e]);
                        }
                    }
                } 
            }
            return fa_string.ToString();
        }


            
        public override ElementTable get_functional_group_elements(){
            ElementTable elements = base.get_functional_group_elements();
            
            // subtract the invisible [X] functional group for regular LCBs
            if (lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR && functional_groups.ContainsKey("O"))
            {
                elements[Element.O] -= 1;
            }
                
            return elements;
        }



        public override void compute_elements()
        {
            foreach (Element e in Elements.element_order) elements[e] = 0;
            
            int num_double_bonds = double_bonds.num_double_bonds;
            if (lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL) num_double_bonds += 1;
            
            if (num_carbon == 0 && num_double_bonds == 0)
            {
                elements[Element.H] = 1;
                return;
            }
            
            if (lipid_FA_bond_type != LipidFaBondType.LCB_EXCEPTION && lipid_FA_bond_type != LipidFaBondType.LCB_REGULAR)
            {
                elements[Element.C] = num_carbon; // carbon
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
                    throw new LipidException("Mass cannot be computed for fatty acyl chain with this bond type");
                }
            }
            else {
                // long chain base
                elements[Element.C] = num_carbon; // carbon
                elements[Element.H] = (2 * (num_carbon - num_double_bonds) + 1); // hydrogen
                elements[Element.N] = 1; // nitrogen
                //elements[Element.O] = 1; // oxygen
            }
        }
    }
    
    
    public class AcylAlkylGroup : FunctionalGroup
    {
        public bool alkyl;
        public bool N_bond;
        
        
        public AcylAlkylGroup(FattyAcid _fa, int _position = -1, int _count = 1, bool _alkyl = false, bool _N_bond = false) : base("O", _position, _count){
            alkyl = _alkyl;
            if (_fa != null){
                functional_groups.Add(alkyl ? "alkyl" : "acyl", new List<FunctionalGroup> {_fa});
            }
            double_bonds.num_double_bonds = alkyl ? 0 : 1;
            set_N_bond_type(_N_bond);
            
        }
        
        
        public override FunctionalGroup copy()
        {
            return new AcylAlkylGroup((FattyAcid)functional_groups[alkyl ? "alkyl" : "acyl"][0].copy(), position, count, alkyl, N_bond);
        }
        
        
        public void set_N_bond_type(bool _N_bond)
        {
            N_bond = _N_bond;
                
            if (N_bond){
                elements[Element.H] = alkyl ? 2 : 0;
                elements[Element.O] = alkyl ? -1 : 0;
                elements[Element.N] = 1;
            }
            else {
                elements[Element.H] = alkyl ? 1 : -1;
                elements[Element.O] = alkyl ? 0 : 1;
            }
        }
        
        
        public override string to_string(LipidLevel level){
            StringBuilder acyl_alkyl_string = new StringBuilder();
            if (level == LipidLevel.FULL_STRUCTURE) acyl_alkyl_string.Append(position);
            acyl_alkyl_string.Append(N_bond ? "N" : "O").Append("(");
            if (!alkyl) acyl_alkyl_string.Append("FA ");
            acyl_alkyl_string.Append(((FattyAcid)functional_groups[alkyl ? "alkyl" :"acyl"][0]).to_string(level)).Append(")");
            
            return acyl_alkyl_string.ToString();
        }
    }
        

        
    public class CarbonChain : FunctionalGroup
    {
        public CarbonChain(FattyAcid _fa, int _position = -1, int _count = 1) : base("cc", _position, _count)
        {
            if (_fa != null)
            {
                functional_groups.Add("cc", new List<FunctionalGroup>{_fa});
            }
            
            elements[Element.H] = 1;
            elements[Element.O] = -1;
        }
        
        
        public override FunctionalGroup copy()
        {
            return new CarbonChain((FattyAcid)functional_groups["cc"][0].copy(), position, count);
        }
        
        
        public override string to_string(LipidLevel level)
        {
            return (is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE) ? Convert.ToString(position) : "") + "(" + ((FattyAcid)functional_groups["cc"][0]).to_string(level) + ")";
        }
    }
}
