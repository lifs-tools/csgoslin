﻿/*
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
    
    public class LipidMolecularSpecies : LipidSpecies
    {
        public LipidMolecularSpecies (Headgroup _headgroup, List<FattyAcid> _fa = null) : base(_headgroup, _fa)
        {
            info.level = LipidLevel.MOLECULAR_SPECIES;
            foreach (FattyAcid fatty_acid in _fa)
            {
                if (fa.ContainsKey(fatty_acid.name))
                {
                    throw new ConstraintViolationException("FA names must be unique! FA with name " + fatty_acid.name + " was already added!");
                }
                fa.Add(fatty_acid.name, fatty_acid);
                fa_list.Add(fatty_acid);
            }
                
                    
            // add 0:0 dummys
            for (int i = _fa.Count; i < info.total_fa; ++i)
            {
                FattyAcid fatty_acid = new FattyAcid("FA" + Convert.ToString(i + 1));
                fatty_acid.position = -1;
                fatty_acid.unresolved_hidden_fa = (1 < _fa.Count && _fa.Count < info.poss_fa);
                info.add(fatty_acid);
                fa.Add(fatty_acid.name, fatty_acid);
                fa_list.Add(fatty_acid);
            }
        }


        public string build_lipid_subspecies_name(LipidLevel level = LipidLevel.NO_LEVEL)
        {
            if (level == LipidLevel.NO_LEVEL) level = LipidLevel.MOLECULAR_SPECIES;
            
            string fa_separator = (level != LipidLevel.MOLECULAR_SPECIES || headgroup.lipid_category == LipidCategory.SP) ? "/" : "_";
            StringBuilder lipid_name = new StringBuilder();
            lipid_name.Append(headgroup.get_lipid_string(level));
            

            string fa_headgroup_separator = (headgroup.lipid_category != LipidCategory.ST) ? " " : "/";
            
            switch (level)
            {
                case LipidLevel.COMPLETE_STRUCTURE:
                case LipidLevel.FULL_STRUCTURE:
                case LipidLevel.STRUCTURE_DEFINED:
                case LipidLevel.SN_POSITION:
                    if (fa_list.Count > 0)
                    {
                        lipid_name.Append(fa_headgroup_separator);
                        int i = 0;
            
                        foreach (FattyAcid fatty_acid in fa_list)
                        {
                            if (i++ > 0) lipid_name.Append(fa_separator);
                            lipid_name.Append(fatty_acid.to_string(level));
                        }
                    }
                    break;
                    
                default:
                    bool go_on = false;
                    foreach (FattyAcid fatty_acid in fa_list)
                    {
                        if (fatty_acid.num_carbon > 0)
                        {
                            go_on = true;
                            break;
                        }
                    }
                    
                    if (go_on){
                        lipid_name.Append(fa_headgroup_separator);
                        int i = 0;
                        foreach (FattyAcid fatty_acid in fa_list)
                        {
                            if (fatty_acid.num_carbon > 0)
                            {
                                if (i++ > 0) lipid_name.Append(fa_separator);
                                lipid_name.Append(fatty_acid.to_string(level));
                            }
                        }
                    }
                    break;
            }
            return lipid_name.ToString();
        }


        public override LipidLevel get_lipid_level(){
            return LipidLevel.MOLECULAR_SPECIES;
        }



        public override ElementTable get_elements(){
            ElementTable elements = StringFunctions.create_empty_table();
            
            ElementTable hg_elements = headgroup.get_elements();
            foreach (KeyValuePair<Element, int> kv in hg_elements) elements[kv.Key] += kv.Value;
            
            // add elements from all fatty acyl chains
            foreach (FattyAcid fatty_acid in fa_list)
            {
                ElementTable fa_elements = fatty_acid.get_elements();
                foreach (KeyValuePair<Element, int> kv in fa_elements) elements[kv.Key] += kv.Value;
            }
            
            return elements;
        }


        public override void sort_fatty_acyl_chains()
        {
            if (info.level > LipidLevel.MOLECULAR_SPECIES || fa_list.Count < 2) return;
            
            fa_list.Sort(delegate(FattyAcid fa1, FattyAcid fa2)
            {
                // treat empty fatty acids individually
                if (fa1 == null || fa1.num_carbon == 0) return -1;
                if (fa2 == null || fa2.num_carbon == 0) return 1;
                
                if (fa1.lipid_FA_bond_type != fa2.lipid_FA_bond_type) return fa1.lipid_FA_bond_type - fa2.lipid_FA_bond_type;
                if (fa1.num_carbon != fa2.num_carbon) return fa1.num_carbon - fa2.num_carbon;
                int db1 = fa1.double_bonds.get_num();
                int db2 = fa2.double_bonds.get_num();
                if (db1 != db2) return db1 - db2;
                ElementTable e1 = fa1.get_elements();
                ElementTable e2 = fa2.get_elements();
                double mass1 = StringFunctions.get_mass(e1);
                double mass2 = StringFunctions.get_mass(e2);
                return mass1 < mass2 ? -1 : 1;
            });
        }


        public override string get_lipid_string(LipidLevel level = LipidLevel.NO_LEVEL)
        {
            switch (level)
            {
                case LipidLevel.NO_LEVEL:
                case LipidLevel.MOLECULAR_SPECIES:
                    return build_lipid_subspecies_name(LipidLevel.MOLECULAR_SPECIES);
            
                case LipidLevel.CATEGORY:
                case LipidLevel.CLASS:
                case LipidLevel.SPECIES:
                    return base.get_lipid_string(level);
            
                default:
                    throw new IllegalArgumentException("LipidMolecularSpecies does not know how to create a lipid string for level " + Convert.ToString(level));
            }
        }
            
    }

}
