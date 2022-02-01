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
        
    
    public class LipidBaseParserEventHandler : BaseParserEventHandler<LipidAdduct>
    {
        public LipidLevel level = LipidLevel.FULL_STRUCTURE;
        public string head_group = "";
        public FattyAcid lcb = null;
        public List<FattyAcid> fa_list = new List<FattyAcid>();
        public FattyAcid current_fa = null;
        public Adduct adduct = null;
        public List<HeadgroupDecorator> headgroup_decorators = new List<HeadgroupDecorator>();
        public bool use_head_group = false;
        
        public static HashSet<string> SP_EXCEPTION_CLASSES = new HashSet<string>{"Cer", "Ceramide", "Sphingosine", "So", "Sphinganine", "Sa", "SPH", "Sph", "LCB"};
        
    
        public LipidBaseParserEventHandler() : base()
        {
        }

        
        
        public void set_lipid_level(LipidLevel _level)
        {
            level = (LipidLevel)Math.Min((int)level, (int)_level);
        }
        
        
        
        public bool sp_regular_lcb()
        {
            return Headgroup.get_category(head_group) == LipidCategory.SP && (current_fa.lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR ||current_fa.lipid_FA_bond_type == LipidFaBondType.LCB_EXCEPTION) && !(SP_EXCEPTION_CLASSES.Contains(head_group) && headgroup_decorators.Count == 0);
        }
        
        public Headgroup prepare_headgroup_and_checks()
        {
            Headgroup headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
            if (use_head_group) return headgroup;
            
            int true_fa = 0;
            foreach (FattyAcid fa in fa_list)
            {
                true_fa += (fa.num_carbon > 0 || fa.double_bonds.get_num() > 0) ? 1 : 0;
            }
            int poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            
            // make lyso
            bool can_be_lyso = LipidClasses.lipid_classes.ContainsKey(Headgroup.get_class("L" + head_group)) ? LipidClasses.lipid_classes[Headgroup.get_class("L" + head_group)].special_cases.Contains("Lyso") : false;
            
            if (true_fa + 1 == poss_fa && level != LipidLevel.SPECIES && headgroup.lipid_category == LipidCategory.GP && can_be_lyso)
            {
                head_group = "L" + head_group;
                headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
                poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            }
            
            else if (true_fa + 2 == poss_fa && level != LipidLevel.SPECIES && headgroup.lipid_category == LipidCategory.GP && head_group.Equals("CL"))
            {
                head_group = "DL" + head_group;
                headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
                poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            }
            
            if (level == LipidLevel.SPECIES)
            {
                if (true_fa == 0 && poss_fa != 0)
                {
                    string hg_name = headgroup.headgroup;
                    throw new ConstraintViolationException("No fatty acyl information lipid class '" + hg_name + "' provided.");
                }
            }
                
            else if (true_fa != poss_fa && is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE | LipidLevel.STRUCTURE_DEFINED))
            {
                string hg_name = headgroup.headgroup;
                throw new ConstraintViolationException("Number of described fatty acyl chains (" + true_fa.ToString() + ") not allowed for lipid class '" + hg_name + "' (having " + poss_fa.ToString() + " fatty aycl chains).");
            }
            
            if (LipidClasses.lipid_classes[headgroup.lipid_class].special_cases.Contains("HC"))
            {
                fa_list[0].lipid_FA_bond_type = LipidFaBondType.AMINE;
            }
            
            
            int max_num_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].max_num_fa : 0;
            if (max_num_fa != fa_list.Count) level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.MOLECULAR_SPECIES);
            
            if (fa_list.Count > 0 && headgroup.sp_exception) fa_list[0].set_type(LipidFaBondType.LCB_EXCEPTION);
            
            return headgroup;
        }
        
        
        
        public LipidSpecies assemble_lipid(Headgroup headgroup)
        {
            LipidSpecies ls = null;
            switch (level)
            {
                case LipidLevel.COMPLETE_STRUCTURE: ls = new LipidCompleteStructure(headgroup, fa_list); break;
                case LipidLevel.FULL_STRUCTURE: ls = new LipidFullStructure(headgroup, fa_list); break;
                case LipidLevel.STRUCTURE_DEFINED: ls = new LipidStructureDefined(headgroup, fa_list); break;
                case LipidLevel.SN_POSITION: ls = new LipidSnPosition(headgroup, fa_list); break;
                case LipidLevel.MOLECULAR_SPECIES: ls = new LipidMolecularSpecies(headgroup, fa_list); break;
                case LipidLevel.SPECIES: ls = new LipidSpecies(headgroup, fa_list); break;
                default: break;
            }
            return ls;
        }
    }
}
