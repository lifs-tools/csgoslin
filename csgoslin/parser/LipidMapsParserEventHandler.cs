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

        
    
    public class LipidMapsParserEventHandler : BaseParserEventHandler<LipidAdduct>
    {
        
        public LipidLevel level;
        public LipidAdduct lipid;
        public string head_group;
        public FattyAcid lcb;
        public List<FattyAcid> fa_list;
        public FattyAcid current_fa;
        public bool omit_fa;
        public int db_numbers;
        public bool use_head_group;
        public int db_position;
        public string db_cistrans;
        public Headgroup headgroup;
        public string mod_text;
        public int mod_pos;
        public int mod_num;
        
        public static readonly HashSet<string> head_group_exceptions = new HashSet<string>{"PA", "PC", "PE", "PG", "PI", "PS"};
    
        public LipidMapsParserEventHandler() : base()
        {
            reset_parser(null);
            registered_events.Add("lipid_pre_event", reset_parser);
            /*
            registered_events.Add("lipid_post_event", build_lipid);
            
            registered_events.Add("mediator_pre_event", mediator_event);
            registered_events.Add("sphingoxine_pre_event", mediator_event);
            
            registered_events.Add("sgl_species_pre_event", set_species_level);
            registered_events.Add("species_fa_pre_event", set_species_level);
            registered_events.Add("tgl_species_pre_event", set_species_level);
            registered_events.Add("dpl_species_pre_event", set_species_level);
            registered_events.Add("cl_species_pre_event", set_species_level);
            registered_events.Add("dsl_species_pre_event", set_species_level);
            registered_events.Add("fa2_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa3_unsorted_pre_event", set_molecular_subspecies_level);
            registered_events.Add("fa4_unsorted_pre_event", set_molecular_subspecies_level);
            
            registered_events.Add("fa_no_hg_pre_event", pure_fa);
            
            registered_events.Add("hg_sgl_pre_event", set_head_group_name);
            registered_events.Add("hg_gl_pre_event", set_head_group_name);
            registered_events.Add("hg_cl_pre_event", set_head_group_name);
            registered_events.Add("hg_dpl_pre_event", set_head_group_name);
            registered_events.Add("hg_lpl_pre_event", set_head_group_name);
            registered_events.Add("hg_threepl_pre_event", set_head_group_name);
            registered_events.Add("hg_fourpl_pre_event", set_head_group_name);
            registered_events.Add("sphingosine_name_pre_event", set_head_group_name);
            registered_events.Add("sphinganine_name_pre_event", set_head_group_name);
            registered_events.Add("hg_dsl_pre_event", set_head_group_name);
            registered_events.Add("ch_pre_event", set_head_group_name);
            registered_events.Add("hg_che_pre_event", set_head_group_name);
            registered_events.Add("mediator_const_pre_event", set_head_group_name);
            registered_events.Add("pk_hg_pre_event", set_head_group_name);
            registered_events.Add("hg_fa_pre_event", set_head_group_name);
            registered_events.Add("hg_lsl_pre_event", set_head_group_name);
            
            registered_events.Add("lcb_pre_event", new_lcb);
            registered_events.Add("lcb_post_event", clean_lcb);
            registered_events.Add("fa_pre_event", new_fa);
            registered_events.Add("fa_post_event", append_fa);
            
            registered_events.Add("db_single_position_pre_event", set_isomeric_level);
            registered_events.Add("db_single_position_post_event", add_db_position);
            registered_events.Add("db_position_number_pre_event", add_db_position_number);
            registered_events.Add("cistrans_pre_event", add_cistrans);
            
            registered_events.Add("ether_pre_event", add_ether);
            registered_events.Add("hydroxyl_pre_event", add_hydroxyl);
            registered_events.Add("hydroxyl_lcb_pre_event", add_hydroxyl_lcb);
            registered_events.Add("db_count_pre_event", add_double_bonds);
            registered_events.Add("carbon_pre_event", add_carbon);
            
            registered_events.Add("structural_mod_pre_event", set_structural_subspecies_level);
            registered_events.Add("single_mod_pre_event", set_mod);
            registered_events.Add("mod_text_pre_event", set_mod_text);
            registered_events.Add("mod_pos_pre_event", set_mod_pos);
            registered_events.Add("mod_num_pre_event", set_mod_num);
            registered_events.Add("single_mod_post_event", add_functional_group);
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
            use_head_group = false;
            omit_fa = false;
            db_position = 0;
            db_numbers = -1;
            db_cistrans = "";
            headgroup = null;
            mod_pos = -1;
            mod_num = 1;
            mod_text = "";
        }

        public void set_molecular_subspecies_level(TreeNode node)
        {
            level = LipidLevel.MOLECULAR_SUBSPECIES;
        }
        

        public void pure_fa(TreeNode node)
        {
            head_group = "FA";
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


        public void add_db_position(TreeNode node)
        {
            if (current_fa != null)
            {
                current_fa.double_bonds.double_bond_positions.Add(db_position, db_cistrans);
                
                if (!db_cistrans.Equals("E") && !db_cistrans.Equals("Z"))
                {
                    level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
                }
            }
        }


        public void add_db_position_number(TreeNode node)
        {
            db_position = Convert.ToInt32(node.get_text());
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
            level = LipidLevel.SPECIES;
        }
        
        
        
        public void set_structural_subspecies_level(TreeNode node)
        {
            level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
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
            mod_pos = Convert.ToInt32(node.get_text());
        }


        public void set_mod_num(TreeNode node)
        {
            mod_num = Convert.ToInt32(node.get_text());
        }   
            
            
        public void add_functional_group(TreeNode node)
        {
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group(mod_text);
            functional_group.position = mod_pos;
            functional_group.count = mod_num;
            string fg_name = functional_group.name;
            if (!current_fa.functional_groups.ContainsKey(fg_name)) current_fa.functional_groups.Add(fg_name, new List<FunctionalGroup>());
            current_fa.functional_groups[fg_name].Add(functional_group);
        }
                
                
        public void new_fa(TreeNode node)
        {
            db_numbers = -1;
            current_fa = new FattyAcid("FA" + Convert.ToString(fa_list.Count + 1));
        }
            
            

        public void new_lcb(TreeNode node)
        {
            lcb = new FattyAcid("LCB");
            lcb.lcb = true;
            current_fa = lcb;
        }
                
                

        public void clean_lcb(TreeNode node)
        {
            if (db_numbers > -1 && db_numbers != current_fa.double_bonds.get_num())
            {
                throw new LipidException("Double bond count does not match with number of double bond positions");
            }
            if (current_fa.double_bonds.double_bond_positions.Count == 0 && current_fa.double_bonds.get_num() > 0)
            {
                level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
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
                level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
            }
            
            if (level == LipidLevel.STRUCTURAL_SUBSPECIES || level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                    current_fa.position = fa_list.Count + 1;
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
            if (ether.Equals("O-")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMANYL;
            else if (ether.Equals("P-")) current_fa.lipid_FA_bond_type = LipidFaBondType.ETHER_PLASMENYL;
        }
            
            
            
        public void add_hydroxyl(TreeNode node)
        {
            int num_h = Convert.ToInt32(node.get_text());
            
            if (Headgroup.get_category(head_group) == LipidCategory.SP && current_fa.lcb && !head_group.Equals("Cer") && !head_group.Equals("LCB")) num_h -= 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
        }


            
        public void add_hydroxyl_lcb(TreeNode node)
        {
            string hydroxyl = node.get_text();
            int num_h = 0;
            if (hydroxyl.Equals("m")) num_h = 1;
            else if (hydroxyl.Equals("d")) num_h = 2;
            else if (hydroxyl.Equals("t")) num_h = 3;
            
            
            if (Headgroup.get_category(head_group) == LipidCategory.SP && current_fa.lcb && !head_group.Equals("Cer") && !head_group.Equals("LCB")) num_h -= 1;
            
            FunctionalGroup functional_group = KnownFunctionalGroups.get_functional_group("OH");
            functional_group.count = num_h;
            if (!current_fa.functional_groups.ContainsKey("OH")) current_fa.functional_groups.Add("OH", new List<FunctionalGroup>());
            current_fa.functional_groups["OH"].Add(functional_group);
        }
            
            
        public void add_double_bonds(TreeNode node)
        {
            current_fa.double_bonds.num_double_bonds += Convert.ToInt32(node.get_text());
        }
            
            
        public void add_carbon(TreeNode node)
        {
            current_fa.num_carbon = Convert.ToInt32(node.get_text());
        }
            
            

        public void build_lipid(TreeNode node)
        {
            if (omit_fa && head_group_exceptions.Contains(head_group))
            {
                head_group = "L" + head_group;
            }
            
            if (lcb != null)
            {
                level = (LipidLevel)Math.Min((int)level, (int)LipidLevel.STRUCTURAL_SUBSPECIES);
                foreach (FattyAcid fa in fa_list) fa.position += 1;
                fa_list.Insert(0, lcb);
            }
            
            lipid = null;
            LipidSpecies ls = null;
            
            headgroup = new Headgroup(head_group, null, use_head_group);
            
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
            content = lipid;
        }
    }
}
