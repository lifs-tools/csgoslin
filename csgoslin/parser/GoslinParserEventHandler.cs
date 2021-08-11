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

        
    
    public class GoslinParserEventHandler : BaseParserEventHandler<LipidAdduct>
    {
        
        public LipidLevel level;
        public LipidAdduct lipid;
        public string head_group;
        public FattyAcid lcb;
        public List<FattyAcid> fa_list;
        public FattyAcid current_fa;
        public Adduct adduct;
        public int db_position;
        public string db_cistrans;
        public bool unspecified_ether;
        public Headgroup headgroup;
    
        public GoslinParserEventHandler() : base()
        {
            reset_parser(null);
            /*
            registered_events.Add("lipid_pre_event", reset_parser);
            registered_events.Add("lipid_post_event", build_lipid);
            
            registered_events.Add("hg_cl_pre_event", set_head_group_name);
            registered_events.Add("hg_mlcl_pre_event", set_head_group_name);
            registered_events.Add("hg_pl_pre_event", set_head_group_name);
            registered_events.Add("hg_lpl_pre_event", set_head_group_name);
            registered_events.Add("hg_lpl_o_pre_event", set_head_group_name);
            registered_events.Add("hg_pl_o_pre_event", set_head_group_name);
            registered_events.Add("hg_lsl_pre_event", set_head_group_name);
            registered_events.Add("hg_dsl_pre_event", set_head_group_name);
            registered_events.Add("st_pre_event", set_head_group_name);
            registered_events.Add("hg_ste_pre_event", set_head_group_name);
            registered_events.Add("hg_stes_pre_event", set_head_group_name);
            registered_events.Add("mediator_pre_event", set_head_group_name);
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
            registered_events.Add("lpl_o_pre_event", set_molecular_subspecies_level);
            registered_events.Add("hg_lpl_oc_pre_event", set_unspecified_ether);
            registered_events.Add("hg_pl_oc_pre_event", set_unspecified_ether);
            */
            debug = "";
        }

                
        public void reset_parser(TreeNode node)
        {
            level = LipidLevel.ISOMERIC_SUBSPECIES;
            lipid = null;
            head_group = "";
            lcb = null;
            fa_list = new List<FattyAcid>();
            current_fa = null;
            adduct = null;
            db_position = 0;
            db_cistrans = "";
            unspecified_ether = false;
            headgroup = null;
        }


        public void set_unspecified_ether(TreeNode node)
        {
            unspecified_ether = true;
        }

        public void set_head_group_name(TreeNode node)
        {
            head_group = node.get_text();
        }


        public void set_species_level(TreeNode node)
        {
            level = LipidLevel.SPECIES;
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
            level = LipidLevel.MOLECULAR_SUBSPECIES;
        }
            
            
        public void new_fa(TreeNode node)
        {
            LipidFaBondType lipid_FA_bond_type = LipidFaBondType.ESTER;
            if (unspecified_ether)
            {
                unspecified_ether = false;
                lipid_FA_bond_type = LipidFaBondType.ETHER_UNSPECIFIED;
            }
            current_fa = new FattyAcid("FA" + Convert.ToString(fa_list.Count + 1), 2, null, null, lipid_FA_bond_type);
        }
            
            

        public void new_lcb(TreeNode node)
        {
            lcb = new FattyAcid("LCB");
            lcb.lcb = true;
            current_fa = lcb;
        }
                
                

        public void clean_lcb(TreeNode node)
        {
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
                level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
            }
                
            
            if (current_fa.double_bonds.get_num() < 0)
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            
            if (level == LipidLevel.STRUCTURAL_SUBSPECIES || level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                    current_fa.position = fa_list.Count + 1;
            }
            

            fa_list.Add(current_fa);
            current_fa = null;
        }
            
            

        public void build_lipid(TreeNode node)
        {
            if (lcb != null)
            {
                level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
                foreach (FattyAcid fa in fa_list) fa.position += 1;
                fa_list.Insert(0, lcb);
            }
            
            
            lipid = null;
            LipidSpecies ls = null;
            
            headgroup = new Headgroup(head_group);
            
            int max_num_fa = LipidClasses.lipid_classes.ContainsKey(headgroup.lipid_class) ? LipidClasses.lipid_classes[headgroup.lipid_class].max_num_fa : 0;
            if (max_num_fa != fa_list.Count) level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.MOLECULAR_SUBSPECIES);
            

            switch (level)
            {
                case LipidLevel.SPECIES: ls = new LipidSpecies(headgroup, fa_list); break;
                case LipidLevel.MOLECULAR_SUBSPECIES: ls = new LipidMolecularSubspecies(headgroup, fa_list); break;
                case LipidLevel.STRUCTURAL_SUBSPECIES: ls = new LipidStructuralSubspecies(headgroup, fa_list); break;
                case LipidLevel.ISOMERIC_SUBSPECIES: ls = new LipidIsomericSubspecies(headgroup, fa_list); break;
                default: break;
            }
            lipid = new LipidAdduct();
            lipid.lipid = ls;
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
        }
            
            

        public void add_old_hydroxyl(TreeNode node)
        {
            string old_hydroxyl = node.get_text();
            int num_h = 0;
            if (old_hydroxyl.Equals("d")) num_h = 2;
            else if (old_hydroxyl.Equals("t")) num_h = 3;
            
            
            if (Headgroup.get_category(head_group) == LipidCategory.SP && current_fa.lcb && !head_group.Equals("Cer") && !head_group.Equals("LCB")) num_h -= 1;
            
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
            
            if (Headgroup.get_category(head_group) == LipidCategory.SP && current_fa.lcb && !head_group.Equals("Cer") && !head_group.Equals("LCB")) num_h -= 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
            level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
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
            else if (sign.Equals("-")) adduct.set_charge_sign(-1);
        }
    }
}
