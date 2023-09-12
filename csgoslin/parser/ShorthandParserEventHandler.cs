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
 
    
    
    public class ShorthandParserEventHandler : LipidBaseParserEventHandler
    {
        public ExendedList<FunctionalGroup> current_fas;
        public Dict tmp = new Dict();
        public bool acer_species = false;
        public static readonly HashSet<string> special_types = new HashSet<string>{"acyl", "alkyl", "decorator_acyl", "decorator_alkyl", "cc"};
        public bool contains_stereo_information;
        public Element heavy_element;
        public int heavy_element_number;
        
        
        public string FA_I()
        {
            return "fa" + Convert.ToString(current_fas.Count);
        }
        
        public ShorthandParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);
            
            // set adduct events
            registered_events.Add("adduct_info_pre_event", new_adduct);
            registered_events.Add("adduct_pre_event", add_adduct);
            registered_events.Add("charge_pre_event", add_charge);
            registered_events.Add("charge_sign_pre_event", add_charge_sign);
            
            // set species events
            registered_events.Add("med_species_pre_event", set_species_level);
            registered_events.Add("gl_species_pre_event", set_species_level);
            registered_events.Add("gl_molecular_species_pre_event", set_molecular_level);
            registered_events.Add("pl_species_pre_event", set_species_level);
            registered_events.Add("pl_molecular_species_pre_event", set_molecular_level);
            registered_events.Add("sl_species_pre_event", set_species_level);
            registered_events.Add("pl_single_pre_event", set_molecular_level);
            registered_events.Add("unsorted_fa_separator_pre_event", set_molecular_level);
            registered_events.Add("ether_num_pre_event", set_ether_num);
            
            // set head groups events
            registered_events.Add("med_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("med_hg_double_pre_event", set_headgroup_name);
            registered_events.Add("med_hg_triple_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_double_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_glycosyl_single_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_glycosyl_double_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_triple_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_double_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_quadro_pre_event", set_headgroup_name);
            registered_events.Add("sl_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("sl_hg_glyco_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_double_fa_hg_pre_event", set_headgroup_name);
            registered_events.Add("sl_hg_double_name_pre_event", set_headgroup_name);
            registered_events.Add("st_hg_pre_event", set_headgroup_name);
            registered_events.Add("st_hg_ester_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_m_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_d_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_t_pre_event", set_headgroup_name);
            registered_events.Add("hg_PE_PS_pre_event", set_headgroup_name);
            registered_events.Add("glyco_sphingo_lipid_pre_event", set_glyco_sphingo_lipid);
            registered_events.Add("carbohydrate_number_pre_event", set_carbohydrate_number);

            // set head group headgroup_decorators
            registered_events.Add("carbohydrate_sn_pre_event", set_carbohydrate);
            registered_events.Add("carbohydrate_iso_pre_event", set_carbohydrate);
            registered_events.Add("carbohydrate_sn_position_pre_event", set_carbohydrate_sn_position);
            registered_events.Add("carbohydrate_isomeric_pre_event", set_carbohydrate_isomeric);
            
            // fatty acyl events
            registered_events.Add("lcb_pre_event", new_lcb);
            registered_events.Add("lcb_post_event", add_fatty_acyl_chain);
            registered_events.Add("fatty_acyl_chain_pre_event", new_fatty_acyl_chain);
            registered_events.Add("fatty_acyl_chain_post_event", add_fatty_acyl_chain);
            registered_events.Add("carbon_pre_event", set_carbon);
            registered_events.Add("db_count_pre_event", set_double_bond_count);
            registered_events.Add("db_position_number_pre_event", set_double_bond_position);
            registered_events.Add("db_single_position_pre_event", set_double_bond_information);
            registered_events.Add("db_single_position_post_event", add_double_bond_information);
            registered_events.Add("cistrans_pre_event", set_cistrans);
            registered_events.Add("ether_type_pre_event", set_ether_type);
            registered_events.Add("stereo_type_fa_pre_event", set_fatty_acyl_stereo);
            
            // set functional group events
            registered_events.Add("func_group_data_pre_event", set_functional_group);
            registered_events.Add("func_group_data_post_event", add_functional_group);
            registered_events.Add("func_group_pos_number_pre_event", set_functional_group_position);
            registered_events.Add("func_group_name_pre_event", set_functional_group_name);
            registered_events.Add("func_group_count_pre_event", set_functional_group_count);
            registered_events.Add("stereo_type_fg_pre_event", set_functional_group_stereo);
            registered_events.Add("molecular_func_group_name_pre_event", set_sn_position_func_group);
            registered_events.Add("fa_db_only_post_event", add_dihydroxyl);
            
            // set cycle events
            registered_events.Add("func_group_cycle_pre_event", set_cycle);
            registered_events.Add("func_group_cycle_post_event", add_cycle);
            registered_events.Add("cycle_start_pre_event", set_cycle_start);
            registered_events.Add("cycle_end_pre_event", set_cycle_end);
            registered_events.Add("cycle_number_pre_event", set_cycle_number);
            registered_events.Add("cycle_db_cnt_pre_event", set_cycle_db_count);
            registered_events.Add("cycle_db_positions_pre_event", set_cycle_db_positions);
            registered_events.Add("cycle_db_positions_post_event", check_cycle_db_positions);
            registered_events.Add("cycle_db_position_number_pre_event", set_cycle_db_position);
            registered_events.Add("cycle_db_position_cis_trans_pre_event", set_cycle_db_position_cistrans);
            registered_events.Add("cylce_element_pre_event", add_cycle_element);
            
            // set linkage events
            registered_events.Add("fatty_acyl_linkage_pre_event", set_acyl_linkage);
            registered_events.Add("fatty_acyl_linkage_post_event", add_acyl_linkage);
            registered_events.Add("fatty_alkyl_linkage_pre_event", set_alkyl_linkage);
            registered_events.Add("fatty_alkyl_linkage_post_event", add_alkyl_linkage);
            registered_events.Add("fatty_linkage_number_pre_event", set_fatty_linkage_number);
            registered_events.Add("fatty_acyl_linkage_sign_pre_event", set_linkage_type);
            registered_events.Add("hydrocarbon_chain_pre_event", set_hydrocarbon_chain);
            registered_events.Add("hydrocarbon_chain_post_event", add_hydrocarbon_chain);
            registered_events.Add("hydrocarbon_number_pre_event", set_fatty_linkage_number);
            
            // set remaining events
            registered_events.Add("ring_stereo_pre_event", set_ring_stereo);
            registered_events.Add("pl_hg_fa_pre_event", set_hg_acyl);
            registered_events.Add("pl_hg_fa_post_event", add_hg_acyl);
            registered_events.Add("pl_hg_alk_pre_event", set_hg_alkyl);
            registered_events.Add("pl_hg_alk_post_event", add_hg_alkyl);
            registered_events.Add("pl_hg_species_pre_event", add_pl_species_data);
            registered_events.Add("hg_pip_m_pre_event", suffix_decorator_molecular);
            registered_events.Add("hg_pip_d_pre_event", suffix_decorator_molecular);
            registered_events.Add("hg_pip_t_pre_event", suffix_decorator_molecular);
            registered_events.Add("hg_PE_PS_type_pre_event", suffix_decorator_species);
            registered_events.Add("acer_hg_post_event", set_acer);
            registered_events.Add("acer_species_post_event", set_acer_species);
            
            registered_events.Add("sterol_definition_post_event", set_sterol_definition);
            registered_events.Add("adduct_heavy_element_pre_event", set_heavy_element);
            registered_events.Add("adduct_heavy_number_pre_event", set_heavy_number);
            registered_events.Add("adduct_heavy_component_post_event", add_heavy_component);
     
            debug = "";
        }
        
    
        public void reset_parser(TreeNode node)
        {
            content = null;
            level = LipidLevel.COMPLETE_STRUCTURE;
            adduct = null;
            head_group = "";
            fa_list = new ExendedList<FattyAcid>();
            current_fas = new ExendedList<FunctionalGroup>();
            headgroup_decorators = new ExendedList<HeadgroupDecorator>();
            tmp = new Dict();
            acer_species = false;
            contains_stereo_information = false;
            heavy_element = Element.C;
            heavy_element_number = 0;
        }


        public void set_sterol_definition(TreeNode node)
        {
            head_group += " " + node.get_text();
            fa_list.RemoveAt(0);
        }



        public void set_carbohydrate_number(TreeNode node)
        {
            int carbohydrate_num = node.get_int();
            if (headgroup_decorators.Count > 0 && carbohydrate_num > 0)
            {
                headgroup_decorators[headgroup_decorators.Count - 1].count += (carbohydrate_num - 1);
            }
        }
            
            

        public void set_glyco_sphingo_lipid(TreeNode node)
        {
            
        }


        public void build_lipid(TreeNode node)
        {
            if (acer_species) fa_list[0].num_carbon -= 2;
            Headgroup headgroup = prepare_headgroup_and_checks();

            // add count numbers for fatty acyl chains
            int fa_it = (fa_list.Count > 0 && (fa_list[0].lipid_FA_bond_type == LipidFaBondType.LCB_EXCEPTION || fa_list[0].lipid_FA_bond_type == LipidFaBondType.LCB_REGULAR)) ? 1 : 0;
            for (int it = fa_it; it < fa_list.Count; ++it)
            {
                fa_list[it].name += Convert.ToString(it + 1);
            }
            
            LipidAdduct lipid = new LipidAdduct();
            lipid.adduct = adduct;
            lipid.lipid = assemble_lipid(headgroup);
            
            if (tmp.ContainsKey("num_ethers")) lipid.lipid.info.num_ethers = (int)tmp["num_ethers"];
            
            content = lipid;
        }
        
        
        public void set_acer(TreeNode node)
        {
            head_group = "ACer";
            HeadgroupDecorator hgd = new HeadgroupDecorator("decorator_acyl", -1, 1, null, true);
            hgd.functional_groups.Add("decorator_acyl", new List<FunctionalGroup>{fa_list[fa_list.Count - 1]});
            fa_list.RemoveAt(fa_list.Count - 1);
            headgroup_decorators.Add(hgd);
        }
            
            
        public void set_acer_species(TreeNode node)
        {
            head_group = "ACer";
            set_lipid_level(LipidLevel.SPECIES);
            HeadgroupDecorator hgd = new HeadgroupDecorator("decorator_acyl", -1, 1, null, true);
            hgd.functional_groups.Add("decorator_acyl", new List<FunctionalGroup>{new FattyAcid("FA", 2)});
            headgroup_decorators.Add(hgd);
            acer_species = true;
        }


        public void add_cycle_element(TreeNode node)
        {
            string element = node.get_text();
            
            if (!Elements.element_positions.ContainsKey(element))
            {
                throw new LipidParsingException("Element '" + element + "' unknown");
            }
            
            ((Lst)((Dict)tmp[FA_I()])["cycle_elements"]).Add(Elements.element_positions[element]);
        }

        
        public void set_headgroup_name(TreeNode node)
        {
            if (head_group.Length == 0) head_group = node.get_text();
        }
        

        public void set_carbohydrate(TreeNode node)
        {
            string carbohydrate = node.get_text();
            FunctionalGroup functional_group = null;
            try
            {
                functional_group = KnownFunctionalGroups.get_functional_group(carbohydrate);
            }
            catch
            {
                throw new LipidParsingException("Carbohydrate '" + carbohydrate + "' unknown");
            }
            
            functional_group.elements[Element.O] -= 1;
            if (tmp.ContainsKey("func_group_head") && ((int)tmp["func_group_head"] == 1))
            {
                headgroup_decorators.Add((HeadgroupDecorator)functional_group);
            }
            else
            {
                if (!current_fas.back().functional_groups.ContainsKey(carbohydrate))
                {
                    current_fas.back().functional_groups.Add(carbohydrate, new List<FunctionalGroup>());
                }
                current_fas.back().functional_groups[carbohydrate].Add(functional_group);
            }
        }



        public void set_carbohydrate_sn_position(TreeNode node)
        {
            set_lipid_level(LipidLevel.SN_POSITION);
            tmp.Add("func_group_head", 1);
        }



        public void set_carbohydrate_isomeric(TreeNode node)
        {
            tmp.Add("func_group_head", 1);
        }



        public void suffix_decorator_molecular(TreeNode node)
        {
            headgroup_decorators.Add(new HeadgroupDecorator(node.get_text(), -1, 1, null, true, LipidLevel.MOLECULAR_SPECIES));
        }



        public void suffix_decorator_species(TreeNode node)
        {
            headgroup_decorators.Add(new HeadgroupDecorator(node.get_text(), -1, 1, null, true, LipidLevel.SPECIES));
        }



        public void set_pl_hg_triple(TreeNode node)
        {
                set_molecular_level(node);
                set_headgroup_name(node);
        }



        public void set_ring_stereo(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_ring_stereo", node.get_text());
        }

        
        public void set_fatty_acyl_stereo(TreeNode node)
        {
            current_fas.back().stereochemistry = node.get_text();
            contains_stereo_information = true;
        }
        
        
        public void add_dihydroxyl(TreeNode node)
        {
            if (!LipidEnum.LCB_STATES.Contains(((FattyAcid)current_fas.back()).lipid_FA_bond_type)) return;
            
            int num_h = 1;
            if (SP_EXCEPTION_CLASSES.Contains(head_group) && headgroup_decorators.Count == 0) num_h += 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fas.back().functional_groups.ContainsKey("OH")) current_fas.back().functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fas.back().functional_groups["OH"].Add(functional_group);
        }



        public void add_pl_species_data(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
            HeadgroupDecorator hgd = new HeadgroupDecorator("");
            hgd.elements[Element.O] += 1;
            hgd.elements[Element.H] -= 1;
            headgroup_decorators.Add(hgd);
        }



        public void new_fatty_acyl_chain(TreeNode node)
        {
            current_fas.Add(new FattyAcid("FA"));
            tmp.Add(FA_I(), new Dict());
        }



        public void new_lcb(TreeNode node)
        {
            new_fatty_acyl_chain(node);
            ((FattyAcid)current_fas[current_fas.Count - 1]).set_type(LipidFaBondType.LCB_REGULAR);
            current_fas[current_fas.Count - 1].name = "LCB";
        }
        
        
        
        public void add_fatty_acyl_chain(TreeNode node)
        {
            string fg_i = "fa" + Convert.ToString(current_fas.Count - 2);
            string special_type = "";
            if (current_fas.Count >= 2 && tmp.ContainsKey(fg_i) && ((Dict)tmp[fg_i]).ContainsKey("fg_name"))
            {
                string fg_name = (string)((Dict)tmp[fg_i])["fg_name"];
                if (special_types.Contains(fg_name))
                {
                    special_type = fg_name;
                }
            }
            
            string fa_i = FA_I();
            if (current_fas.back().double_bonds.get_num() != (int)((Dict)tmp[fa_i])["db_count"])
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            else if (current_fas.back().double_bonds.get_num() > 0 && current_fas.back().double_bonds.double_bond_positions.Count == 0)
            {
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            }
            tmp.Remove(fa_i);
            
            FattyAcid fa = (FattyAcid)current_fas.PopBack();
            if (special_type.Length > 0)
            {
                fa.name = special_type;
                if (!current_fas.back().functional_groups.ContainsKey(special_type))
                {
                    current_fas.back().functional_groups.Add(special_type, new List<FunctionalGroup>());
                }
                current_fas.back().functional_groups[special_type].Add(fa);
            }
            else
            {
                fa_list.Add(fa);
            }
        }



        public void set_double_bond_position(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("db_position", node.get_int());
        }



        public void set_double_bond_information(TreeNode node)
        {
            string fa_i = FA_I();
            ((Dict)tmp[fa_i]).Add("db_position", 0);
            ((Dict)tmp[fa_i]).Add("db_cistrans", "");
        }



        public void add_double_bond_information(TreeNode node)
        {
            string fa_i = FA_I();
            Dict d = (Dict)tmp[fa_i];
            int pos = (int)d["db_position"];
            string cistrans = (string)d["db_cistrans"];
            
            if (cistrans.Equals(""))
            {
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            }
            
            d.Remove("db_position");
            d.Remove("db_cistrans");
            current_fas.back().double_bonds.double_bond_positions.Add(pos, cistrans);
        }



        public void set_cistrans(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("db_cistrans", node.get_text());
        }



        public void set_functional_group(TreeNode node)
        {
            string fa_i = FA_I();
            Dict gd = (Dict)tmp[fa_i];
            gd.Add("fg_pos", -1);
            gd.Add("fg_name", "0");
            gd.Add("fg_cnt", 1);
            gd.Add("fg_stereo", "");
            gd.Add("fg_ring_stereo", "");
        }



        public void set_cycle(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "cy");
            current_fas.Add(new Cycle(0));
            
            string fa_i = FA_I();
            tmp.Add(fa_i, new Dict());
            ((Dict)tmp[fa_i]).Add("cycle_elements", new Lst());
        }



        public void add_cycle(TreeNode node)
        {
            string fa_i = FA_I();
            Lst cycle_elements = (Lst)((Dict)tmp[fa_i])["cycle_elements"];
            Cycle cycle = (Cycle)current_fas.PopBack();
            for (int i = 0; i < cycle_elements.Count; ++i)
            {
                cycle.bridge_chain.Add((Element)cycle_elements[i]);
            }
            ((Dict)tmp[fa_i]).Remove("cycle_elements");
                
            if (cycle.start > -1 && cycle.end > -1 && cycle.end - cycle.start + 1 + cycle.bridge_chain.Count < cycle.cycle)
            {
                throw new ConstraintViolationException("Cycle length '" + Convert.ToString(cycle.cycle) + "' does not match with cycle description.");
            }
            if (!current_fas.back().functional_groups.ContainsKey("cy"))
            {
                current_fas.back().functional_groups.Add("cy", new List<FunctionalGroup>());
            }
            current_fas.back().functional_groups["cy"].Add(cycle);
        }



        public void set_fatty_linkage_number(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("linkage_pos", node.get_int());
        }



        public void set_hg_acyl(TreeNode node)
        {
            string fa_i = FA_I();
            tmp.Add(fa_i, new Dict());
            ((Dict)tmp[fa_i]).Add("fg_name", "decorator_acyl");
            current_fas.Add(new HeadgroupDecorator("decorator_acyl", -1, 1, null, true));
            tmp.Add(FA_I(), new Dict());
        }



        public void add_hg_acyl(TreeNode node)
        {
            tmp.Remove(FA_I());
            headgroup_decorators.Add((HeadgroupDecorator)current_fas.PopBack());
            tmp.Remove(FA_I());
        }



        public void set_hg_alkyl(TreeNode node)
        {
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("fg_name", "decorator_alkyl");
            current_fas.Add(new HeadgroupDecorator("decorator_alkyl", -1, 1, null, true));
            tmp.Add(FA_I(), new Dict());
        }



        public void add_hg_alkyl(TreeNode node){
            tmp.Remove(FA_I());
            headgroup_decorators.Add((HeadgroupDecorator)current_fas.PopBack());
            tmp.Remove(FA_I());
        }


        public void set_linkage_type(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("linkage_type", node.get_text().Equals("N") ? 1 : 0);
        }



        public void set_hydrocarbon_chain(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "cc");
            current_fas.Add(new CarbonChain((FattyAcid)null));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_hydrocarbon_chain(TreeNode node)
        {
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            tmp.Remove(FA_I());
            CarbonChain cc = (CarbonChain)current_fas.PopBack();
            cc.position = linkage_pos;
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            
            if (!current_fas.back().functional_groups.ContainsKey("cc")) current_fas.back().functional_groups.Add("cc", new List<FunctionalGroup>());
            current_fas.back().functional_groups["cc"].Add(cc);
        }



        public void set_acyl_linkage(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "acyl");
            current_fas.Add(new AcylAlkylGroup((FattyAcid)null));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_acyl_linkage(TreeNode node)
        {
            bool linkage_type = (int)((Dict)tmp[FA_I()])["linkage_type"] == 1;
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            
            tmp.Remove(FA_I());
            AcylAlkylGroup acyl = (AcylAlkylGroup)current_fas.PopBack();
                
            acyl.position = linkage_pos;
            acyl.set_N_bond_type(linkage_type);
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
                
            if (!current_fas.back().functional_groups.ContainsKey("acyl")) current_fas.back().functional_groups.Add("acyl", new List<FunctionalGroup>());
            current_fas.back().functional_groups["acyl"].Add(acyl);
        }



        public void set_alkyl_linkage(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "alkyl");
            current_fas.Add(new AcylAlkylGroup(null, -1, 1, true));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_alkyl_linkage(TreeNode node)
        {
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            tmp.Remove(FA_I());
            AcylAlkylGroup alkyl = (AcylAlkylGroup)current_fas.PopBack();
            
            alkyl.position = linkage_pos;
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            
            if (!current_fas.back().functional_groups.ContainsKey("alkyl")) current_fas.back().functional_groups.Add("alkyl", new List<FunctionalGroup>());
            current_fas.back().functional_groups["alkyl"].Add(alkyl);
        }



        public void set_cycle_start(TreeNode node)
        {
            ((Cycle)current_fas.back()).start = node.get_int();
        }



        public void set_cycle_end(TreeNode node)
        {
            ((Cycle)current_fas.back()).end = node.get_int();
        }



        public void set_cycle_number(TreeNode node){
            ((Cycle)current_fas.back()).cycle = node.get_int();
        }



        public void set_cycle_db_count(TreeNode node)
        {
            ((Cycle)current_fas.back()).double_bonds.num_double_bonds = node.get_int();
        }



        public void set_cycle_db_positions(TreeNode node){
            ((Dict)tmp[FA_I()]).Add("cycle_db", ((Cycle)current_fas.back()).double_bonds.get_num());
        }



        public void check_cycle_db_positions(TreeNode node)
        {
            if (((Cycle)current_fas.back()).double_bonds.get_num() != (int)((Dict)tmp[FA_I()])["cycle_db"])
            {
                throw new LipidException("Double bond number in cycle does not correspond to number of double bond positions.");
            }
        }



        public void set_cycle_db_position(TreeNode node)
        {
            int pos = node.get_int();
            ((Cycle)current_fas.back()).double_bonds.double_bond_positions.Add(pos, "");
            ((Dict)tmp[FA_I()]).Add("last_db_pos", pos);
        }


        public void set_cycle_db_position_cistrans(TreeNode node)
        {
            int pos = (int)((Dict)tmp[FA_I()])["last_db_pos"];
            ((Cycle)current_fas.back()).double_bonds.double_bond_positions[pos] = node.get_text();
        }



        public void set_functional_group_position(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_pos", node.get_int());
        }



        public void set_functional_group_name(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", node.get_text());
        }



        public void set_functional_group_count(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_cnt", node.get_int());
        }



        public void set_functional_group_stereo(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_stereo", node.get_text());
            contains_stereo_information = true;
        }



        public void set_sn_position_func_group(TreeNode node)
        {
            ((Dict)tmp[FA_I()])["fg_name"] = node.get_text();
            set_lipid_level(LipidLevel.SN_POSITION);
        }



        public void add_functional_group(TreeNode node)
        {
            string fa_i = FA_I();
            Dict gd = (Dict)tmp[FA_I()];
            string fg_name = (string)gd["fg_name"];
            
            if (special_types.Contains(fg_name) || fg_name.Equals("cy")) return;
                
            int fg_pos = (int)gd["fg_pos"];
            int fg_cnt = (int)gd["fg_cnt"];
            string fg_stereo = (string)gd["fg_stereo"];
            string fg_ring_stereo = (string)gd["fg_ring_stereo"];
            
            if (fg_pos == -1)
            {
                set_lipid_level(LipidLevel.STRUCTURE_DEFINED);
            }
    
            if (fg_cnt <= 0)
            {
                return;
            }
            
            FunctionalGroup functional_group = null;
            try {
                functional_group = KnownFunctionalGroups.get_functional_group(fg_name);
            }
            catch
            {
                throw new LipidParsingException("'" + fg_name + "' unknown");
            }
            
            functional_group.position = fg_pos;
            functional_group.count = fg_cnt;
            functional_group.stereochemistry = fg_stereo;
            functional_group.ring_stereo = fg_ring_stereo;
            
            gd.Remove("fg_pos");
            gd.Remove("fg_name");
            gd.Remove("fg_cnt");
            gd.Remove("fg_stereo");
            
            if (!current_fas.back().functional_groups.ContainsKey(fg_name)) current_fas.back().functional_groups.Add(fg_name, new List<FunctionalGroup>());
            current_fas.back().functional_groups[fg_name].Add(functional_group);
            
        }



        public void set_ether_type(TreeNode node)
        {
            string ether_type = node.get_text();
            if (ether_type.Equals("O-")) ((FattyAcid)current_fas.back()).lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether_type.Equals("P-")) ((FattyAcid)current_fas.back()).lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
        }



        public void set_ether_num(TreeNode node)
        {
            int num_ethers = 0;
            string ether = node.get_text();
            if (ether.Equals("d")) num_ethers = 2;
            else if (ether.Equals("t")) num_ethers = 3;
            else if (ether.Equals("e")) num_ethers = 4;
            tmp.Add("num_ethers", num_ethers);
        }


        public void set_species_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
        }



        public void set_molecular_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.MOLECULAR_SPECIES);
        }



        public void set_carbon(TreeNode node)
        {
            ((FattyAcid)current_fas.back()).num_carbon = node.get_int();
        }



        public void set_double_bond_count(TreeNode node)
        {
            int db_cnt = node.get_int();
            ((Dict)tmp[FA_I()]).Add("db_count", db_cnt);
            ((FattyAcid)current_fas.back()).double_bonds.num_double_bonds = db_cnt;
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
            else adduct.set_charge_sign(-1);
            if (adduct.charge == 0) adduct.charge = 1;
        }



        public void set_heavy_element(TreeNode node)
        {
            heavy_element = Elements.heavy_element_table[node.get_text()];
        }



        public void set_heavy_number(TreeNode node)
        {
            heavy_element_number = node.get_int();
        }



        public void add_heavy_component(TreeNode node)
        {
            adduct.heavy_elements[heavy_element] = heavy_element_number;
        }
    }
}
