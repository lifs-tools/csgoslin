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
        
    
    public class HmdbParserEventHandler : LipidBaseParserEventHandler
    {
        
        public int db_position;
        public string db_cistrans;
        public Dict furan = new Dict();
        public string func_type;
    
        public HmdbParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);
            registered_events.Add("fa_hg_pre_event", set_head_group_name);
            registered_events.Add("gl_hg_pre_event", set_head_group_name);
            registered_events.Add("gl_molecular_hg_pre_event", set_head_group_name);
            registered_events.Add("mediator_pre_event", mediator_event);
            registered_events.Add("gl_mono_hg_pre_event", set_head_group_name);
            registered_events.Add("pl_hg_pre_event", set_head_group_name);
            registered_events.Add("pl_three_hg_pre_event", set_head_group_name);
            registered_events.Add("pl_four_hg_pre_event", set_head_group_name);
            registered_events.Add("sl_hg_pre_event", set_head_group_name);
            registered_events.Add("st_species_hg_pre_event", set_head_group_name);
            registered_events.Add("st_sub1_hg_pre_event", set_head_group_name);
            registered_events.Add("st_sub2_hg_pre_event", set_head_group_name);
            registered_events.Add("ganglioside_names_pre_event", set_head_group_name);
            registered_events.Add("fa_species_pre_event", set_species_level);
            registered_events.Add("gl_molecular_pre_event", set_molecular_level);
            registered_events.Add("unsorted_fa_separator_pre_event", set_molecular_level);
            registered_events.Add("fa2_unsorted_pre_event", set_molecular_level);
            registered_events.Add("fa3_unsorted_pre_event", set_molecular_level);
            registered_events.Add("fa4_unsorted_pre_event", set_molecular_level);
            registered_events.Add("db_single_position_pre_event", set_isomeric_level);
            registered_events.Add("db_single_position_post_event", add_db_position);
            registered_events.Add("db_position_number_pre_event", add_db_position_number);
            registered_events.Add("cistrans_pre_event", add_cistrans);
            registered_events.Add("lcb_pre_event", new_lcb);
            registered_events.Add("lcb_post_event", clean_lcb);
            registered_events.Add("fa_pre_event", new_fa);
            registered_events.Add("fa_post_event", append_fa);
            registered_events.Add("ether_pre_event", add_ether);
            registered_events.Add("hydroxyl_pre_event", add_hydroxyl);
            registered_events.Add("db_count_pre_event", add_double_bonds);
            registered_events.Add("carbon_pre_event", add_carbon);
            registered_events.Add("fa_lcb_suffix_type_pre_event", add_one_hydroxyl);
            registered_events.Add("interlink_fa_pre_event", interlink_fa);
            registered_events.Add("lipid_suffix_pre_event", lipid_suffix);
            registered_events.Add("methyl_pre_event", add_methyl);
            registered_events.Add("furan_fa_pre_event", furan_fa);
            registered_events.Add("furan_fa_post_event", furan_fa_post);
            registered_events.Add("furan_fa_mono_pre_event", furan_fa_mono);
            registered_events.Add("furan_fa_di_pre_event", furan_fa_di);
            registered_events.Add("furan_first_number_pre_event", furan_fa_first_number);
            registered_events.Add("furan_second_number_pre_event", furan_fa_second_number);
            registered_events.Add("adduct_info_pre_event", new_adduct);
            registered_events.Add("adduct_pre_event", add_adduct);
            registered_events.Add("charge_pre_event", add_charge);
            registered_events.Add("charge_sign_pre_event", add_charge_sign);
            registered_events.Add("fa_lcb_suffix_types_pre_event", register_suffix_type);
            registered_events.Add("fa_lcb_suffix_position_pre_event", register_suffix_pos);
            registered_events.Add("fa_synonym_pre_event", register_fa_synonym);
            
            
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
            db_position = 0;
            db_cistrans = "";
            adduct = null;
            furan = new Dict();
            headgroup_decorators = new ExendedList<HeadgroupDecorator>();
            func_type = "";
        }


        public void register_suffix_type(TreeNode node)
        {
                func_type = node.get_text();
                if (!func_type.Equals("me") && !func_type.Equals("OH") && !func_type.Equals("O"))
                {
                    throw new LipidException("Unknown functional abbreviation: " + func_type);
                }
                if (func_type.Equals("me")) func_type = "Me";
                else if (func_type.Equals("O")) func_type = "oxo";
        }
            
            
            
        public void register_suffix_pos(TreeNode node)
        {
                int pos = node.get_int();
                FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group(func_type);
                functional_group.position = pos;
                if (!current_fa.functional_groups.ContainsKey(func_type)) current_fa.functional_groups.Add(func_type, new List<FunctionalGroup>());
                current_fa.functional_groups[func_type].Add(functional_group);
        }



        public void register_fa_synonym(TreeNode node)
        {
            current_fa = resolve_fa_synonym(node.get_text());
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
            db_position = node.get_int();
        }


        public void add_cistrans(TreeNode node)
        {
            db_cistrans = node.get_text();
        }


        public void set_head_group_name(TreeNode node)
        {
            head_group = node.get_text();
        }


        public void set_species_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
        }
            



        public void set_molecular_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.MOLECULAR_SPECIES);
        }


        public void mediator_event(TreeNode node)
        {
            use_head_group = true;
            head_group = node.get_text();
        }
            
            

        public void new_fa(TreeNode node)
        {
            current_fa = new FattyAcid("FA");
        }
            
            

        public void new_lcb(TreeNode node)
        {
            lcb = new FattyAcid("LCB");
            lcb.set_type(LipidFaBondType.LCB_REGULAR);
            current_fa = lcb;
            set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
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
            if (current_fa.double_bonds.get_num() < 0)
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                set_lipid_level(LipidLevel.SN_POSITION);
            }

            fa_list.Add(current_fa);
            current_fa = null;
        }
            
            

        public void build_lipid(TreeNode node)
        {
            if (lcb != null)
            {
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
                fa_list.Insert(0, lcb);
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
            if (ether.Equals("O-") || ether.Equals("o-")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether.Equals("P-")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
            else throw new UnsupportedLipidException("Fatty acyl chain of type '" + ether + "' is currently not supported");
        }
    
    

        public void add_methyl(TreeNode node)
        {
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("Me");
            functional_group.position = current_fa.num_carbon - (node.get_text() == "i-" ? 1 : 2);
            current_fa.num_carbon -= 1;
            if (!current_fa.functional_groups.ContainsKey("Me")) current_fa.functional_groups.Add("Me", new List<FunctionalGroup>());
            current_fa.functional_groups["Me"].Add(functional_group);
        }
            
            

        public void add_hydroxyl(TreeNode node)
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


        public void add_one_hydroxyl(TreeNode node)
        {
            if (current_fa.functional_groups.ContainsKey("OH") && current_fa.functional_groups["OH"][0].position == -1)
            {
                current_fa.functional_groups["OH"][0].count += 1;
            }
            else {
                FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
                if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
                current_fa.functional_groups["OH"].Add(functional_group);
            }
        }
            

        public void add_double_bonds(TreeNode node)
        {
            current_fa.double_bonds.num_double_bonds = node.get_int();
        }
            
            

        public void add_carbon(TreeNode node)
        {
            current_fa.num_carbon += node.get_int();
        }
            

        public void furan_fa(TreeNode node)
        {
            furan = new Dict();
        }
        
        
        public void furan_fa_post(TreeNode node)
        {
            int l = 4 + (int)furan["len_first"] + (int)furan["len_second"];
            current_fa.num_carbon = l;
            
            int start = 1 + (int)furan["len_first"];
            int end = 3 + start;
            DoubleBonds cyclo_db = new DoubleBonds(2);
            cyclo_db.double_bond_positions.Add(start, "E");
            cyclo_db.double_bond_positions.Add(2 + start, "E");
            
            Dictionary<string, List<FunctionalGroup> > cyclo_fg = new Dictionary<string, List<FunctionalGroup> >();
            cyclo_fg.Add("Me", new List<FunctionalGroup>());
            
            if (((string)furan["type"]).Equals("m"))
            {
                FunctionalGroup fg = KnownFunctionalGroups.get_functional_group("Me");
                fg.position = 1 + start;
                cyclo_fg["Me"].Add(fg);
            }
                
            else if (((string)furan["type"]).Equals("d"))
            {
                FunctionalGroup fg = KnownFunctionalGroups.get_functional_group("Me");
                fg.position = 1 + start;
                cyclo_fg["Me"].Add(fg);
                fg = KnownFunctionalGroups.get_functional_group("Me");
                fg.position = 2 + start;
                cyclo_fg["Me"].Add(fg);
            }
            
            List<Element> bridge_chain = new List<Element>{Element.O};
            Cycle cycle = new Cycle(end - start + 1 + bridge_chain.Count, start, end, cyclo_db, cyclo_fg, bridge_chain);
            current_fa.functional_groups.Add("cy", new List<FunctionalGroup>(){cycle});
        }
        
        
        public void furan_fa_mono(TreeNode node)
        {
            furan.Add("type", "m");
        }
        
        
        public void furan_fa_di(TreeNode node)
        {
            furan.Add("type", "d");
        }
        
        
        public void furan_fa_first_number(TreeNode node)
        {
            furan.Add("len_first", node.get_int());
        }
         
         
        public void furan_fa_second_number(TreeNode node)
        {
            furan.Add("len_second", node.get_int());
        }
        
        
            

        public void interlink_fa(TreeNode node)
        {
            throw new UnsupportedLipidException("Interconnected fatty acyl chains are currently not supported");
        }
            

        public void lipid_suffix(TreeNode node)
        {
            //throw new UnsupportedLipidException("Lipids with suffix '" + node.get_text() + "' are currently not supported");
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
