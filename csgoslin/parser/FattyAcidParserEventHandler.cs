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
    using Lst = System.Collections.Generic.List<Object>;

    public class FattyAcidParserHandler : BaseParserEventHandler<LipidAdduct>
    {
        
        public LipidLevel level;
        public LipidAdduct lipid;
        public string headgroup;
        public ExendedList<FattyAcid> fatty_acyl_stack;
        public Dict tmp;
        
        public static readonly Dictionary<string, int> last_numbers = new Dictionary<string, int>{{"un", 1}, {"hen", 1}, {"do", 2}, {"di", 2}, {"tri", 3}, {"buta", 4}, {"but", 4}, {"tetra", 4}, {"penta", 5}, {"pent", 5}, {"hexa", 6}, {"hex", 6}, {"hepta", 7}, {"hept", 7}, {"octa", 8}, {"oct", 8}, {"nona", 9}, {"non", 9}};


        public static readonly Dictionary<string, int> second_numbers = new Dictionary<string, int>{{"deca", 10}, {"dec", 10}, {"eicosa", 20}, {"eicos", 20 }, {"cosa", 20}, {"cos", 20}, {"triaconta", 30}, {"triacont", 30}, {"tetraconta", 40}, {"tetracont", 40}, {"pentaconta", 50}, {"pentacont", 50}, {"hexaconta", 60}, {"hexacont", 60}, {"heptaconta", 70}, {"heptacont", 70}, {"octaconta", 80}, {"octacont", 80}, {"nonaconta", 90}, {"nonacont", 90}};

        public static readonly Dictionary<string, string> func_groups = new Dictionary<string, string>{{"keto", "oxo"}, {"ethyl", "Et"}, {"hydroxy", "OH"}, {"phospho", "Ph"}, {"oxo", "oxo"}, {"bromo", "Br"}, {"methyl", "Me"}, {"hydroperoxy", "OOH"}, {"homo", ""}, {"Epoxy", "Ep"}, {"fluro", "F"}, {"fluoro", "F"}, {"chloro", "Cl"}, {"methylene", "My"}, {"sulfooxy", "Su"}, {"amino", "NH2"}, {"sulfanyl", "SH"}, {"methoxy", "OMe"}, {"iodo", "I"}, {"cyano", "CN"}, {"nitro", "NO2"}, {"OH", "OH"}, {"thio", "SH"}, {"mercapto", "SH"}, {"carboxy", "COOH"}, {"acetoxy", "Ac"}, {"cysteinyl", "Cys"}, {"phenyl", "Phe"}, {"s-glutathionyl", "SGlu"}, {"s-cysteinyl", "SCys"}, {"butylperoxy", "BOO"}, {"dimethylarsinoyl", "MMAs"}, {"methylsulfanyl", "SMe"}, {"imino", "NH"}, {"s-cysteinylglycinyl", "SCG"}};

        public static readonly Dictionary<string, int> ate = new Dictionary<string, int>{{"formate", 1}, {"acetate", 2}, {"butyrate", 4}, {"propionate", 3}, {"valerate", 5}, {"isobutyrate", 4}};

        public static readonly Dictionary<string, int> special_numbers = new Dictionary<string, int>{{"meth", 1}, {"etha", 2}, {"eth", 2}, {"propa", 3}, {"isoprop", 3}, {"prop", 3}, {"propi", 3}, {"propio", 3}, {"buta", 4}, {"but", 4}, {"butr", 4}, {"valer", 5}, {"eicosa", 20}, {"eicos", 20}, {"icosa", 20}, {"icos", 20}, {"prosta", 20}, {"prost", 20}, {"prostan", 20}};

        public static readonly HashSet<string> noic_set = new HashSet<string>{"noic acid", "nic acid", "dioic_acid"};
        public static readonly HashSet<string> nal_set = new HashSet<string>{"nal", "dial"};
        public static readonly HashSet<string> acetate_set = new HashSet<string>{"acetate", "noate", "nate"};
    
        public FattyAcidParserHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            /*
            registeretd_events.Add("lipid_post_event", build_lipid);
            registeretd_events.Add("fatty_acid_post_event", set_fatty_acid);
            
            registeretd_events.Add("acid_single_type_pre_event", set_fatty_acyl_type);
            registeretd_events.Add("ol_ending_pre_event", set_fatty_acyl_type);
            registeretd_events.Add("double_bond_position_pre_event", set_double_bond_information);
            registeretd_events.Add("double_bond_position_post_event", add_double_bond_information);
            registeretd_events.Add("db_number_post_event", set_double_bond_position);
            registeretd_events.Add("cistrans_post_event", set_cistrans);
            registeretd_events.Add("acid_type_double_post_event", check_db);
            registeretd_events.Add("db_length_pre_event", set_db_length);
            registeretd_events.Add("db_length_post_event", check_db_length);
            
            // lengths
            registeretd_events.Add("functional_length_pre_event", reset_length);
            registeretd_events.Add("fatty_length_pre_event", reset_length);
            registeretd_events.Add("functional_length_post_event", set_functional_length);
            registeretd_events.Add("fatty_length_post_event", set_fatty_length);
            
            // numbers
            registeretd_events.Add("notation_specials_pre_event", special_number);
            registeretd_events.Add("notation_last_digit_pre_event", last_number);
            registeretd_events.Add("notation_second_digit_pre_event", second_number);
            
            // functional groups
            registeretd_events.Add("functional_group_pre_event", set_functional_group);
            registeretd_events.Add("functional_group_post_event", add_functional_group);
            registeretd_events.Add("functional_pos_pre_event", set_functional_pos);
            registeretd_events.Add("functional_position_pre_event", set_functional_position);
            registeretd_events.Add("functional_group_type_pre_event", set_functional_type);
            
            // cyclo / epoxy
            registeretd_events.Add("cyclo_position_pre_event", set_functional_group);
            registeretd_events.Add("cyclo_position_post_event", rearrange_cycle);
            registeretd_events.Add("epoxy_pre_event", set_functional_group);
            registeretd_events.Add("epoxy_post_event", add_epoxy);
            registeretd_events.Add("cycle_pre_event", set_cycle);
            registeretd_events.Add("methylene_post_event", set_methylene);

            // dioic
            registeretd_events.Add("dioic_pre_event", set_functional_group);
            registeretd_events.Add("dioic_post_event", set_dioic);
            registeretd_events.Add("dioic_acid_pre_event", set_fatty_acyl_type);
            registeretd_events.Add("dial_post_event", set_dial);

            
            // prosta
            registeretd_events.Add("prosta_pre_event", set_prosta);
            registeretd_events.Add("prosta_post_event", add_cyclo);
            registeretd_events.Add("reduction_pre_event", set_functional_group);
            registeretd_events.Add("reduction_post_event", reduction);
            registeretd_events.Add("homo_post_event", homo);

            
            // recursion
            registeretd_events.Add("recursion_description_pre_event", set_recursion);
            registeretd_events.Add("recursion_description_post_event", add_recursion);
            registeretd_events.Add("recursion_pos_pre_event", set_recursion_pos);
            registeretd_events.Add("yl_ending_pre_event", set_yl_ending);
            registeretd_events.Add("acetic_acid_post_event", set_acetic_acid);
            registeretd_events.Add("acetic_recursion_pre_event", set_recursion);
            registeretd_events.Add("acetic_recursion_post_event", add_recursion);
            registeretd_events.Add("hydroxyl_number_pre_event", add_hydroxyl);
            registeretd_events.Add("ol_pre_event", setup_hydroxyl);
            registeretd_events.Add("ol_post_event", add_hydroxyls);
            registeretd_events.Add("ol_pos_post_event", set_yl_ending);
            
            
            // wax esters
            registeretd_events.Add("wax_ester_pre_event", set_recursion);
            registeretd_events.Add("wax_ester_post_event", add_wax_ester);
            registeretd_events.Add("ate_post_event", set_ate);
            registeretd_events.Add("isoprop_post_event", set_iso);
            registeretd_events.Add("isobut_post_event", set_iso);
            
            // CoA
            registeretd_events.Add("CoA_post_event", set_coa);
            registeretd_events.Add("methyl_pre_event", set_methyl);
            
            // CAR
            registeretd_events.Add("CAR_pre_event", set_car);
            registeretd_events.Add("CAR_post_event", add_car);
            
            // amine
            registeretd_events.Add("ethanolamine_post_event", add_ethanolamine);
            registeretd_events.Add("amine_n_pre_event", set_recursion);
            registeretd_events.Add("amine_n_post_event", add_amine);
            registeretd_events.Add("amine_post_event", add_amine_name);
            
            // functional group position summary
            registeretd_events.Add("fg_pos_summary_pre_event", set_functional_group);
            registeretd_events.Add("fg_pos_summary_post_event", add_summary);
            registeretd_events.Add("func_stereo_pre_event", add_func_stereo);
            
            */
            debug = "";
        }

                
        public void reset_parser(TreeNode node)
        {
            level = LipidLevel.ISOMERIC_SUBSPECIES;
            headgroup = "";
            fatty_acyl_stack = new ExendedList<FattyAcid>();
            fatty_acyl_stack.Add(new FattyAcid("FA"));
            tmp = new Dict();
            tmp.Add("fa1", new Dict());
        }
        
        
        public void set_lipid_level(LipidLevel _level)
        {
            level = (LipidLevel)Math.Min((int)level, (int)_level);
        }
        
        

        public void build_lipid(TreeNode node)
        {
            
            if (tmp.ContainsKey("cyclo_yl"))
            {
                tmp.Add("fg_pos", new Lst());
                
                Lst l1 = new Lst();
                l1.Add(1);
                l1.Add("");
                ((Lst)tmp["fg_pos"]).Add(l1);
                
                Lst l2 = new Lst();
                l2.Add((int)tmp["cyclo_len"]);
                l2.Add("");
                ((Lst)tmp["fg_pos"]).Add(l2);
                
                
                add_cyclo(node);
                tmp.Remove("cyclo_yl");
                tmp.Remove("cyclo_len");
            }
            
                    
            
            if (tmp.ContainsKey("post_adding"))
            {
                FattyAcid curr_fa_p = fatty_acyl_stack.back();
                int s = ((Lst)tmp["post_adding"]).Count;
                curr_fa_p.num_carbon += s;
                for (int i = 0; i < s; ++i)
                {
                    int pos = (int)((Lst)tmp["post_adding"])[i];
                    curr_fa_p.add_position(pos);
                    DoubleBonds db = new DoubleBonds(curr_fa_p.double_bonds.num_double_bonds);
                    foreach (KeyValuePair<int, string> kv in curr_fa_p.double_bonds.double_bond_positions)
                    {
                        db.double_bond_positions.Add(kv.Key + (kv.Key >= pos ? 1 : 0), kv.Value);
                    }
                    db.num_double_bonds = db.double_bond_positions.Count;
                    curr_fa_p.double_bonds = db;
                }
            }
            
            FattyAcid curr_fa = fatty_acyl_stack.back();
            if (curr_fa.double_bonds.double_bond_positions.Count > 0)
            {
                int db_right = 0;
                foreach (KeyValuePair<int, string> kv in curr_fa.double_bonds.double_bond_positions) db_right += kv.Value.Length > 0 ? 1 : 0;
                if (db_right != curr_fa.double_bonds.double_bond_positions.Count)
                {
                    set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
                }
            }
            
            Headgroup head_group = new Headgroup(headgroup);
            
            lipid = new LipidAdduct();
            
            switch(level)
            {
                case LipidLevel.ISOMERIC_SUBSPECIES:
                    lipid.lipid = new LipidIsomericSubspecies(head_group, fatty_acyl_stack);
                    break;
                    
                case LipidLevel.STRUCTURAL_SUBSPECIES:
                    lipid.lipid = new LipidStructuralSubspecies(head_group, fatty_acyl_stack);
                    break;
                    
                case LipidLevel.MOLECULAR_SUBSPECIES:
                    lipid.lipid = new LipidMolecularSubspecies(head_group, fatty_acyl_stack);
                    break;
                    
                case LipidLevel.SPECIES:
                    lipid.lipid = new LipidSpecies(head_group, fatty_acyl_stack);
                    break;
                    
                default:
                    break;
            }
            content = lipid;
        }


        public void switch_position(FunctionalGroup func_group, int switch_num)
        {
            func_group.position = switch_num - func_group.position;
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in func_group.functional_groups)
            {
                foreach (FunctionalGroup fg in kv.Value)
                {
                    switch_position(fg, switch_num);
                }
            }
        }


        public void set_fatty_acid(TreeNode node)
        {
            FattyAcid curr_fa = fatty_acyl_stack.back();
            
            /*
            if (tmp.get_dictionary(FA_I).contains_key("fg_pos_summary")){
                for (auto &kv : tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").dictionary){
                    string str_pos = kv.first;
                    string cistrans = to_upper(tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").get_string(str_pos));
                    int pos = atoi(str_pos.c_str());
                    if (pos > 0 && (cistrans == "E" || cistrans == "Z")){
                        curr_fa.double_bonds.double_bond_positions.insert({pos, cistrans});
                    }
                }
                curr_fa.double_bonds.num_double_bonds = curr_fa.double_bonds.double_bond_positions.size();
            }
            */
            
            if (curr_fa.functional_groups.ContainsKey("noyloxy"))
            {
                if (headgroup.Equals("FA")) headgroup = "FAHFA";
                
                while (curr_fa.functional_groups["noyloxy"].Count > 0)
                {
                    FattyAcid fa = (FattyAcid)curr_fa.functional_groups["noyloxy"][curr_fa.functional_groups["noyloxy"].Count - 1];
                    curr_fa.functional_groups["noyloxy"].RemoveAt(curr_fa.functional_groups["noyloxy"].Count - 1);
                
                    AcylAlkylGroup acyl = new AcylAlkylGroup(fa);
                    acyl.position = fa.position;
                    
                    if (!curr_fa.functional_groups.ContainsKey("acyl")) curr_fa.functional_groups.Add("acyl", new List<FunctionalGroup>());
                    curr_fa.functional_groups["acyl"].Add(acyl);
                }
                curr_fa.functional_groups.Remove("noyloxy");
            }
                
            else if (curr_fa.functional_groups.ContainsKey("nyloxy") || curr_fa.functional_groups.ContainsKey("yloxy"))
            {
                string yloxy = curr_fa.functional_groups.ContainsKey("nyloxy") ? "nyloxy" : "yloxy";
                while (curr_fa.functional_groups[yloxy].Count > 0)
                {
                    FattyAcid fa = (FattyAcid)curr_fa.functional_groups[yloxy][curr_fa.functional_groups[yloxy].Count - 1];
                    curr_fa.functional_groups[yloxy].RemoveAt(curr_fa.functional_groups[yloxy].Count - 1);
                    
                    AcylAlkylGroup alkyl = new AcylAlkylGroup(fa, -1, 1, true);
                    alkyl.position = fa.position;
                    
                    if (!curr_fa.functional_groups.ContainsKey("alkyl")) curr_fa.functional_groups.Add("alkyl", new List<FunctionalGroup>());
                    curr_fa.functional_groups["alkyl"].Add(alkyl);
                }
                curr_fa.functional_groups.Remove(yloxy);
            }
                    
            else
            {
                bool has_yl = false;
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in curr_fa.functional_groups)
                {
                    if (kv.Key.EndsWith("yl"))
                    {
                        has_yl = true;
                        break;
                    }
                }
                if (has_yl)
                {
                    while (true)
                    {
                        string yl = "";
                        foreach (KeyValuePair<string, List<FunctionalGroup> > kv in curr_fa.functional_groups)
                        {
                            if (kv.Key.EndsWith("yl"))
                            {
                                yl = kv.Key;
                                break;
                            }
                        }
                        if (yl.Length == 0)
                        {
                            break;
                        }
                    
                        while (curr_fa.functional_groups[yl].Count > 0)
                        {
                            FattyAcid fa = (FattyAcid)curr_fa.functional_groups[yl][curr_fa.functional_groups[yl].Count - 1];
                            curr_fa.functional_groups[yl].RemoveAt(curr_fa.functional_groups[yl].Count - 1);
                            
                            if (tmp.ContainsKey("cyclo"))
                            {
                                int cyclo_len = curr_fa.num_carbon;
                                tmp.Add("cyclo_len", cyclo_len);
                                if (fa.position != cyclo_len)
                                {
                                    switch_position(curr_fa, 2 + cyclo_len);
                                }
                                fa.shift_positions(cyclo_len);
                                
                                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in fa.functional_groups)
                                {
                                    if (!curr_fa.functional_groups.ContainsKey(kv.Key))
                                    {
                                        curr_fa.functional_groups.Add(kv.Key, new List<FunctionalGroup>());
                                    }
                                    foreach (FunctionalGroup func_group in kv.Value) curr_fa.functional_groups[kv.Key].Add(func_group);
                                }
                                    
                                curr_fa.num_carbon = cyclo_len + fa.num_carbon;
                                
                                foreach (KeyValuePair<int, string> kv in fa.double_bonds.double_bond_positions)
                                {
                                    curr_fa.double_bonds.double_bond_positions.Add(kv.Key + cyclo_len, kv.Value);
                                }
                                curr_fa.double_bonds.num_double_bonds = curr_fa.double_bonds.double_bond_positions.Count;
                                tmp.Add("cyclo_yl", true);
                            }
                            else {
                                // add carbon chains here here
                                // special chains: i.e. ethyl, methyl
                                string fg_name = "";
                                if (fa.double_bonds.get_num() == 0 && fa.functional_groups.Count == 0)
                                {
                                    FunctionalGroup fg = null;
                                    if (fa.num_carbon == 1)
                                    {
                                        fg_name = "Me";
                                        fg = KnownFunctionalGroups.get_functional_group(fg_name);
                                    }
                                    else if (fa.num_carbon == 2)
                                    {
                                        fg_name = "Et";
                                        fg = KnownFunctionalGroups.get_functional_group(fg_name);
                                    }
                                    if (fg_name.Length > 0)
                                    {
                                        fg.position = fa.position;
                                        if (!curr_fa.functional_groups.ContainsKey(fg_name)) curr_fa.functional_groups.Add(fg_name, new List<FunctionalGroup>());
                                        curr_fa.functional_groups[fg_name].Add(fg);
                                    }
                                }
                                if (fg_name.Length == 0)
                                {
                                    CarbonChain cc = new CarbonChain(fa, fa.position);
                                    if (!curr_fa.functional_groups.ContainsKey("cc")) curr_fa.functional_groups.Add("cc", new List<FunctionalGroup>());
                                    curr_fa.functional_groups["cc"].Add(cc);
                                }
                            }
                        }
                        if (tmp.ContainsKey("cyclo")) tmp.Remove("cyclo");
                        curr_fa.functional_groups.Remove(yl);
                    }
                }
            }
                
            if (curr_fa.functional_groups.ContainsKey("cyclo"))
            {
                FattyAcid fa = (FattyAcid)curr_fa.functional_groups["cyclo"][0];
                curr_fa.functional_groups.Remove("cyclo");
                tmp.Add("cyclo_len", curr_fa.num_carbon);
                int start_pos = curr_fa.num_carbon + 1;
                int end_pos = curr_fa.num_carbon + (tmp.ContainsKey("cyclo_len") ? (int)tmp["cyclo_len"] : 5);
                fa.shift_positions(start_pos - 1);
                
                if (curr_fa.functional_groups.ContainsKey("cy"))
                {
                    foreach (FunctionalGroup cy in curr_fa.functional_groups["cy"])
                    {
                        cy.shift_positions(start_pos - 1);
                    }
                }
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in fa.functional_groups)
                {
                    if (!curr_fa.functional_groups.ContainsKey(kv.Key))
                    {
                        curr_fa.functional_groups.Add(kv.Key, new List<FunctionalGroup>());
                    }
                    foreach (FunctionalGroup func_group in kv.Value)
                    {
                        curr_fa.functional_groups[kv.Key].Add(func_group);
                    }
                }
                
                curr_fa.num_carbon += fa.num_carbon;
                
                foreach (KeyValuePair<int, string> kv in fa.double_bonds.double_bond_positions) curr_fa.double_bonds.double_bond_positions.Add(kv.Key + start_pos - 1, kv.Value);
                curr_fa.double_bonds.num_double_bonds = curr_fa.double_bonds.double_bond_positions.Count;
                        
                tmp.Add("fg_pos", new Lst());
                Lst l1 = new Lst();
                l1.Add(start_pos);
                l1.Add("");
                ((Lst)tmp["fg_pos"]).Add(l1);
                Lst l2 = new Lst();
                l2.Add(end_pos);
                l2.Add("");
                ((Lst)tmp["fg_pos"]).Add(l2);
                
                add_cyclo(node);
                
                if (tmp.ContainsKey("cyclo_len")) tmp.Remove("cyclo_len");
                if (tmp.ContainsKey("cyclo")) tmp.Remove("cyclo");
            }
                
            else if (tmp.ContainsKey("cyclo"))
            {
                tmp.Add("cyclo_yl", 1);
                tmp.Add("cyclo_len", curr_fa.num_carbon);    
                tmp.Add("fg_pos", new Lst());
                Lst l1 = new Lst();
                l1.Add(1);
                l1.Add("");
                ((Lst)tmp["fg_pos"]).Add(l1);
                Lst l2 = new Lst();
                l2.Add(curr_fa.num_carbon);
                l2.Add("");
                ((Lst)tmp["fg_pos"]).Add(l2);
                
                tmp.Remove("cyclo");
            }
        }
        
        
        public void add_cyclo(TreeNode node)
        {
            int start = (int)((Lst)((Lst)tmp["fg_pos"])[0])[0];
            int end = (int)((Lst)((Lst)tmp["fg_pos"])[1])[0];
            
            
            DoubleBonds cyclo_db = new DoubleBonds();
            // check double bonds
            if (fatty_acyl_stack.back().double_bonds.double_bond_positions.Count > 0)
            {
                foreach (KeyValuePair<int, string> kv in fatty_acyl_stack.back().double_bonds.double_bond_positions)
                {
                    if (start <= kv.Key && kv.Key <= end)
                    {
                        cyclo_db.double_bond_positions.Add(kv.Key, kv.Value);
                    }
                }
                cyclo_db.num_double_bonds = cyclo_db.double_bond_positions.Count;
            
                foreach (KeyValuePair<int, string> kv in cyclo_db.double_bond_positions)
                {
                    fatty_acyl_stack.back().double_bonds.double_bond_positions.Remove(kv.Key);
                }
                fatty_acyl_stack.back().double_bonds.num_double_bonds = fatty_acyl_stack.back().double_bonds.double_bond_positions.Count;
                
            }        
            // check functional_groups
            Dictionary<string, List<FunctionalGroup> > cyclo_fg = new Dictionary<string, List<FunctionalGroup> >();
            HashSet<string> remove_list = new HashSet<string>();
            FattyAcid curr_fa = fatty_acyl_stack.back();
            
            if (curr_fa.functional_groups.ContainsKey("noyloxy"))
            {
                List<int> remove_item = new List<int>();
                int i = 0;
                foreach (FunctionalGroup func_group in curr_fa.functional_groups["noyloxy"])
                {
                    if (start <= func_group.position && func_group.position <= end)
                    {
                        CarbonChain cc = new CarbonChain((FattyAcid)func_group, func_group.position);
                        
                        if (!curr_fa.functional_groups.ContainsKey("cc")) curr_fa.functional_groups.Add("cc", new List<FunctionalGroup>());
                        curr_fa.functional_groups["cc"].Add(cc);
                        remove_item.Add(i);
                    }
                    ++i;
                }
                for (int ii = remove_item.Count - 1; ii >= 0; --ii)
                {
                    curr_fa.functional_groups["noyloxy"].RemoveAt(remove_item[ii]);
                }
                if (curr_fa.functional_groups["noyloxy"].Count == 0) remove_list.Add("noyloxy");
            }
            
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in curr_fa.functional_groups)
            {
                List<int> remove_item = new List<int>();
                int i = 0;
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    if (start <= func_group.position && func_group.position <= end)
                    {
                        if (!cyclo_fg.ContainsKey(kv.Key)) cyclo_fg.Add(kv.Key, new List<FunctionalGroup>());
                        cyclo_fg[kv.Key].Add(func_group);
                        remove_item.Add(i);
                    }
                    ++i;    
                }
                for (int ii = remove_item.Count - 1; ii >= 0; --ii)
                {
                    kv.Value.RemoveAt(remove_item[ii]);
                }
                if (kv.Value.Count == 0) remove_list.Add(kv.Key);
            }
            foreach (string fg in remove_list) curr_fa.functional_groups.Remove(fg);

            Cycle cycle = new Cycle(end - start + 1, start, end, cyclo_db, cyclo_fg);
            if (!fatty_acyl_stack.back().functional_groups.ContainsKey("cy")) fatty_acyl_stack.back().functional_groups.Add("cy", new List<FunctionalGroup>());
            fatty_acyl_stack.back().functional_groups["cy"].Add(cycle);
        }


        /*
        public void set_fatty_acyl_type(TreeNode node)
        {
            string t = node.get_text();
            
            if (endswith(t, "ol")) headgroup = "FOH";
            else if (contains(noic_set, t)) headgroup = "FA";
            else if (contains(nal_set, t)) headgroup = "FAL";
            else if (contains(acetate_set, t)) headgroup = "WE";
            else if (t == "ne"){
                headgroup = "HC";
                fatty_acyl_stack.back().lipid_FA_bond_type = AMINE;
            }
            else {
                headgroup = t;
            }
        }


        public void set_double_bond_information(TreeNode node)
        {
            tmp.get_dictionary(FA_I).set_int("db_position", 0);
            tmp.get_dictionary(FA_I).set_string("db_cistrans", "");
        }


        public void add_double_bond_information(TreeNode node)
        {    
            int pos = tmp.get_dictionary(FA_I).get_int("db_position");
            string str_pos = std::to_string(pos);
            string cistrans = tmp.get_dictionary(FA_I).get_string("db_cistrans");
            if (cistrans == "" && tmp.get_dictionary(FA_I).contains_key("fg_pos_summary") && tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").contains_key(str_pos)){
                cistrans = tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").get_string(str_pos);
            }
            if (pos == 0) return;
            
            cistrans = to_upper(cistrans);
            
            tmp.get_dictionary(FA_I).remove("db_position");
            tmp.get_dictionary(FA_I).remove("db_cistrans");
            
            
            if (cistrans != "E" && cistrans != "Z") cistrans = "";
            if (uncontains(fatty_acyl_stack.back().double_bonds.double_bond_positions, pos) || fatty_acyl_stack.back().double_bonds.double_bond_positions.at(pos).length() == 0){
                if (uncontains(fatty_acyl_stack.back().double_bonds.double_bond_positions, pos)){
                    fatty_acyl_stack.back().double_bonds.double_bond_positions.insert({pos, cistrans});
                }
                else {
                    fatty_acyl_stack.back().double_bonds.double_bond_positions.at(pos) = cistrans;
                }
                fatty_acyl_stack.back().double_bonds.num_double_bonds = fatty_acyl_stack.back().double_bonds.double_bond_positions.size();
            }
        }


        public void set_double_bond_position(TreeNode node)
        {
            int pos = atoi(node.get_text().c_str());
            int num_db = 0;
            if (tmp.contains_key("reduction")){
                GenericList *gl = tmp.get_list("reduction");
                int l = gl.list.size();
                for (int i = 0; i < l; ++i){
                    num_db += gl.get_int(i) < pos;
                }
            }
            
            tmp.get_dictionary(FA_I).set_int("db_position", pos - num_db);
        }


        public void set_cistrans(TreeNode node)
        {
            tmp.get_dictionary(FA_I).set_string("db_cistrans", node.get_text());
        }


        public void check_db(TreeNode node)
        {
            FattyAcid* curr_fa = fatty_acyl_stack.back();
            if (tmp.get_dictionary(FA_I).contains_key("fg_pos_summary")){
                for (auto &kv : tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").dictionary){
                    int k = atoi(kv.first.c_str());
                    string v = tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").get_string(kv.first);
                    if (k > 0 && uncontains(curr_fa.double_bonds.double_bond_positions, k) && (v == "E" || v == "Z" || v == "")){
                        curr_fa.double_bonds.double_bond_positions.insert({k, v});
                        curr_fa.double_bonds.num_double_bonds = curr_fa.double_bonds.double_bond_positions.size();
                    }
                }
            }
        }


        public void reset_length(TreeNode node)
        {
            tmp.set_int("length", 0);
        }


        public void set_functional_length(TreeNode node)
        {
            if (tmp.get_int("length") != (int)tmp.get_list("fg_pos").list.size()){
                throw LipidException("Length of functional group '" + std::to_string(tmp.get_int("length")) + "' does not match with number of its positions '" + std::to_string(tmp.get_list("fg_pos").list.size()) + "'");
            }
        }


        public void set_fatty_length(TreeNode node)
        {
            fatty_acyl_stack.back().num_carbon += tmp.get_int("length");
        }


        public void special_number(TreeNode node)
        {
            tmp.set_int("length", tmp.get_int("length") + special_numbers.at(node.get_text()));
        }


        public void last_number(TreeNode node)
        {
            tmp.set_int("length", tmp.get_int("length") + last_numbers.at(node.get_text()));
        }


        public void second_number(TreeNode node)
        {
            tmp.set_int("length", tmp.get_int("length") + second_numbers.at(node.get_text()));
        }


        public void set_functional_group(TreeNode node)
        {
            tmp.set_list("fg_pos", new GenericList());
            tmp.set_string("fg_type", "");
        }


        public void add_functional_group(TreeNode node)
        {
            if (tmp.contains_key("added_func_group")){
                tmp.remove("added_func_group");
                return;
            }
            
            else if (tmp.contains_key("add_methylene")){ 
                tmp.remove("add_methylene");
                add_cyclo(node);
                return;
            }
            
            string t = tmp.get_string("fg_type");
            
            FunctionalGroup *fg = 0;
            if (t != "acetoxy"){
                if (uncontains(func_groups, t)){
                    throw LipidException("Unknown functional group: '" + t + "'");
                }
                t = func_groups.at(t);
                if (t.length() == 0) return;
                fg = KnownFunctionalGroups::get_functional_group(t);
            }
            else {
                fg = new AcylAlkylGroup(new FattyAcid("O", 2));
            }
            
            FattyAcid* fa = fatty_acyl_stack.back();
            if (uncontains_p(fa.functional_groups, t)) fa.functional_groups.insert({t, new List<FunctionalGroup>());
            int l = tmp.get_list("fg_pos").list.size();
            for (int i = 0; i < l; ++i){
                int pos = tmp.get_list("fg_pos").get_list(i).get_int(0);
                
                int num_pos = 0;
                if (tmp.contains_key("reduction")){
                    GenericList *gl = tmp.get_list("reduction");
                    int l = gl.list.size();
                    for (int i = 0; i < l; ++i){
                        num_pos += gl.get_int(i) < pos;
                    }
                }
                FunctionalGroup* fg_insert = fg.copy();
                fg_insert.position = pos - num_pos;
                fa.functional_groups.at(t).push_back(fg_insert);
            }
            delete fg;
        }


        public void set_functional_pos(TreeNode node)
        {
            GenericList* gl = tmp.get_list("fg_pos");
            int s = gl.list.size();
            gl.get_list(s - 1).set_int(0, atoi(node.get_text().c_str()));
        }


        public void set_functional_position(TreeNode node)
        {
            GenericList* gl = new GenericList();
            gl.add_int(0);
            gl.add_string("");
            tmp.get_list("fg_pos").add_list(gl);
        }


        public void set_functional_type(TreeNode node)
        {
            tmp.set_string("fg_type", node.get_text());
        }


        public void rearrange_cycle(TreeNode node)
        {
            if (tmp.contains_key("post_adding")){
                fatty_acyl_stack.back().num_carbon += tmp.get_list("post_adding").list.size();
                tmp.remove("post_adding");
            }
                
            FattyAcid* curr_fa = fatty_acyl_stack.back();
            int start = tmp.get_list("fg_pos").get_list(0).get_int(0);
            if (contains_p(curr_fa.functional_groups, "cy")){
                for (auto &cy : curr_fa.functional_groups.at("cy")){
                    int shift_val = start - cy.position;
                    if (shift_val == 0) continue;
                    ((Cycle*)cy).rearrange_functional_groups(curr_fa, shift_val);
                }
            }
        }


        public void add_epoxy(TreeNode node)
        {
            GenericList *gl = tmp.get_list("fg_pos");
            while(gl.list.size() > 1){
                gl.del(gl.list.back());
                gl.list.pop_back();
            }
            tmp.set_string("fg_type", "Epoxy");
        }


        public void set_cycle(TreeNode node)
        {
            tmp.set_int("cyclo", 1);
        }


        public void set_methylene(TreeNode node)
        {
            tmp.set_string("fg_type", "methylene");
            GenericList *gl = tmp.get_list("fg_pos");
            if (gl.list.size() > 1){
                if (gl.get_list(0).get_int(0) < gl.get_list(1).get_int(0)) {
                    gl.get_list(1).set_int(0, gl.get_list(1).get_int(0) + 1);
                }
                else if (gl.get_list(0).get_int(0) > gl.get_list(1).get_int(0)){
                    gl.get_list(0).set_int(0, gl.get_list(0).get_int(0) + 1);
                }
                fatty_acyl_stack.back().num_carbon += 1;
                tmp.set_int("add_methylene", 1);
            }
        }


        public void set_dioic(TreeNode node)
        {
            headgroup = "FA";
            
            int pos = (tmp.get_list("fg_pos").list.size() == 2) ? tmp.get_list("fg_pos").get_list(1).get_int(0) : fatty_acyl_stack.back().num_carbon;
            fatty_acyl_stack.back().num_carbon -= 1;
            FunctionalGroup* func_group = KnownFunctionalGroups::get_functional_group("COOH");
            func_group.position = pos - 1;
            if (uncontains_p(fatty_acyl_stack.back().functional_groups, "COOH")) fatty_acyl_stack.back().functional_groups.insert({"COOH", new List<FunctionalGroup>());
            fatty_acyl_stack.back().functional_groups.at("COOH").push_back(func_group);
        }


        public void set_dial(TreeNode node)
        {
            FattyAcid* curr_fa = fatty_acyl_stack.back();
            int pos = curr_fa.num_carbon;
            FunctionalGroup *fg = KnownFunctionalGroups::get_functional_group("oxo");
            fg.position = pos;
            if (uncontains_p(curr_fa.functional_groups, "oxo")) curr_fa.functional_groups.insert({"oxo", new List<FunctionalGroup>());
            curr_fa.functional_groups.at("oxo").push_back(fg);
        }


        public void set_prosta(TreeNode node)
        {
            int minus_pos = 0;
            if (tmp.contains_key("reduction")){
                GenericList *gl = tmp.get_list("reduction");
                for (int i = 0; i < (int)gl.list.size(); ++i){
                    minus_pos = gl.get_int(i) < 8;
                }
            }
            tmp.set_list("fg_pos", new GenericList());
            tmp.get_list("fg_pos").add_list(new GenericList());
            tmp.get_list("fg_pos").add_list(new GenericList());
            tmp.get_list("fg_pos").get_list(0).add_int(8 - minus_pos);
            tmp.get_list("fg_pos").get_list(0).add_string("");
            tmp.get_list("fg_pos").get_list(1).add_int(12 - minus_pos);
            tmp.get_list("fg_pos").get_list(1).add_string("");
            tmp.set_string("fg_type", "cy");
        }


        


        public void reduction(TreeNode node)
        {
            fatty_acyl_stack.back().num_carbon -= tmp.get_list("fg_pos").list.size();
            tmp.set_list("reduction", new GenericList());
            for (int i = 0; i < (int)tmp.get_list("fg_pos").list.size(); ++i){
                tmp.get_list("reduction").add_int(tmp.get_list("fg_pos").get_list(i).get_int(0));
            }
        }


        public void homo(TreeNode node)
        {
            tmp.set_list("post_adding", new GenericList());
            for (int i = 0; i < (int)tmp.get_list("fg_pos").list.size(); ++i){
                tmp.get_list("post_adding").add_int(tmp.get_list("fg_pos").get_list(i).get_int(0));
            }
        }


        public void set_recursion(TreeNode node)
        {
            tmp.set_list("fg_pos", new GenericList());
            tmp.set_string("fg_type", "");
            fatty_acyl_stack.push_back(new FattyAcid("FA"));
            tmp.set_dictionary(FA_I, new GenericDictionary());
            tmp.get_dictionary(FA_I).set_int("recursion_pos", 0);
        }


        public void add_recursion(TreeNode node)
        {
                int pos = tmp.get_dictionary(FA_I).get_int("recursion_pos");
                
                FattyAcid *fa = fatty_acyl_stack.back();
                fatty_acyl_stack.pop_back();
                fa.position = pos;
                FattyAcid *curr_fa = fatty_acyl_stack.back();
                
                string fname = "";
                if (tmp.contains_key("cyclo_yl")){
                    fname = "cyclo";
                    tmp.remove("cyclo_yl");
                }
                else {
                    fname = headgroup;
                }
                if (uncontains_p(curr_fa.functional_groups, fname)) curr_fa.functional_groups.insert({fname, new List<FunctionalGroup>());
                curr_fa.functional_groups.at(fname).push_back(fa);
                tmp.set_int("added_func_group", 1);
        }


        public void set_recursion_pos(TreeNode node)
        {
            tmp.get_dictionary(FA_I).set_int("recursion_pos", atoi(node.get_text().c_str()));
        }


        public void set_yl_ending(TreeNode node)
        {
            int l = atoi(node.get_text().c_str()) - 1;
            if (l == 0) return;

            FattyAcid *curr_fa = fatty_acyl_stack.back();
            string fname = "";
            FunctionalGroup *fg = 0;
            if (l == 1){
                fname = "Me";
                fg = KnownFunctionalGroups::get_functional_group(fname);
            }
            else if (l == 2){
                fname = "Et";
                fg = KnownFunctionalGroups::get_functional_group(fname);
            }
            else {
                FattyAcid *fa = new FattyAcid("FA", l);
                // shift functional groups
                for (auto &kv : *(curr_fa.functional_groups)){
                    vector<int> remove_item;
                    int i = 0;
                    for (auto &func_group : kv.second){
                        if (func_group.position <= l){
                            remove_item.push_back(i);
                            if (uncontains_p(fa.functional_groups, kv.first)) fa.functional_groups.insert({kv.first, new List<FunctionalGroup>());
                            func_group.position = l + 1 - func_group.position;
                            fa.functional_groups.at(kv.first).push_back(func_group);
                        }
                    }
                    for (int i = remove_item.size() - 1; i >= 0; --i) curr_fa.functional_groups.at(kv.first).erase(curr_fa.functional_groups.at(kv.first).begin() + remove_item.at(i));
                }
                map<string, vector<FunctionalGroup*> > *tmp = curr_fa.functional_groups;
                curr_fa.functional_groups = new map<string, vector<FunctionalGroup*> >();
                for (auto &kv : *tmp){
                    if (!kv.second.empty()) curr_fa.functional_groups.insert({kv.first, kv.second});
                }
                delete tmp;
                
                // shift double bonds
                if (!curr_fa.double_bonds.double_bond_positions.empty()){
                    delete fa.double_bonds;
                    fa.double_bonds = new DoubleBonds();
                    for (auto &kv : curr_fa.double_bonds.double_bond_positions){
                        if (kv.first <= l) fa.double_bonds.double_bond_positions.insert({l + 1 - kv.first, kv.second});
                    }
                    fa.double_bonds.num_double_bonds = fa.double_bonds.double_bond_positions.size();
                    for (auto &kv : fa.double_bonds.double_bond_positions) curr_fa.double_bonds.double_bond_positions.erase(kv.first);
                }
                fname = "cc";
                fg = new CarbonChain(fa);
            }
            curr_fa.num_carbon -= l;
            fg.position = l;
            curr_fa.shift_positions(-l);
            if (uncontains_p(curr_fa.functional_groups, fname)) curr_fa.functional_groups.insert({fname, new List<FunctionalGroup>());
            curr_fa.functional_groups.at(fname).push_back(fg);
        }


        public void set_acetic_acid(TreeNode node)
        {
            fatty_acyl_stack.back().num_carbon += 2;
            headgroup = "FA";
        }


        public void add_hydroxyl(TreeNode node)
        {
            int h = atoi(node.get_text().c_str());
            tmp.get_list("hydroxyl_pos").add_int(h);
        }


        public void setup_hydroxyl(TreeNode node)
        {
            tmp.set_list("hydroxyl_pos", new GenericList());
        }


        public void add_hydroxyls(TreeNode node)
        {       
            
            if (tmp.get_list("hydroxyl_pos").list.size() > 1){
                FunctionalGroup *fg_oh = KnownFunctionalGroups::get_functional_group("OH");
                vector<int> sorted_pos;
                for (int i = 0; i < (int)tmp.get_list("hydroxyl_pos").list.size(); ++i) sorted_pos.push_back(tmp.get_list("hydroxyl_pos").get_int(i));
                std::sort(sorted_pos.rbegin(), sorted_pos.rend());
                for (int i = 0; i < (int)sorted_pos.size() - 1; ++i){
                    int pos = sorted_pos.at(i);
                    FunctionalGroup *fg_insert = fg_oh.copy();
                    fg_insert.position = pos;
                    if (uncontains_p(fatty_acyl_stack.back().functional_groups, "OH")) fatty_acyl_stack.back().functional_groups.insert({"OH", new List<FunctionalGroup>());
                    fatty_acyl_stack.back().functional_groups.at("OH").push_back(fg_insert);
                }
                delete fg_oh;
            }
        }


        public void add_wax_ester(TreeNode node)
        {
            FattyAcid *fa = fatty_acyl_stack.back();
            fatty_acyl_stack.pop_back();
            
            fa.name += "1";
            fa.lipid_FA_bond_type = AMINE;
            fatty_acyl_stack.back().name += "2";
            fatty_acyl_stack.insert(fatty_acyl_stack.begin(), fa);
        }


        public void set_ate(TreeNode node)
        {
            fatty_acyl_stack.back().num_carbon += ate.at(node.get_text());
            headgroup = "WE";
        }


        public void set_iso(TreeNode node)
        {
                FattyAcid *curr_fa = fatty_acyl_stack.back();
                curr_fa.num_carbon -= 1;
                FunctionalGroup *fg = KnownFunctionalGroups::get_functional_group("Me");
                fg.position = 2;
                if (uncontains_p(curr_fa.functional_groups, "Me")) curr_fa.functional_groups.insert({"Me", new List<FunctionalGroup>());
                curr_fa.functional_groups.at("Me").push_back(fg);
        }


        public void set_coa(TreeNode node)
        {
            headgroup = "CoA";
        }


        public void set_methyl(TreeNode node)
        {
            fatty_acyl_stack.back().num_carbon += 1;
        }


        public void set_car(TreeNode node)
        {
            tmp.set_list("fg_pos", new GenericList());
            tmp.set_string("fg_type", "");
        }


        public void add_car(TreeNode node)
        {
            headgroup = "CAR";
        }


        public void add_ethanolamine(TreeNode node)
        {
            headgroup = "NAE";
        }


        public void add_amine(TreeNode node)
        {
            FattyAcid *fa = fatty_acyl_stack.back();
            fatty_acyl_stack.pop_back();
            
            fa.name += "1";
            fatty_acyl_stack.back().name += "2";
            fa.lipid_FA_bond_type = AMINE;
            fatty_acyl_stack.insert(fatty_acyl_stack.begin(), fa);
        }


        public void add_amine_name(TreeNode node)
        {
            headgroup = "NA";
        }


        public void add_summary(TreeNode node)
        {    
            tmp.get_dictionary(FA_I).set_dictionary("fg_pos_summary", new GenericDictionary());
            for (int i = 0; i < (int)tmp.get_list("fg_pos").list.size(); ++i){
                string k = std::to_string(tmp.get_list("fg_pos").get_list(i).get_int(0));
                string v = to_upper(tmp.get_list("fg_pos").get_list(i).get_string(1));
                tmp.get_dictionary(FA_I).get_dictionary("fg_pos_summary").set_string(k, v);
            }
        }


        public void add_func_stereo(TreeNode node)
        {
            int l = tmp.get_list("fg_pos").list.size();
            tmp.get_list("fg_pos").get_list(l - 1).set_string(1, node.get_text());
        }


        public void set_db_length(TreeNode node)
        {
            tmp.set_int("old_length", tmp.get_int("length"));
            tmp.set_int("length", 0);
        }


        public void check_db_length(TreeNode node)
        {
            int old_length = tmp.get_int("old_length");
            int db_length = tmp.get_int("length");
            
            if (old_length < db_length) fatty_acyl_stack.back().num_carbon += db_length;
        }
        */
    }
}
