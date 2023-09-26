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
        public LipidLevel level = LipidLevel.NO_LEVEL;
        public string head_group = "";
        public FattyAcid lcb = null;
        public List<FattyAcid> fa_list = new List<FattyAcid>();
        public FattyAcid current_fa = null;
        public List<HeadgroupDecorator> headgroup_decorators = new List<HeadgroupDecorator>();
        public bool use_head_group = false;
        public static HashSet<string> SP_EXCEPTION_CLASSES = new HashSet<string>{"Cer", "Ceramide", "Sphingosine", "So", "Sphinganine", "Sa", "SPH", "Sph", "LCB"};
        public Adduct adduct = null;
        
    
        public LipidBaseParserEventHandler() : base()
        {
            fa_list = new List<FattyAcid>();
            level = LipidLevel.FULL_STRUCTURE;
            head_group = "";
            lcb = null;
            current_fa = null;
            adduct = null;
            headgroup_decorators = new List<HeadgroupDecorator>();
            use_head_group = false;
        }

        
        
        public void set_lipid_level(LipidLevel _level)
        {
            level = (LipidLevel)Math.Min((int)level, (int)_level);
        }
        
        
        
        public bool sp_regular_lcb()
        {
            return Headgroup.get_category(head_group) == LipidCategory.SP && (current_fa.lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR ||current_fa.lipid_FA_bond_type == LipidFaBondType.LCB_EXCEPTION) && !(SP_EXCEPTION_CLASSES.Contains(head_group) && headgroup_decorators.Count == 0);
        }
        
        
        
        public bool check_full_structure(FunctionalGroup obj)
        {
            bool full = true;
            
            bool is_fa = obj.GetType() == typeof(FattyAcid);
            if (is_fa && ((FattyAcid)obj).num_carbon == 0) return true;
            if (is_fa && obj.double_bonds.num_double_bonds > 0 && obj.double_bonds.double_bond_positions.Count == 0) return false;
            if (is_fa && obj.double_bonds.double_bond_positions.Count != 0)
            {
                int sum = 0;
                foreach (KeyValuePair<int, string> kv in obj.double_bonds.double_bond_positions) sum += (kv.Value.Equals("E") || kv.Value.Equals("Z") || (kv.Value.Equals("") && kv.Key == ((FattyAcid)obj).num_carbon - 1)) ? 1 : 0;
                full &= (sum == obj.double_bonds.double_bond_positions.Count);
            }

            foreach (KeyValuePair<string, List<FunctionalGroup>> kv in obj.functional_groups)
            {
                foreach (FunctionalGroup fg in kv.Value)
                {
                    if (fg.name == "X") continue;
                    if (fg.position < 0) return false;
                    full &= check_full_structure(fg);
                }
            }
            
            return full;
        }
        
        
        
        public FattyAcid resolve_fa_synonym(string mediator_name)
        {
            switch(mediator_name)
            {
                case "Palmitic acid":
                    return new FattyAcid("FA", 16);
                    
                case "Linoleic acid":
                    return new FattyAcid("FA", 18, new DoubleBonds(2));
                    
                case "AA":
                    return new FattyAcid("FA", 20, new DoubleBonds(4));
                    
                case "ALA":
                    return new FattyAcid("FA", 18, new DoubleBonds(3));
                    
                case "EPA":
                    return new FattyAcid("FA", 20, new DoubleBonds(5));
                    
                case "DHA":
                    return new FattyAcid("FA", 22, new DoubleBonds(6));
                
                case "LTB4":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 5;
                        f2.position = 12;
                        return new FattyAcid("FA", 20, new DoubleBonds(new Dictionary<int, string>(){{6, "Z"}, {8, "E"}, {10, "E"}, {14, "Z"}}), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2}}});
                    }
                    
                case "Resolvin D3":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 4;
                        f2.position = 11;
                        f3.position = 17;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2, f3}}});
                    }
                    
                case "Maresin 1":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 4;
                        f2.position = 14;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2}}});
                    }
                    
                case "Resolvin D2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 4;
                        f2.position = 16;
                        f3.position = 17;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2, f3}}});
                    }
                    
                case "Resolvin D5":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 7;
                        f2.position = 17;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2}}});
                    }
                    
                case "Resolvin D1":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 7;
                        f2.position = 8;
                        f3.position = 17;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1, f2, f3}}});
                    }
                    
                case "TXB1":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f4 = KnownFunctionalGroups.get_functional_group("oxy");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        f4.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}, {"oxy", new List<FunctionalGroup>(){f4}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>{{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                
                case "TXB2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f4 = KnownFunctionalGroups.get_functional_group("oxy");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        f4.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}, {"oxy", new List<FunctionalGroup>(){f4}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "TXB3":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f4 = KnownFunctionalGroups.get_functional_group("oxy");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        f4.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}, {"oxy", new List<FunctionalGroup>(){f4}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(3), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PGF2alpha":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PGD2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("oxo");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2}}, {"oxo", new List<FunctionalGroup>(){f3}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PGE2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("oxo");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f3}}, {"oxy", new List<FunctionalGroup>(){f2}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PGB2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 15;
                        f2.position = 9;
                        Cycle cy = new Cycle(5, 8, 12, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "15d-PGJ2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("oxo");
                        f1.position = 15;
                        f2.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>(){{"oxo", new List<FunctionalGroup>(){f2}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(3), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PGJ2":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("oxo");
                        f1.position = 15;
                        f2.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>(){{"oxo", new List<FunctionalGroup>(){f2}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(2), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
            
            
            
                case "PGF1alpha":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 15;
                        f2.position = 9;
                        f3.position = 11;
                        //Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>()/*{{"OH", new List<FunctionalGroup>>(){f2, f3}}})*/;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                    
                case "PDX":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 10;
                        f2.position = 17;
                        return new FattyAcid("FA", 22, new DoubleBonds(6), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1, f2}}});
                    }
                    
                case "Oleic acid":
                case "OA":
                    return new FattyAcid("FA", 18, new DoubleBonds(1));
                    
                case "DGLA":
                    return new FattyAcid("FA", 20, new DoubleBonds(3));
                    
                case "iPF2alpha-VI":
                    {
                        FunctionalGroup f1 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f2 = KnownFunctionalGroups.get_functional_group("OH");
                        FunctionalGroup f3 = KnownFunctionalGroups.get_functional_group("OH");
                        f1.position = 5;
                        f2.position = 9;
                        f3.position = 11;
                        Cycle cy = new Cycle(5, 8, 12, null, new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f2, f3}}});
                        return new FattyAcid("FA", 20, new DoubleBonds(1), new Dictionary<string, List<FunctionalGroup>>(){{"OH", new List<FunctionalGroup>(){f1}}, {"cy", new List<FunctionalGroup>(){cy}}});
                    }
                
                default: return null;
            }
        }
        
        
        
        
        public Headgroup prepare_headgroup_and_checks()
        {
            Headgroup headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
            if (use_head_group) return headgroup;
            
            head_group = headgroup.get_class_name();
            int true_fa = 0;
            foreach (FattyAcid fa in fa_list)
            {
                true_fa += (fa.num_carbon > 0 || fa.double_bonds.get_num() > 0) ? 1 : 0;
            }
            int poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            
            // make lyso
            bool can_be_lyso = LipidClasses.lipid_classes.ContainsKey(Headgroup.get_class("L" + head_group)) ? LipidClasses.lipid_classes[Headgroup.get_class("L" + head_group)].special_cases.Contains("Lyso") : false;
            
            if ((true_fa + 1 == poss_fa || true_fa + 2 == poss_fa) && level != LipidLevel.SPECIES && headgroup.lipid_category == LipidCategory.GP && can_be_lyso)
            {
                if (true_fa + 1 == poss_fa) head_group = "L" + head_group;
                else head_group = "DL" + head_group;
                headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
                poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            }
    
            else if ((true_fa + 1 == poss_fa || true_fa + 2 == poss_fa) && level != LipidLevel.SPECIES && headgroup.lipid_category == LipidCategory.GL && head_group == "TG")
            {
                if (true_fa + 1 == poss_fa) head_group = "DG";
                else head_group = "MG";
                headgroup.decorators.Clear();
                headgroup = new Headgroup(head_group, headgroup_decorators, use_head_group);
                poss_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].possible_num_fa : 0;
            }
    
            // check if all functional groups have a position to be full structure
            if (is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE))
            {
                foreach (FattyAcid fa in fa_list)
                {
                    if (!check_full_structure(fa))
                    {
                        Console.WriteLine(fa.name + " " + fa.num_carbon);
                        set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
                        break;
                    }
                }
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
    
            else if (LipidClasses.lipid_classes[headgroup.lipid_class].special_cases.Contains("Lyso") && true_fa > poss_fa)
            {
                string hg_name = headgroup.headgroup;
                throw new ConstraintViolationException("Number of described fatty acyl chains (" + true_fa.ToString() + ") not allowed for lipid class '" + hg_name + "' (having " + poss_fa.ToString() + " fatty aycl chains).");
            }
            
            if (LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class))
            {
            
                if (LipidClasses.lipid_classes[headgroup.lipid_class].special_cases.Contains("HC"))
                {
                    fa_list[0].lipid_FA_bond_type = LipidFaBondType.ETHER;
                }
        
                if (LipidClasses.lipid_classes[headgroup.lipid_class].special_cases.Contains("Amide"))
                {
                    foreach (FattyAcid fatty in fa_list) fatty.lipid_FA_bond_type = LipidFaBondType.AMIDE;
                }
                
                
                int max_num_fa = LipidClasses.lipid_classes[headgroup.lipid_class].max_num_fa;
                if (max_num_fa != fa_list.Count) set_lipid_level(LipidLevel.MOLECULAR_SPECIES);
            }
            
            if (fa_list.Count > 0 && headgroup.sp_exception) fa_list[0].set_type(LipidFaBondType.LCB_EXCEPTION);
            return headgroup;
        }
        
        
        
        public LipidSpecies assemble_lipid(Headgroup headgroup)
        {
            foreach (FattyAcid fa in fa_list)
            {
                if (fa.stereo_information_missing())
                {
                    set_lipid_level(LipidLevel.FULL_STRUCTURE);
                    break;
                }
            }
            
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
