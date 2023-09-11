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
    //using Dict = System.Collections.Generic.Dictionary<string, Object>;
    using Lst = System.Collections.Generic.List<Object>;
    using static LevelFunctions;

        
    
    public class GoslinParserEventHandler : LipidBaseParserEventHandler
    {
        
        public int db_position;
        public string db_cistrans;
        public char plasmalogen;
        public bool unspecified_ether;
        public string mediator_function;
        public List<int> mediator_function_positions;
        public bool mediator_suffix;
        public Element heavy_element;
        public int heavy_element_number;
    
        public static Dictionary<string, int> mediator_FA = new Dictionary<string, int>(){{"H", 17}, {"O", 18}, {"E", 20}, {"Do", 22}};
        public static Dictionary<string, int> mediator_DB = new Dictionary<string, int>(){{"M", 1}, {"D", 2}, {"Tr", 3}, {"T", 4}, {"P", 5}, {"H", 6}};
        
        
    
        public GoslinParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);
            registered_events.Add("hg_cl_pre_event", set_head_group_name);
            registered_events.Add("hg_mlcl_pre_event", set_head_group_name);
            registered_events.Add("hg_pl_pre_event", set_head_group_name);
            registered_events.Add("hg_lpl_pre_event", set_head_group_name);
            registered_events.Add("hg_lsl_pre_event", set_head_group_name);
            registered_events.Add("hg_so_lsl_pre_event", set_head_group_name);
            registered_events.Add("hg_dsl_pre_event", set_head_group_name);
            registered_events.Add("st_pre_event", set_head_group_name);
            registered_events.Add("hg_ste_pre_event", set_head_group_name);
            registered_events.Add("hg_stes_pre_event", set_head_group_name);
            registered_events.Add("hg_mgl_pre_event", set_head_group_name);
            registered_events.Add("hg_dgl_pre_event", set_head_group_name);
            registered_events.Add("hg_sgl_pre_event", set_head_group_name);
            registered_events.Add("hg_tgl_pre_event", set_head_group_name);
            registered_events.Add("hg_dlcl_pre_event", set_head_group_name);
            registered_events.Add("hg_sac_di_pre_event", set_head_group_name);
            registered_events.Add("hg_sac_f_pre_event", set_head_group_name);
            registered_events.Add("hg_tpl_pre_event", set_head_group_name);  
            registered_events.Add("gl_species_pre_event", set_species_level);
            registered_events.Add("pl_species_pre_event", set_species_level);
            registered_events.Add("sl_species_pre_event", set_species_level);
            registered_events.Add("fa2_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa3_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa4_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("slbpa_pre_event", set_molecular_subspecies_level);
            registered_events.Add("dlcl_pre_event", set_molecular_subspecies_level);
            registered_events.Add("mlcl_pre_event", set_molecular_subspecies_level);
            registered_events.Add("lcb_pre_event", new_lcb);
            registered_events.Add("lcb_post_event", clean_lcb);
            registered_events.Add("fa_pre_event", new_fa);
            registered_events.Add("fa_post_event", append_fa);
            registered_events.Add("db_single_position_pre_event", set_isomeric_level);
            registered_events.Add("db_single_position_post_event", add_db_position);
            registered_events.Add("db_position_number_pre_event", add_db_position_number);
            registered_events.Add("cistrans_pre_event", add_cistrans);
            registered_events.Add("ether_pre_event", add_ether);
            registered_events.Add("old_hydroxyl_pre_event", add_old_hydroxyl);
            registered_events.Add("db_count_pre_event", add_double_bonds);
            registered_events.Add("carbon_pre_event", add_carbon);
            registered_events.Add("hydroxyl_pre_event", add_hydroxyl);
            registered_events.Add("adduct_info_pre_event", new_adduct);
            registered_events.Add("adduct_pre_event", add_adduct);
            registered_events.Add("charge_pre_event", add_charge);
            registered_events.Add("charge_sign_pre_event", add_charge_sign);
            registered_events.Add("lpl_pre_event", set_molecular_subspecies_level);
            registered_events.Add("plasmalogen_pre_event", set_plasmalogen);
            registered_events.Add("mediator_pre_event", set_mediator);
            registered_events.Add("mediator_post_event", add_mediator);
            registered_events.Add("unstructured_mediator_pre_event", set_unstructured_mediator);
            registered_events.Add("trivial_mediator_pre_event", set_trivial_mediator);
            registered_events.Add("mediator_carbon_pre_event", set_mediator_carbon);
            registered_events.Add("mediator_db_pre_event", set_mediator_db);
            registered_events.Add("mediator_mono_functions_pre_event", set_mediator_function);
            registered_events.Add("mediator_di_functions_pre_event", set_mediator_function);
            registered_events.Add("mediator_position_pre_event", set_mediator_function_position);
            registered_events.Add("mediator_functional_group_post_event", add_mediator_function);
            registered_events.Add("mediator_suffix_pre_event", add_mediator_suffix);
            registered_events.Add("mediator_tetranor_pre_event", set_mediator_tetranor);
            registered_events.Add("isotope_pair_pre_event", new_adduct);
            registered_events.Add("isotope_element_pre_event", set_heavy_d_element);
            registered_events.Add("isotope_number_pre_event", set_heavy_d_number);
            registered_events.Add("heavy_pre_event", new_adduct);
            registered_events.Add("adduct_heavy_element_pre_event", set_heavy_element);
            registered_events.Add("adduct_heavy_number_pre_event", set_heavy_number);
            registered_events.Add("adduct_heavy_component_post_event", add_heavy_component);
            
            
            debug = "";
        }

                
        public void reset_parser(TreeNode node)
        {
            content = null;
            level = LipidLevel.FULL_STRUCTURE;
            head_group = "";
            lcb = null;
            fa_list = new List<FattyAcid>();
            current_fa = null;
            adduct = null;
            db_position = 0;
            db_cistrans = "";
            plasmalogen = '\0';
        }
        
    

        public void set_mediator(TreeNode node)
        {
            head_group = "FA";
            current_fa = new FattyAcid("FA");
            fa_list.Add(current_fa);
            set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
        }
    
    
        public void set_unstructured_mediator(TreeNode node)
        {
            head_group = node.get_text();
            use_head_group = true;
            fa_list.Clear();
        }
        
        
        public void set_mediator_tetranor(TreeNode node)
        {
            current_fa.num_carbon -= 4;
        }
            

        public void set_mediator_carbon(TreeNode node)
        {
            current_fa.num_carbon += mediator_FA[node.get_text()];
        }
                
                
                
        public void set_heavy_d_element(TreeNode node)
        {
            adduct.heavy_elements[Element.H2] = 1;
        }
                
                
                
        public void set_heavy_d_number(TreeNode node)
        {
            adduct.heavy_elements[Element.H2] = node.get_int();
        }

            

        public void set_mediator_db(TreeNode node)
        {
            current_fa.double_bonds.num_double_bonds = mediator_DB[node.get_text()];
        }
            
            
        public void set_mediator_function(TreeNode node)
        {
            mediator_function = node.get_text();
        }
            
            
        public void set_mediator_function_position(TreeNode node)
        {
            mediator_function_positions.Add(node.get_int());
        }
        
        
        
        public void add_mediator_function(TreeNode node)
        {
            FunctionalGroup functional_group = null;
            string fg = "";
            if (mediator_function.Equals("H"))
            {
                functional_group = KnownFunctionalGroups.get_functional_group("OH");
                fg = "OH";
                if (mediator_function_positions.Count > 0) functional_group.position = mediator_function_positions[0];
            }
                
            else if (mediator_function.Equals("Oxo"))
            {
                functional_group = KnownFunctionalGroups.get_functional_group("oxo");
                fg = "oxo";
                if (mediator_function_positions.Count > 0) functional_group.position = mediator_function_positions[0];
            }
                
            else if (mediator_function.Equals("Hp"))
            {
                functional_group = KnownFunctionalGroups.get_functional_group("OOH");
                fg = "OOH";
                if (mediator_function_positions.Count > 0) functional_group.position = mediator_function_positions[0];
            }
                
            else if (mediator_function.Equals("E") || mediator_function.Equals("Ep"))
            {
                functional_group = KnownFunctionalGroups.get_functional_group("Ep");
                fg = "Ep";
                if (mediator_function_positions.Count > 0) functional_group.position = mediator_function_positions[0];
            }
                
            else if (mediator_function.Equals("DH") || mediator_function.Equals("DiH") || mediator_function.Equals("diH"))
            {
                functional_group = KnownFunctionalGroups.get_functional_group("OH");
                fg = "OH";
                if (mediator_function_positions.Count > 0)
                {
                    functional_group.position = mediator_function_positions[0];
                    FunctionalGroup functional_group2 = KnownFunctionalGroups.get_functional_group("OH");
                    functional_group2.position = mediator_function_positions[1];
                    current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
                    current_fa.functional_groups["OH"].Add(functional_group2);
                }
            }
                
            if (!current_fa.functional_groups.ContainsKey(fg)) current_fa.functional_groups.Add(fg, new List<FunctionalGroup>());
            current_fa.functional_groups[fg].Add(functional_group);
        }
        

        public void set_trivial_mediator(TreeNode node)
        {
            head_group = "FA";
            string mediator_name = node.get_text();
            
            current_fa = resolve_fa_synonym(mediator_name);
            fa_list.Clear();
            fa_list.Add(current_fa);
            mediator_suffix = true;
        }
        
        
        public void add_mediator_suffix(TreeNode node)
        {
            mediator_suffix = true;
        }
            
            
        public void add_mediator(TreeNode node)
        {
            if (!mediator_suffix)
            {
                current_fa.double_bonds.num_double_bonds -= 1;
            }
        }


        public void set_plasmalogen(TreeNode node)
        {
            plasmalogen = node.get_text().ToUpper()[0];
        }


        void set_unspecified_ether(TreeNode node)
        {
            
        }
        
        


        public void add_plasmalogen(TreeNode node)
        {
            plasmalogen = node.get_text().ToUpper()[0];
        }
        

        public void set_head_group_name(TreeNode node)
        {
            head_group = node.get_text();
        }


        public void set_species_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
        }

        
        public void set_isomeric_level(TreeNode node)
        {
            db_position = 0;
            db_cistrans = "";
        }


        public void add_db_position(TreeNode node)
        {
            if (current_fa != null)
            {
                current_fa.double_bonds.double_bond_positions.Add(db_position, db_cistrans);
                if (!db_cistrans.Equals("E") && !db_cistrans.Equals("Z")) set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            }
        }


        public void add_db_position_number(TreeNode node)
        {
            db_position = Convert.ToInt32(node.get_text());
        }


        public void add_cistrans(TreeNode node){
            db_cistrans = node.get_text();
        }
            
            

        public void set_molecular_subspecies_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.MOLECULAR_SPECIES);
        }
            
            
        public void new_fa(TreeNode node)
        {
            LipidFaBondType lipid_FA_bond_type = LipidFaBondType.ESTER;
            if (unspecified_ether)
            {
                unspecified_ether = false;
                lipid_FA_bond_type = LipidFaBondType.ETHER_UNSPECIFIED;
            }
            current_fa = new FattyAcid("FA", 2, null, null, lipid_FA_bond_type);
        }
            
            

        public void new_lcb(TreeNode node)
        {
            lcb = new FattyAcid("LCB");
            current_fa = lcb;
            set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            lcb.set_type(LipidFaBondType.LCB_REGULAR);
        }
                
                

        public void clean_lcb(TreeNode node)
        {
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                set_lipid_level(LipidLevel.SN_POSITION);
            }
            current_fa = null;
        }
            
            
                

        public void append_fa(TreeNode node)
        {
            if (current_fa.lipid_FA_bond_type == LipidFaBondType.ETHER_UNSPECIFIED)
            {
                throw new LipidException("Lipid with unspecified ether bond cannot be treated properly.");
            }
            
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                set_lipid_level(LipidLevel.SN_POSITION);
            }
            
            if (current_fa.double_bonds.get_num() < 0)
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }

            fa_list.Add(current_fa);
            current_fa = null;
            
            if (head_group.Equals("Sa") || head_group.Equals("So") || head_group.Equals("S1P") || head_group.Equals("Sa1P"))
            {
                FattyAcid fa = fa_list[0];
                
                FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
                if (head_group.Equals("Sa") || head_group.Equals("So"))
                {
                    functional_group.count = 2;
                    fa.lipid_FA_bond_type = LipidFaBondType.LCB_EXCEPTION;
                }
                else
                {
                    functional_group.count = 1;
                    fa.lipid_FA_bond_type = LipidFaBondType.LCB_REGULAR;
                }
                if (!fa.functional_groups.ContainsKey("OH")) fa.functional_groups.Add("OH", new List<FunctionalGroup>());
                fa.functional_groups["OH"].Add(functional_group);
            }
        }
            
            

        public void build_lipid(TreeNode node)
        {
            if (lcb != null)
            {
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
                fa_list.Insert(0, lcb);
            }
            
            if (lcb == null && plasmalogen != '\0' && fa_list.Count > 0)
            {
                fa_list[0].lipid_FA_bond_type = plasmalogen == 'O' ? LipidFaBondType.ETHER_PLASMANYL : LipidFaBondType.ETHER_PLASMENYL;
            }
            
            Headgroup headgroup = prepare_headgroup_and_checks();
            
            LipidAdduct lipid = new LipidAdduct();
            lipid.lipid = assemble_lipid(headgroup);
            lipid.adduct = adduct;
            content = lipid;
            
        }
            
            

        public void add_ether(TreeNode node)
        {
            string ether = node.get_text();
            if (ether.Equals("a")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether.Equals("p"))
            {
                current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
                current_fa.double_bonds.num_double_bonds = Math.Max(0, current_fa.double_bonds.num_double_bonds - 1);
            }
            plasmalogen = '\0';
        }
            
            

        public void add_old_hydroxyl(TreeNode node)
        {
            string old_hydroxyl = node.get_text();
            int num_h = 0;
            if (old_hydroxyl.Equals("d")) num_h = 2;
            else if (old_hydroxyl.Equals("t")) num_h = 3;
            
            
            if (sp_regular_lcb()) num_h -= 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
        }
            
            

        public void add_double_bonds(TreeNode node)
        {
            current_fa.double_bonds.num_double_bonds = Convert.ToInt32(node.get_text());
        }
            
            

        public void add_carbon(TreeNode node)
        {
            current_fa.num_carbon = Convert.ToInt32(node.get_text());
        }
            
            

        public void add_hydroxyl(TreeNode node)
        {
            int num_h = Convert.ToInt32(node.get_text());
            if (sp_regular_lcb()) num_h -= 1;
            if (num_h <= 0) return;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
            set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
        }
            
            

        public void new_adduct(TreeNode node)
        {
            adduct = new Adduct("", "");
        }
            
            

        public void add_adduct(TreeNode node)
        {
            adduct.adduct_string = node.get_text();
        }
            
            

        public void add_charge(TreeNode node)
        {
            adduct.charge = Convert.ToInt32(node.get_text());
        }
            
            

        public void add_charge_sign(TreeNode node)
        {
            string sign = node.get_text();
            if (sign.Equals("+")) adduct.set_charge_sign(1);
            else if (sign.Equals("-")) adduct.set_charge_sign(-1);
            if (adduct.charge == 0) adduct.charge = 1;
        }
        
        
        
        public void set_heavy_element(TreeNode node)
        {
            heavy_element = Elements.heavy_element_table[node.get_text()];
            heavy_element_number = 1;
        }
                
                
        public void set_heavy_number(TreeNode node)
        {
            heavy_element_number = node.get_int();
        }
                
                
        public void add_heavy_component(TreeNode node)
        {
            adduct.heavy_elements[heavy_element] += heavy_element_number;
        }
    }
}
