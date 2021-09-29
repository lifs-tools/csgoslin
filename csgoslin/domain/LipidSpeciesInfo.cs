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
    using ClassMap = System.Collections.Generic.Dictionary<LipidClass, LipidClassMeta>;
    
    public class LipidSpeciesInfo : FattyAcid
    {
        public LipidLevel level;
        public int num_ethers;
        public int num_specified_fa;
        public int total_fa;
        public LipidFaBondType extended_class;
        public static readonly string[] ether_prefix = {"", "O-", "dO-", "tO-", "eO-"};
        
        public LipidSpeciesInfo (LipidClass lipid_class) : base("info")
        {
            level = LipidLevel.NO_LEVEL;
            num_ethers = 0;
            num_specified_fa = 0;
            extended_class = LipidFaBondType.ESTER;
            ClassMap lipid_classes = LipidClasses.lipid_classes;
            total_fa = lipid_classes.ContainsKey(lipid_class) ? lipid_classes[lipid_class].max_num_fa : 0;
        }


        public override ElementTable get_elements()
        {
            ElementTable elements = base.get_elements();
            elements[Element.O] -= (num_ethers == 0) ? 1 : 0;
            elements[Element.H] += num_ethers == 0 ? 1 : -1;
            
            return elements;
        }


        public void add(FattyAcid _fa)
        {
            if (_fa.lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMENYL || _fa.lipid_FA_bond_type == LipidFaBondType.ETHER_PLASMANYL)
            {
                num_ethers += 1;
                lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
                extended_class = _fa.lipid_FA_bond_type;
            }
            else if (_fa.lipid_FA_bond_type == LipidFaBondType.LCB_EXCEPTION | _fa.lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR)
            {
                lipid_FA_bond_type = _fa.lipid_FA_bond_type;
            }
                    
            else
            {
                num_specified_fa += 1;
            }
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in  _fa.functional_groups)
            {
                if (!functional_groups.ContainsKey(kv.Key)) functional_groups.Add(kv.Key, new List<FunctionalGroup>());
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    functional_groups[kv.Key].Add(func_group.copy());
                }
            }
                
            ElementTable e = _fa.get_elements();
            num_carbon += e[Element.C];
            double_bonds.num_double_bonds += _fa.get_double_bonds();
        }


        public string to_string()
        {
            StringBuilder info_string = new StringBuilder();
            info_string.Append(ether_prefix[num_ethers]);
            info_string.Append(num_carbon).Append(":").Append(double_bonds.get_num());
            
            
            ElementTable elements = get_functional_group_elements();
            for (int i = 2; i < Elements.element_order.Count; ++i)
            {
                Element e = Elements.element_order[i];
                if (elements[e] > 0)
                {
                    info_string.Append(";").Append(Elements.element_shortcut[e]);
                    if (elements[e] > 1)
                    {
                        info_string.Append(elements[e]);
                    }
                }
            }
            
            return info_string.ToString();
        }
    }

}
