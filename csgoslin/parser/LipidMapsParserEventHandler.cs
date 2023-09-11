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
        
    
    public class LipidMapsParserEventHandler : LipidBaseParserEventHandler
    {
        public bool omit_fa;
        public int db_numbers;
        public int db_position;
        public string db_cistrans;
        public string mod_text;
        public int mod_pos;
        public int mod_num;
        public bool add_omega_linoleoyloxy_Cer;
        public int heavy_number;
        public Element heavy_element;
        public bool sphinga_pure;
        public int lcb_carbon_pre_set;
        public int lcb_db_pre_set;
        public List<FunctionalGroup> lcb_hydro_pre_set = new List<FunctionalGroup>();
        public string sphinga_prefix = "";
        public string sphinga_suffix = "";
        
        public static readonly HashSet<string> head_group_exceptions = new HashSet<string>{"PA", "PC", "PE", "PG", "PI", "PS"};
        public static readonly Dictionary<string, int> acer_heads = new Dictionary<string, int>{{"1-O-myristoyl", 14}, {"1-O-palmitoyl", 16}, {"1-O-stearoyl", 18}, {"1-O-eicosanoyl", 20}, {"1-O-behenoyl", 22}, {"1-O-lignoceroyl", 24}, {"1-O-cerotoyl", 26}, {"1-O-pentacosanoyl", 25},    {"1-O-carboceroyl", 28}, {"1-O-tricosanoyl", 30}, {"1-O-lignoceroyl-omega-linoleoyloxy", 24}, {"1-O-stearoyl-omega-linoleoyloxy", 18}};

    
        public LipidMapsParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);
            registered_events.Add("mediator_pre_event", mediator_event);
            registered_events.Add("sgl_species_pre_event", set_species_level);
            registered_events.Add("species_fa_pre_event", set_species_level);
            registered_events.Add("tgl_species_pre_event", set_species_level);
            registered_events.Add("dpl_species_pre_event", set_species_level);
            registered_events.Add("cl_species_pre_event", set_species_level);
            registered_events.Add("dsl_species_pre_event", set_species_level);
            registered_events.Add("fa2_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa3_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa4_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("hg_dg_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa_lpl_molecular_pre_event", set_molecular_subspecies_level);
            registered_events.Add("hg_lbpa_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa_no_hg_pre_event", pure_fa);
            registered_events.Add("additional_modifier_pre_event", add_additional_modifier);
            registered_events.Add("hg_sgl_pre_event", set_head_group_name);
            registered_events.Add("hg_gl_pre_event", set_head_group_name);
            registered_events.Add("hg_cl_pre_event", set_head_group_name);
            registered_events.Add("hg_dpl_pre_event", set_head_group_name);
            registered_events.Add("hg_lpl_pre_event", set_head_group_name);
            registered_events.Add("hg_threepl_pre_event", set_head_group_name);
            registered_events.Add("hg_fourpl_pre_event", set_head_group_name);
            registered_events.Add("hg_dsl_pre_event", set_head_group_name);
            registered_events.Add("hg_cpa_pre_event", set_head_group_name);
            registered_events.Add("ch_pre_event", set_head_group_name);
            registered_events.Add("hg_che_pre_event", set_head_group_name);
            registered_events.Add("mediator_const_pre_event", set_head_group_name);
            registered_events.Add("pk_hg_pre_event", set_head_group_name);
            registered_events.Add("hg_fa_pre_event", set_head_group_name);
            registered_events.Add("hg_lsl_pre_event", set_head_group_name);
            registered_events.Add("special_cer_pre_event", set_head_group_name);
            registered_events.Add("special_cer_hg_pre_event", set_head_group_name);
            registered_events.Add("omega_linoleoyloxy_Cer_pre_event", set_omega_head_group_name);
            registered_events.Add("lcb_pre_event", new_lcb);
            registered_events.Add("lcb_post_event", clean_lcb);
            registered_events.Add("fa_pre_event", new_fa);
            registered_events.Add("fa_post_event", append_fa);
            registered_events.Add("glyco_struct_pre_event", add_glyco);
            registered_events.Add("db_single_position_pre_event", set_isomeric_level);
            registered_events.Add("db_single_position_post_event", add_db_position);
            registered_events.Add("db_position_number_pre_event", add_db_position_number);
            registered_events.Add("cistrans_pre_event", add_cistrans);
            registered_events.Add("ether_prefix_pre_event", add_ether);
            registered_events.Add("ether_suffix_pre_event", add_ether);
            registered_events.Add("hydroxyl_pre_event", add_hydroxyl);
            registered_events.Add("lcb_pure_fa_pre_event", add_dihydroxyl);
            registered_events.Add("hydroxyl_lcb_pre_event", add_hydroxyl_lcb);
            registered_events.Add("db_count_pre_event", add_double_bonds);
            registered_events.Add("carbon_pre_event", add_carbon);
            registered_events.Add("structural_mod_pre_event", set_structural_subspecies_level);
            registered_events.Add("single_mod_pre_event", set_mod);
            registered_events.Add("mod_text_pre_event", set_mod_text);
            registered_events.Add("mod_pos_pre_event", set_mod_pos);
            registered_events.Add("mod_num_pre_event", set_mod_num);
            registered_events.Add("single_mod_post_event", add_functional_group);
            registered_events.Add("special_cer_prefix_pre_event", add_ACer);
            registered_events.Add("adduct_info_pre_event", new_adduct);
            registered_events.Add("adduct_pre_event", add_adduct);
            registered_events.Add("charge_pre_event", add_charge);
            registered_events.Add("charge_sign_pre_event", add_charge_sign);
            registered_events.Add("isotope_pair_pre_event", new_adduct);
            registered_events.Add("isotope_element_pre_event", set_heavy_element);
            registered_events.Add("isotope_number_pre_event", set_heavy_number);
            registered_events.Add("sphinga_pre_event", new_sphinga);
            registered_events.Add("sphinga_phospho_pre_event", add_phospho);
            registered_events.Add("sphinga_suffix_pre_event", sphinga_db_set);
            registered_events.Add("sphinga_lcb_len_pre_event", add_carbon_pre_len);
            registered_events.Add("sphinga_prefix_pre_event", set_hydro_pre_num);
            registered_events.Add("sphinga_hg_pure_pre_event", new_sphinga_pure);
            registered_events.Add("sphinga_hg_pure_post_event", clean_lcb);
    
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
            use_head_group = false;
            omit_fa = false;
            db_position = 0;
            db_numbers = -1;
            adduct = null;
            db_cistrans = "";
            mod_pos = -1;
            mod_num = 1;
            mod_text = "";
            headgroup_decorators = new List<HeadgroupDecorator>();
            add_omega_linoleoyloxy_Cer = false;
            heavy_number = 0;
            heavy_element = Element.C;
            sphinga_pure = false;
            lcb_carbon_pre_set = 18;
            lcb_db_pre_set = 0;
            lcb_hydro_pre_set.Clear();
            sphinga_prefix = "";
            sphinga_suffix = "";
        }

        
        public void set_molecular_subspecies_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.MOLECULAR_SPECIES);
        }
        

        public void pure_fa(TreeNode node)
        {
            head_group = "FA";
        }



        public void set_heavy_element(TreeNode node)
        {
            adduct.heavy_elements[Element.H2] = 0;
        }



        public void add_additional_modifier(TreeNode node)
        {
            string modifier = node.get_text();
            if (modifier.Equals("h"))
            {
                FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
                string fg_name = functional_group.name;
                if (!current_fa.functional_groups.ContainsKey(fg_name)) current_fa.functional_groups.Add(fg_name, new List<FunctionalGroup>());
                current_fa.functional_groups[fg_name].Add(functional_group);
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            }
        }


        public void add_carbon_pre_len(TreeNode node)
        {
                lcb_carbon_pre_set = node.get_int();
        }

        
        public void sphinga_db_set(TreeNode node)
        {
                sphinga_suffix = node.get_text();
                
                if (sphinga_suffix.Equals("anine")) lcb_db_pre_set = 0;
                else if (sphinga_suffix.Equals("osine")) lcb_db_pre_set = 1;
                else if (sphinga_suffix.Equals("adienine")) lcb_db_pre_set = 2;
        }
        
        
        
        
        public void new_sphinga(TreeNode node)
        {
                head_group = "SPB";
        }
        
        
        
        public void new_sphinga_pure(TreeNode node)
        {
                sphinga_pure = true;
                lcb_hydro_pre_set.Add(KnownFunctionalGroups.get_functional_group("OH"));
                lcb_hydro_pre_set.Add(KnownFunctionalGroups.get_functional_group("OH"));
                lcb_hydro_pre_set[0].position = 1;
                lcb_hydro_pre_set[1].position = 3;
                new_lcb(node);
        }
        
        
        
        public void set_hydro_pre_num(TreeNode node)
        {
                lcb_hydro_pre_set.Add(KnownFunctionalGroups.get_functional_group("OH"));
                lcb_hydro_pre_set[lcb_hydro_pre_set.Count - 1].position = 4;
                sphinga_prefix = node.get_text();
        }
        
        
        
        public void add_phospho(TreeNode node)
        {
            string phospho_suffix = node.get_text();
            if (phospho_suffix.Equals("1-phosphate"))
            {
                head_group += "P";
            }
            else if (phospho_suffix.Equals("1-phosphocholine"))
            {
                head_group = "LSM";
            }
            lcb_hydro_pre_set.RemoveAt(0);
        }



        public void set_heavy_number(TreeNode node)
        {
            adduct.heavy_elements[Element.H2] = node.get_int();
        }

            
        public void mediator_event(TreeNode node)
        {
            use_head_group = true;
            head_group = node.get_text();
        }


        public void set_isomeric_level(TreeNode node)
        {
            db_position = 0;
            db_cistrans = "";
        }
    
    
        public void add_ACer(TreeNode node)
        {
            string head = node.get_text();
            head_group = "ACer";
            
            if (!acer_heads.ContainsKey(head))
            {
                throw new LipidException("ACer head group '" + head + "' unknown");
            }
            
            HeadgroupDecorator hgd = new HeadgroupDecorator("decorator_acyl", -1, 1, null, true);
            int acer_num = acer_heads[head];
            hgd.functional_groups.Add("decorator_acyl", new List<FunctionalGroup>(){new FattyAcid("FA", acer_num)});
            headgroup_decorators.Add(hgd);
            
            if (head.Equals("1-O-lignoceroyl-omega-linoleoyloxy") || head.Equals("1-O-stearoyl-omega-linoleoyloxy"))
            {
                add_omega_linoleoyloxy_Cer = true;
            }
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
            db_position = node.get_int();
        }


        public void add_cistrans(TreeNode node)
        {
            db_cistrans = node.get_text();
        }
            
            
        public void set_head_group_name(TreeNode node)
        {
            if (head_group.Length == 0) head_group = node.get_text();
        }
        
        
        public void set_omega_head_group_name(TreeNode node)
        {
            add_omega_linoleoyloxy_Cer = true;
            set_head_group_name(node);
        }
            
            
        public void set_species_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
        }
        
        
        public void set_structural_subspecies_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
        }


        public void set_mod(TreeNode node)
        {
            mod_text = "";
            mod_pos = -1;
            mod_num = 1;
        }


        public void set_mod_text(TreeNode node)
        {
            mod_text = node.get_text();
        }


        public void set_mod_pos(TreeNode node)
        {
            mod_pos = node.get_int();
        }


        public void set_mod_num(TreeNode node)
        {
            mod_num = node.get_int();
        } 
            
            
        public void add_functional_group(TreeNode node)
        {
            if (!mod_text.Equals("Cp"))
            {
                if (LipidEnum.LCB_STATES.Contains(current_fa.lipid_FA_bond_type) && mod_text.Equals("OH") && current_fa.functional_groups.ContainsKey("OH") && current_fa.functional_groups["OH"].Count > 0){
                    current_fa.functional_groups["OH"][current_fa.functional_groups["OH"].Count - 1].position = mod_pos;
                }
                else
                {
                    FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group(mod_text);
                    functional_group.position = mod_pos;
                    functional_group.count = mod_num;
                    string fg_name = functional_group.name;
                    if (!current_fa.functional_groups.ContainsKey(fg_name)) current_fa.functional_groups.Add(fg_name, new List<FunctionalGroup>());
                    current_fa.functional_groups[fg_name].Add(functional_group);
                }
            }
            else
            {
                current_fa.num_carbon += 1;
                Cycle cycle = new Cycle(3, mod_pos, mod_pos + 2);
                if (!current_fa.functional_groups.ContainsKey("cy")) current_fa.functional_groups.Add("cy", new List<FunctionalGroup>());
                current_fa.functional_groups["cy"].Add(cycle);
            }
        }
        
        
        public void add_glyco(TreeNode node)
        {
            string glyco_name = node.get_text();
            HeadgroupDecorator functional_group = null;
            try
            {
                functional_group = (HeadgroupDecorator)KnownFunctionalGroups.get_functional_group(glyco_name);
            }
            catch
            {
                throw new LipidParsingException("Carbohydrate '" + glyco_name + "' unknown");
            }
            
            functional_group.elements[Element.O] -= 1;
            headgroup_decorators.Add(functional_group);
        }
                
                
        public void new_fa(TreeNode node)
        {
            db_numbers = -1;
            current_fa = new FattyAcid("FA");
        }
            
            

        public void new_lcb(TreeNode node)
        {
            lcb = new FattyAcid("LCB");
            lcb.set_type(LipidFaBondType.LCB_REGULAR);
            current_fa = lcb;
        }
                
                

        public void clean_lcb(TreeNode node)
        {
            if (sphinga_pure)
            {
                lcb.num_carbon = lcb_carbon_pre_set;
                lcb.double_bonds.num_double_bonds = lcb_db_pre_set;
                current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
                foreach (FunctionalGroup fg in lcb_hydro_pre_set) current_fa.functional_groups["OH"].Add(fg);
            }
            
            if (!sphinga_suffix.Equals(""))
            {
                if ((sphinga_suffix.Equals("anine") && lcb.double_bonds.get_num() != 0) || (sphinga_suffix.Equals("osine") && lcb.double_bonds.get_num() != 1) || (sphinga_suffix.Equals("adienine") && lcb.double_bonds.get_num() != 2))
                {
                    throw new LipidException("Double bond count does not match with head group description");
                }
            }
                
            if (sphinga_prefix.Equals("Phyto") && !sphinga_pure)
            {
                HashSet<int> pos_hydro = new HashSet<int>();
                foreach (FunctionalGroup fg in lcb.functional_groups["OH"]) pos_hydro.Add(fg.position);
                if (lcb.functional_groups.Count == 0 || !lcb.functional_groups.ContainsKey("OH") || !pos_hydro.Contains(4))
                {
                    throw new LipidException("hydroxyl count does not match with head group description");
                }
            }

            if (db_numbers > -1 && db_numbers != current_fa.double_bonds.get_num())
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                set_lipid_level(LipidLevel.SN_POSITION);
            }
            if (current_fa.functional_groups.ContainsKey("OH"))
            {
                foreach (FunctionalGroup fg in current_fa.functional_groups["OH"])
                {
                    if (fg.position < 1)
                    {
                        set_structural_subspecies_level(node);
                        break;
                    }
                }
            }
            current_fa = null;
        }
            
        
        public void append_fa(TreeNode node)
        {
            if (db_numbers > -1 && db_numbers != current_fa.double_bonds.get_num())
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                set_lipid_level(LipidLevel.SN_POSITION);
            }
            
            if (current_fa.num_carbon == 0)
            {
                omit_fa = true;
            }
            fa_list.Add(current_fa);
            current_fa = null;
        }
            
            
        public void add_ether(TreeNode node)
        {
            string ether = node.get_text();
            if (ether.Equals("O-") || ether.Equals("e")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether.Equals("P-") || ether.Equals("p")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
        }
            
            
            
        public void add_hydroxyl(TreeNode node)
        {
            int num_h = node.get_int();
            
            if (sp_regular_lcb()) num_h -= 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
        }    
    
    
        public void add_dihydroxyl(TreeNode node)
        {
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            
            FunctionalGroup functional_group_p3 = KnownFunctionalGroups.get_functional_group("OH");
            functional_group_p3.position = 3;
            current_fa.functional_groups["OH"].Add(functional_group_p3);

            if (!sp_regular_lcb())
            {
                FunctionalGroup functional_group_p1 = KnownFunctionalGroups.get_functional_group("OH");
                functional_group_p1.position = 1;
                current_fa.functional_groups["OH"].Add(functional_group_p1);
            }
        }


            
        public void add_hydroxyl_lcb(TreeNode node)
        {
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
    
            string hydroxyl = node.get_text();
            if (hydroxyl.Equals("m"))
            {
                FunctionalGroup functional_group_p3 = KnownFunctionalGroups.get_functional_group("OH");
                functional_group_p3.position = 3;
                current_fa.functional_groups["OH"].Add(functional_group_p3);
            }
            else if (hydroxyl.Equals("d"))
            {
                if (!sp_regular_lcb())
                {
                    FunctionalGroup functional_group_p1 = KnownFunctionalGroups.get_functional_group("OH");
                    functional_group_p1.position = 1;
                    current_fa.functional_groups["OH"].Add(functional_group_p1);
                }
                
                FunctionalGroup functional_group_p3 = KnownFunctionalGroups.get_functional_group("OH");
                functional_group_p3.position = 3;
                current_fa.functional_groups["OH"].Add(functional_group_p3);
            }
            else if (hydroxyl.Equals("t"))
            {
                if (!sp_regular_lcb())
                {
                    FunctionalGroup functional_group_p1 = KnownFunctionalGroups.get_functional_group("OH");
                    functional_group_p1.position = 1;
                    current_fa.functional_groups["OH"].Add(functional_group_p1);
                }
                
                FunctionalGroup functional_group_p3 = KnownFunctionalGroups.get_functional_group("OH");
                functional_group_p3.position = 3;
                current_fa.functional_groups["OH"].Add(functional_group_p3);
                
                FunctionalGroup functional_group_t = KnownFunctionalGroups.get_functional_group("OH");
                functional_group_t.position = 4;
                current_fa.functional_groups["OH"].Add(functional_group_t);
            }
        }
            
            
        public void add_double_bonds(TreeNode node)
        {
            current_fa.double_bonds.num_double_bonds += node.get_int();
        }
            
            
        public void add_carbon(TreeNode node)
        {
            current_fa.num_carbon = node.get_int();
        }
            
            

        public void build_lipid(TreeNode node)
        {
            if (omit_fa && head_group_exceptions.Contains(head_group))
            {
                head_group = "L" + head_group;
            }
            
            if (lcb != null)
            {
                fa_list.Insert(0, lcb);
            }
    
            if (add_omega_linoleoyloxy_Cer)
            {
                if (fa_list.Count != 2)
                {
                    throw new LipidException("omega-linoleoyloxy-Cer with a different combination to one long chain base and one fatty acyl chain unknown");
                }
                if (!fa_list[fa_list.Count - 1].functional_groups.ContainsKey("acyl")) fa_list[fa_list.Count - 1].functional_groups.Add("acyl", new List<FunctionalGroup>());
                
                DoubleBonds db = new DoubleBonds(2);
                db.double_bond_positions.Add(9, "Z");
                db.double_bond_positions.Add(12, "Z");
                fa_list[fa_list.Count - 1].functional_groups["acyl"].Add(new AcylAlkylGroup(new FattyAcid("FA", 18, db)));
                head_group = "Cer";
            }
            
            
            Headgroup headgroup = prepare_headgroup_and_checks();
            
            LipidAdduct lipid = new LipidAdduct();
            lipid.lipid = assemble_lipid(headgroup);
            lipid.adduct = adduct;
            content = lipid;
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
            adduct.charge = node.get_int();
        }
            
            

        public void add_charge_sign(TreeNode node)
        {
            string sign = node.get_text();
            if (sign.Equals("+")) adduct.set_charge_sign(1);
            else if (sign.Equals("-")) adduct.set_charge_sign(-1);
            if (adduct.charge == 0) adduct.charge = 1;
        }
    }
}
