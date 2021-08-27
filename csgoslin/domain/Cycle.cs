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
    public class Cycle : FunctionalGroup
    {
        public int cycle;
        public int start;
        public int end;
        public List<Element> bridge_chain;
        
        public Cycle(int _cycle, int _start = -1, int _end = -1, DoubleBonds _double_bonds = null, Dictionary<string, List< FunctionalGroup > > _functional_groups = null, List< Element > _bridge_chain = null) : base("cy", _start, 1, _double_bonds, false, "", null, _functional_groups)
        {
            cycle = _cycle;
            start = _start;
            end = _end;
            elements[Element.H] = -2;
            bridge_chain = (_bridge_chain == null) ? new List< Element >() : _bridge_chain;
        }

        public override FunctionalGroup copy()
        {
            DoubleBonds db = double_bonds.copy();
            Dictionary<string, List<FunctionalGroup> > fg = new Dictionary<string, List<FunctionalGroup> >();
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                fg.Add(kv.Key, new List<FunctionalGroup>());
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    fg[kv.Key].Add(func_group.copy());
                }
            }
            List<Element> bc = new List<Element>();
            foreach (Element e in bridge_chain) bc.Add(e);
            
            return new Cycle(cycle, start, end, db, fg, bc);
        }
                        
                        
        public override int get_double_bonds()
        {
            return base.get_double_bonds() + 1;
        }

        public override void add_position(int pos)
        {
            start += (start >= pos) ? 1 : 0;
            end += (end >= pos) ? 1 : 0;
            base.add_position(pos);
        }
            
            
        public void rearrange_functional_groups(FunctionalGroup parent, int shift)
        {
            // put everything back into parent
            foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions)
            {
                parent.double_bonds.double_bond_positions.Add(kv.Key, kv.Value);
            }
            double_bonds = new DoubleBonds();
            
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups){
                if (!parent.functional_groups.ContainsKey(kv.Key))
                {
                    parent.functional_groups.Add(kv.Key, new List<FunctionalGroup>());
                }
                parent.functional_groups[kv.Key].AddRange(functional_groups[kv.Key]);
            }
            functional_groups = new Dictionary<string, List<FunctionalGroup> >();
            
            
            // shift the cycle
            shift_positions(shift);
            
            
            // take back what's mine# check double bonds
            foreach (KeyValuePair<int, string> kv in parent.double_bonds.double_bond_positions)
            {
                if (start <= kv.Key && kv.Key <= end)
                {
                    double_bonds.double_bond_positions.Add(kv.Key, kv.Value);
                }
            }
            double_bonds.num_double_bonds = double_bonds.double_bond_positions.Count;
            
            foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions)
            {
                parent.double_bonds.double_bond_positions.Remove(kv.Key);
            }
            parent.double_bonds.num_double_bonds = parent.double_bonds.double_bond_positions.Count;
            
                    
            HashSet<string> remove_list = new HashSet<string>();
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in parent.functional_groups)
            {
                List<int> remove_item = new List<int>();
                
                int i = 0;
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    if (start <= func_group.position && func_group.position <= end && func_group != this)
                    {
                        if (!functional_groups.ContainsKey(kv.Key))
                        {
                            functional_groups.Add(kv.Key, new List<FunctionalGroup>());
                        }
                        functional_groups[kv.Key].Add(func_group);
                        remove_item.Add(i);
                    }
                    ++i;
                }
                        
                while (remove_item.Count > 0)
                {
                    int pos = remove_item[remove_item.Count - 1];
                    remove_item.RemoveAt(remove_item.Count - 1);
                    kv.Value.RemoveAt(pos);
                }
                if (kv.Value.Count == 0) remove_list.Add(kv.Key);
            }
                
            foreach (string fg in remove_list) parent.functional_groups.Remove(fg);
        }
                
        public override void shift_positions(int shift)
        {
            base.shift_positions(shift);
            start += shift;
            end += shift;
            DoubleBonds db = new DoubleBonds();
            foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions) db.double_bond_positions.Add(kv.Key + shift, kv.Value);
            db.num_double_bonds = db.double_bond_positions.Count;
            double_bonds = db;
        }
            

        public override void compute_elements()
        {
            elements = StringFunctions.create_empty_table();
            elements[Element.H] = -2 - 2 * double_bonds.num_double_bonds;
            
            foreach (Element chain_element in bridge_chain)
            {
                switch(chain_element)
                {
                    case Element.C:
                        elements[Element.C] += 1;
                        elements[Element.H] += 2;
                        break;
                        
                    case Element.N:
                        elements[Element.N] += 1;
                        elements[Element.H] += 1;
                        break;
                        
                    case Element.P:
                        elements[Element.P] += 1;
                        elements[Element.H] += 1;
                        break;
                        
                    case Element.As:
                        elements[Element.As] += 1;
                        elements[Element.H] += 1;
                        break;
                        
                    case Element.O:
                        elements[Element.O] += 1;
                        break;
                        
                    case Element.S:
                        elements[Element.S] += 1;
                        break;
                        
                    default:
                        throw new ConstraintViolationException("Element '" + Elements.element_shortcut[chain_element] + "' cannot be part of a cycle bridge");
                }
                
            }
                
            // add all implicit carbon chain elements
            if (start != -1 && end != -1)
            {
                int n = cycle - (end - start + 1 + bridge_chain.Count);
                elements[Element.C] += n;
                elements[Element.H] += 2 * n;
            }
        }

            
        public override string to_string(LipidLevel level)
        {
            StringBuilder cycle_string = new StringBuilder();
            cycle_string.Append("[");
            if (start != -1 && level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                cycle_string.Append(start).Append("-").Append(end);
            }
            
            if ((level == LipidLevel.ISOMERIC_SUBSPECIES || level == LipidLevel.STRUCTURAL_SUBSPECIES) && bridge_chain.Count > 0)
            {
                foreach (Element e in bridge_chain) cycle_string.Append(Elements.element_shortcut[e]);
            }
            cycle_string.Append("cy").Append(cycle);            
            cycle_string.Append(":").Append(double_bonds.get_num());
            
            if (level == LipidLevel.ISOMERIC_SUBSPECIES || level == LipidLevel.STRUCTURAL_SUBSPECIES)
            {
                if (double_bonds.double_bond_positions.Count > 0)
                {
                    int i = 0;
                    cycle_string.Append("(");
                    foreach (KeyValuePair<int, string> kv in double_bonds.double_bond_positions)
                    {
                        if (i++ > 0) cycle_string.Append(",");
                        if (level == LipidLevel.ISOMERIC_SUBSPECIES) cycle_string.Append(kv.Key).Append(kv.Value);
                        else cycle_string.Append(kv.Key);
                    }
                    cycle_string.Append(")");
                }
            }
            
            if (level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(delegate(string x, string y)
                {
                    return x.ToLower().CompareTo(y.ToLower());
                });
                
                foreach (string fg in fg_names)
                {
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (fg_list.Count == 0) continue;
                    
                    fg_list.Sort(delegate(FunctionalGroup f1, FunctionalGroup f2)
                    {
                        return f1.position - f2.position;
                    });
                    int i = 0;
                    cycle_string.Append(";");
                    foreach (FunctionalGroup func_group in fg_list)
                    {
                        if (i++ > 0) cycle_string.Append(",");
                        cycle_string.Append(func_group.to_string(level));
                    }
                }
            }
            
            else if (level == LipidLevel.STRUCTURAL_SUBSPECIES)
            {
                List<string> fg_names = new List<string>();
                foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups) fg_names.Add(kv.Key);
                fg_names.Sort(delegate(string x, string y)
                {
                    return x.ToLower().CompareTo(y.ToLower());
                });
                
                foreach (string fg in fg_names)
                {
                    List<FunctionalGroup> fg_list = functional_groups[fg];
                    if (fg_list.Count == 0) continue;
                    
                    else if (fg_list.Count == 1 && fg_list[0].count == 1)
                    {
                        cycle_string.Append(";").Append(fg_list[0].to_string(level));
                    }
                    else {
                        int fg_count = 0;
                        foreach (FunctionalGroup func_group in fg_list) fg_count += func_group.count;
                        if (fg_count > 1)
                        {
                            cycle_string.Append(";(").Append(fg).Append(")").Append(fg_count);
                        }
                        else {
                            cycle_string.Append(";").Append(fg);
                        }
                    }
                }
            }
                        
            cycle_string.Append("]");
            if (stereochemistry.Length > 0) cycle_string.Append("[").Append(stereochemistry).Append("]");
            
            return cycle_string.ToString();
        }
    }
}
