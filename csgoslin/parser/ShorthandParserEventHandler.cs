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
 
    
    
    public class ShorthandParserEventHandler : BaseParserEventHandler<LipidAdduct>
    {
        public LipidLevel level;
        public LipidAdduct lipid;
        public string headgroup;
        public ExendedList<FattyAcid> fa_list;
        public ExendedList<FunctionalGroup> current_fa;
        public Adduct adduct;
        public ExendedList<HeadgroupDecorator> headgroup_decorators;
        public Dict tmp = new Dict();
        public static readonly HashSet<string> special_types = new HashSet<string>{"acyl", "alkyl", "decorator_acyl", "decorator_alkyl", "cc"};
        
        public ShorthandParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);

            // set categories
            registered_events.Add("sl_pre_event", pre_sphingolipid);
            registered_events.Add("sl_post_event", post_sphingolipid);
            registered_events.Add("sl_hydroxyl_pre_event", set_hydroxyl);

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
            registered_events.Add("gl_hg_true_double_pre_event", set_headgroup_name);
            registered_events.Add("gl_hg_triple_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_double_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_quadro_pre_event", set_headgroup_name);
            registered_events.Add("sl_hg_single_pre_event", set_headgroup_name);
            registered_events.Add("pl_hg_double_fa_hg_pre_event", set_headgroup_name);
            registered_events.Add("sl_hg_double_name_pre_event", set_headgroup_name);
            registered_events.Add("st_hg_pre_event", set_headgroup_name);
            registered_events.Add("st_hg_ester_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_m_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_d_pre_event", set_headgroup_name);
            registered_events.Add("hg_pip_pure_t_pre_event", set_headgroup_name);
            registered_events.Add("hg_PE_PS_pre_event", set_headgroup_name);

            // set head group headgroup_decorators
            registered_events.Add("carbohydrate_pre_event", set_carbohydrate);
            registered_events.Add("carbohydrate_structural_pre_event", set_carbohydrate_structural);
            registered_events.Add("carbohydrate_isomeric_pre_event", set_carbohydrate_isomeric);

            // fatty acyl events
            registered_events.Add("lcb_post_event", set_lcb);
            registered_events.Add("fatty_acyl_chain_pre_event", new_fatty_acyl_chain);
            registered_events.Add("fatty_acyl_chain_post_event", add_fatty_acyl_chain);
            registered_events.Add("carbon_pre_event", set_carbon);
            registered_events.Add("db_count_pre_event", set_double_bond_count);
            registered_events.Add("db_position_number_pre_event", set_double_bond_position);
            registered_events.Add("db_single_position_pre_event", set_double_bond_information);
            registered_events.Add("db_single_position_post_event", add_double_bond_information);
            registered_events.Add("cistrans_pre_event", set_cistrans);
            registered_events.Add("ether_type_pre_event", set_ether_type);

            // set functional group events
            registered_events.Add("func_group_data_pre_event", set_functional_group);
            registered_events.Add("func_group_data_post_event", add_functional_group);
            registered_events.Add("func_group_pos_number_pre_event", set_functional_group_position);
            registered_events.Add("func_group_name_pre_event", set_functional_group_name);
            registered_events.Add("func_group_count_pre_event", set_functional_group_count);
            registered_events.Add("stereo_type_pre_event", set_functional_group_stereo);
            registered_events.Add("molecular_func_group_name_pre_event", set_molecular_func_group);

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
            
            debug = "";
        }
        
        
        public string FA_I()
        {
            return "fa" + Convert.ToString(current_fa.Count);
        }
        
    
        public void reset_parser(TreeNode node)
        {
            content = null;
            level = LipidLevel.ISOMERIC_SUBSPECIES;
            lipid = null;
            adduct = null;
            headgroup = "";
            fa_list = new ExendedList<FattyAcid>();
            current_fa = new ExendedList<FunctionalGroup>();
            headgroup_decorators = new ExendedList<HeadgroupDecorator>();
            tmp = new Dict();
        }


        public void build_lipid(TreeNode node)
        {
            Headgroup head_group = new Headgroup(headgroup, headgroup_decorators);
            int true_fa = 0;
            foreach (FattyAcid fa in fa_list)
            {
                true_fa += (fa.num_carbon > 0 || fa.double_bonds.get_num() > 0) ? 1 : 0;
            }
            int poss_fa = LipidClasses.lipid_classes[head_group.lipid_class].possible_num_fa;
            
            
            // make lyso
            if (true_fa + 1 == poss_fa && level != LipidLevel.SPECIES && head_group.lipid_category == LipidCategory.GP && !headgroup.Substring(3).Equals("PIP"))
            {
                headgroup = "L" + headgroup;
                head_group = new Headgroup(headgroup, headgroup_decorators);
                poss_fa = LipidClasses.lipid_classes[head_group.lipid_class].possible_num_fa;
            }
            
            if (level == LipidLevel.SPECIES)
            {
                if (true_fa == 0 && poss_fa != 0)
                {
                    string hg_name = head_group.headgroup;
                    throw new ConstraintViolationException("No fatty acyl information lipid class '" + hg_name + "' provided.");
                }
            }
                
            else if (true_fa != poss_fa && (level == LipidLevel.ISOMERIC_SUBSPECIES || level == LipidLevel.STRUCTURAL_SUBSPECIES))
            {
                string hg_name = head_group.headgroup;
                throw new ConstraintViolationException("Number of described fatty acyl chains (" + Convert.ToString(true_fa) + ") not allowed for lipid class '" + hg_name + "' (having " + Convert.ToString(poss_fa) + " fatty aycl chains).");
            }
            
            if (LipidClasses.lipid_classes[head_group.lipid_class].special_cases.Contains("HC"))
            {
                fa_list[0].lipid_FA_bond_type = LipidFaBondType.AMINE;
            }
            
            
            
            // add count numbers for fatty acyl chains
            int fa_it = (fa_list.Count > 0 && fa_list[0].lcb) ? 1 : 0;
            for (int it = fa_it; it < fa_list.Count; ++it)
            {
                fa_list[it].name += Convert.ToString(it + 1);
            }
            
            lipid = new LipidAdduct();
            lipid.adduct = adduct;
            
            
            switch(level)
            {
                case LipidLevel.ISOMERIC_SUBSPECIES:
                    lipid.lipid = new LipidIsomericSubspecies(head_group, fa_list);
                    break;
                    
                case LipidLevel.STRUCTURAL_SUBSPECIES:
                    lipid.lipid = new LipidStructuralSubspecies(head_group, fa_list);
                    break;
                    
                case LipidLevel.MOLECULAR_SUBSPECIES:
                    lipid.lipid = new LipidMolecularSubspecies(head_group, fa_list);
                    break;
                    
                case LipidLevel.SPECIES:
                    lipid.lipid = new LipidSpecies(head_group, fa_list);
                    break;
                    
                default:
                    break;
            }
            
            if (tmp.ContainsKey("num_ethers")) lipid.lipid.info.num_ethers = (int)tmp["num_ethers"];
            
            if (level == LipidLevel.SPECIES && lipid.lipid.headgroup.sp_exception && lipid.lipid.info.functional_groups.ContainsKey("O"))
            {
                lipid.lipid.info.functional_groups["O"][0].count -= 1;
            }
            
            content = lipid;
        }

        public void set_lipid_level(LipidLevel _level)
        {
            level = (LipidLevel)Math.Min((int)level, (int)_level);
        }



        public void set_species_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.SPECIES);
        }



        public void set_molecular_level(TreeNode node)
        {
            set_lipid_level(LipidLevel.MOLECULAR_SUBSPECIES);
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
            if (headgroup.Length == 0) headgroup = node.get_text();
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
            
            if (tmp.ContainsKey("func_group_head") && ((int)tmp["func_group_head"] == 1))
            {
                headgroup_decorators.Add((HeadgroupDecorator)functional_group);
            }
            else
            {
                if (!current_fa.back().functional_groups.ContainsKey(carbohydrate))
                {
                    current_fa.back().functional_groups.Add(carbohydrate, new List<FunctionalGroup>());
                }
                current_fa.back().functional_groups[carbohydrate].Add(functional_group);
            }
        }



        public void set_carbohydrate_structural(TreeNode node)
        {
            set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            tmp.Add("func_group_head", 1);
        }



        public void set_carbohydrate_isomeric(TreeNode node)
        {
            tmp.Add("func_group_head", 1);
        }



        public void suffix_decorator_molecular(TreeNode node)
        {
            headgroup_decorators.Add(new HeadgroupDecorator(node.get_text(), -1, 1, null, true, LipidLevel.MOLECULAR_SUBSPECIES));
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



        public void pre_sphingolipid(TreeNode node)
        {
            tmp.Add("sl_hydroxyl", 0);
        }



        public void set_ring_stereo(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_ring_stereo", node.get_text());
        }



        public void post_sphingolipid(TreeNode node)
        {
            if (((int)tmp["sl_hydroxyl"]) == 0 && !headgroup.Equals("Cer") && !headgroup.Equals("SPB"))
            {
                set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            }
        }


        public void set_hydroxyl(TreeNode node)
        {
            tmp.Add("sl_hydroxyl", 1);
        }



        public void set_lcb(TreeNode node)
        {
                fa_list.back().lcb = true;
                fa_list.back().name = "LCB";
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
            current_fa.Add(new FattyAcid("FA"));
            tmp.Add(FA_I(), new Dict());
        }



        public void add_fatty_acyl_chain(TreeNode node)
        {
            string fg_i = "fa" + Convert.ToString(current_fa.Count - 2);
            string special_type = "";
            if (current_fa.Count >= 2 && tmp.ContainsKey(fg_i) && ((Dict)tmp[fg_i]).ContainsKey("fg_name"))
            {
                string fg_name = (string)((Dict)tmp[fg_i])["fg_name"];
                if (special_types.Contains(fg_name))
                {
                    special_type = fg_name;
                }
            }
            
            string fa_i = FA_I();
            if (current_fa.back().double_bonds.get_num() != (int)((Dict)tmp[fa_i])["db_count"])
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            else if (current_fa.back().double_bonds.get_num() > 0 && current_fa.back().double_bonds.double_bond_positions.Count == 0)
            {
                set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            }
            tmp.Remove(fa_i);
            
            FattyAcid fa = (FattyAcid)current_fa.PopBack();
            if (special_type.Length > 0)
            {
                fa.name = special_type;
                if (!current_fa.back().functional_groups.ContainsKey(special_type))
                {
                    current_fa.back().functional_groups.Add(special_type, new List<FunctionalGroup>());
                }
                current_fa.back().functional_groups[special_type].Add(fa);
            }
            else
            {
                fa_list.Add(fa);
            }
        }



        public void set_double_bond_position(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("db_position", Convert.ToInt32(node.get_text()));
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
                set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            }
            
            d.Remove("db_position");
            d.Remove("db_cistrans");
            current_fa.back().double_bonds.double_bond_positions.Add(pos, cistrans);
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
            current_fa.Add(new Cycle(0));
            
            string fa_i = FA_I();
            tmp.Add(fa_i, new Dict());
            ((Dict)tmp[fa_i]).Add("cycle_elements", new Lst());
        }



        public void add_cycle(TreeNode node)
        {
            string fa_i = FA_I();
            Lst cycle_elements = (Lst)((Dict)tmp[fa_i])["cycle_elements"];
            Cycle cycle = (Cycle)current_fa.PopBack();
            for (int i = 0; i < cycle_elements.Count; ++i)
            {
                cycle.bridge_chain.Add((Element)cycle_elements[i]);
            }
            ((Dict)tmp[fa_i]).Remove("cycle_elements");
                
            if (cycle.start > -1 && cycle.end > -1 && cycle.end - cycle.start + 1 + cycle.bridge_chain.Count < cycle.cycle)
            {
                throw new ConstraintViolationException("Cycle length '" + Convert.ToString(cycle.cycle) + "' does not match with cycle description.");
            }
            if (!current_fa.back().functional_groups.ContainsKey("cy"))
            {
                current_fa.back().functional_groups.Add("cy", new List<FunctionalGroup>());
            }
            current_fa.back().functional_groups["cy"].Add(cycle);
        }



        public void set_fatty_linkage_number(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("linkage_pos", Convert.ToInt32(node.get_text()));
        }



        public void set_hg_acyl(TreeNode node)
        {
            string fa_i = FA_I();
            tmp.Add(fa_i, new Dict());
            ((Dict)tmp[fa_i]).Add("fg_name", "decorator_acyl");
            current_fa.Add(new HeadgroupDecorator("decorator_acyl", -1, 1, null, true));
            tmp.Add(FA_I(), new Dict());
        }



        public void add_hg_acyl(TreeNode node)
        {
            tmp.Remove(FA_I());
            headgroup_decorators.Add((HeadgroupDecorator)current_fa.PopBack());
            tmp.Remove(FA_I());
        }



        public void set_hg_alkyl(TreeNode node)
        {
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("fg_name", "decorator_alkyl");
            current_fa.Add(new HeadgroupDecorator("decorator_alkyl", -1, 1, null, true));
            tmp.Add(FA_I(), new Dict());
        }



        public void add_hg_alkyl(TreeNode node){
            tmp.Remove(FA_I());
            headgroup_decorators.Add((HeadgroupDecorator)current_fa.PopBack());
            tmp.Remove(FA_I());
        }



        public void set_linkage_type(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("linkage_type", node.get_text().Equals("N") ? 1 : 0);
        }



        public void set_hydrocarbon_chain(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "cc");
            current_fa.Add(new CarbonChain((FattyAcid)null));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_hydrocarbon_chain(TreeNode node)
        {
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            tmp.Remove(FA_I());
            CarbonChain cc = (CarbonChain)current_fa.PopBack();
            cc.position = linkage_pos;
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            
            if (!current_fa.back().functional_groups.ContainsKey("cc")) current_fa.back().functional_groups.Add("cc", new List<FunctionalGroup>());
            current_fa.back().functional_groups["cc"].Add(cc);
        }



        public void set_acyl_linkage(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "acyl");
            current_fa.Add(new AcylAlkylGroup((FattyAcid)null));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_acyl_linkage(TreeNode node)
        {
            bool linkage_type = (int)((Dict)tmp[FA_I()])["linkage_type"] == 1;
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            
            tmp.Remove(FA_I());
            AcylAlkylGroup acyl = (AcylAlkylGroup)current_fa.PopBack();
                
            acyl.position = linkage_pos;
            acyl.set_N_bond_type(linkage_type);
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
                
            if (!current_fa.back().functional_groups.ContainsKey("acyl")) current_fa.back().functional_groups.Add("acyl", new List<FunctionalGroup>());
            current_fa.back().functional_groups["acyl"].Add(acyl);
        }



        public void set_alkyl_linkage(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", "alkyl");
            current_fa.Add(new AcylAlkylGroup(null, -1, 1, true));
            tmp.Add(FA_I(), new Dict());
            ((Dict)tmp[FA_I()]).Add("linkage_pos", -1);
        }



        public void add_alkyl_linkage(TreeNode node)
        {
            int linkage_pos = (int)((Dict)tmp[FA_I()])["linkage_pos"];
            tmp.Remove(FA_I());
            AcylAlkylGroup alkyl = (AcylAlkylGroup)current_fa.PopBack();
            
            alkyl.position = linkage_pos;
            if (linkage_pos == -1) set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
            
            if (!current_fa.back().functional_groups.ContainsKey("alkyl")) current_fa.back().functional_groups.Add("alkyl", new List<FunctionalGroup>());
            current_fa.back().functional_groups["alkyl"].Add(alkyl);
        }



        public void set_cycle_start(TreeNode node)
        {
            ((Cycle)current_fa.back()).start = Convert.ToInt32(node.get_text());
        }



        public void set_cycle_end(TreeNode node)
        {
            ((Cycle)current_fa.back()).end = Convert.ToInt32(node.get_text());
        }



        public void set_cycle_number(TreeNode node){
            ((Cycle)current_fa.back()).cycle = Convert.ToInt32(node.get_text());
        }



        public void set_cycle_db_count(TreeNode node)
        {
            ((Cycle)current_fa.back()).double_bonds.num_double_bonds = Convert.ToInt32(node.get_text());
        }



        public void set_cycle_db_positions(TreeNode node){
            ((Dict)tmp[FA_I()]).Add("cycle_db", ((Cycle)current_fa.back()).double_bonds.get_num());
            //delete ((Cycle*)current_fa.back()).double_bonds;
            //((Cycle*)current_fa.back()).double_bonds = new DoubleBonds();
        }



        public void check_cycle_db_positions(TreeNode node)
        {
            if (((Cycle)current_fa.back()).double_bonds.get_num() != (int)((Dict)tmp[FA_I()])["cycle_db"])
            {
                throw new LipidException("Double bond number in cycle does not correspond to number of double bond positions.");
            }
        }



        public void set_cycle_db_position(TreeNode node)
        {
            int pos = Convert.ToInt32(node.get_text());
            ((Cycle)current_fa.back()).double_bonds.double_bond_positions.Add(pos, "");
            ((Dict)tmp[FA_I()]).Add("last_db_pos", pos);
        }



        public void set_cycle_db_position_cistrans(TreeNode node)
        {
            int pos = (int)((Dict)tmp[FA_I()])["last_db_pos"];
            ((Cycle)current_fa.back()).double_bonds.double_bond_positions[pos] = node.get_text();
        }



        public void set_functional_group_position(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_pos", Convert.ToInt32(node.get_text()));
        }



        public void set_functional_group_name(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", node.get_text());
        }



        public void set_functional_group_count(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_cnt", Convert.ToInt32(node.get_text()));
        }



        public void set_functional_group_stereo(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_stereo", node.get_text());
        }



        public void set_molecular_func_group(TreeNode node)
        {
            ((Dict)tmp[FA_I()]).Add("fg_name", node.get_text());
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
                set_lipid_level(LipidLevel.STRUCTURAL_SUBSPECIES);
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
            
            if (!current_fa.back().functional_groups.ContainsKey(fg_name)) current_fa.back().functional_groups.Add(fg_name, new List<FunctionalGroup>());
            current_fa.back().functional_groups[fg_name].Add(functional_group);
            
        }



        public void set_ether_type(TreeNode node)
        {
            string ether_type = node.get_text();
            if (ether_type.Equals("O-")) ((FattyAcid)current_fa.back()).lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether_type.Equals("P-")) ((FattyAcid)current_fa.back()).lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
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



        public void set_carbon(TreeNode node)
        {
            ((FattyAcid)current_fa.back()).num_carbon = Convert.ToInt32(node.get_text());
        }



        public void set_double_bond_count(TreeNode node)
        {
            int db_cnt = Convert.ToInt32(node.get_text());
            ((Dict)tmp[FA_I()]).Add("db_count", db_cnt);
            ((FattyAcid)current_fa.back()).double_bonds.num_double_bonds = db_cnt;
        }



        public void new_adduct(TreeNode node)
        {
            adduct = new Adduct("", "", 0, 0);
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
            else adduct.set_charge_sign(-1);
        }
    }
}
