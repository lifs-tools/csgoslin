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
    
    public class FunctionalGroup
    {
        public string name;
        public int position;
        public int count;
        public string stereochemistry;
        public string ring_stereo;
        public DoubleBonds double_bonds;
        public bool is_atomic;
        public ElementTable elements;
        public Dictionary<string, List<FunctionalGroup> > functional_groups;
        
        public FunctionalGroup(string _name, int _position = -1, int _count = 1, DoubleBonds _double_bonds = null, bool _is_atomic = false, string _stereochemistry = "", ElementTable _elements = null, Dictionary<string, List<FunctionalGroup> > _functional_groups = null)
        {
            name = _name;
            position = _position;
            count = _count;
            stereochemistry = _stereochemistry;
            ring_stereo = "";
            double_bonds = (_double_bonds != null) ? _double_bonds : new DoubleBonds(0);
            is_atomic = _is_atomic;
            elements = (_elements != null) ? _elements : StringFunctions.create_empty_table();
            functional_groups = (_functional_groups != null) ? _functional_groups : (new Dictionary<string, List<FunctionalGroup> >());
        }


        public FunctionalGroup copy()
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
            ElementTable e = StringFunctions.create_empty_table();
            foreach (KeyValuePair<Element, int> kv in elements)
            {
                e[kv.Key] = kv.Value;
            }
            
            FunctionalGroup func_group_new = new FunctionalGroup(name, position, count, db, is_atomic, stereochemistry, e, fg);
            func_group_new.ring_stereo = ring_stereo;
            return func_group_new;
        }



        public bool position_sort_function (FunctionalGroup f1, FunctionalGroup f2)
        {
            return (f1.position < f2.position);
        }


        public bool lower_name_sort_function (string s1, string s2)
        {
            return String.Compare(s1.ToLower(), s2.ToLower()) > 0;
        }


        public ElementTable get_elements()
        {
            compute_elements();
            ElementTable _elements = StringFunctions.create_empty_table();
            foreach (KeyValuePair<Element, int> kv in elements) _elements[kv.Key] = kv.Value;
            
            ElementTable fgElements = get_functional_group_elements();
            foreach (KeyValuePair<Element, int> kv in fgElements) _elements[kv.Key] += kv.Value;
            return _elements;
        }


        public void shift_positions(int shift)
        {
            position += shift;
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                foreach (FunctionalGroup fg in kv.Value)
                    fg.shift_positions(shift);
            }
        }


        ElementTable get_functional_group_elements(){
            ElementTable _elements = StringFunctions.create_empty_table();
            
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    ElementTable fg_elements = func_group.get_elements();
                    foreach (KeyValuePair<Element, int> el in fg_elements)
                    {
                        _elements[el.Key] += el.Value * func_group.count;
                    }
                }
            }
            
            return _elements;
        }


        public void compute_elements()
        {
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    func_group.compute_elements();
                }
            }
        }



                
        public string to_string(LipidLevel level)
        {
            string fg_string = "";
            if (level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                if ('0' <= name[0] && name[0] <= '9')
                {
                    fg_string = (position > -1) ? (Convert.ToString(position) + ring_stereo + "(" + name + ")") : name;
                }
                else
                {
                    fg_string = (position > -1) ? (Convert.ToString(position) + ring_stereo + name) : name;
                }
            }
            else
            {
                fg_string = (count > 1) ? ("(" + name + ")" + Convert.ToString(count)) : name;
            }
            if (stereochemistry.Length > 0 && level == LipidLevel.ISOMERIC_SUBSPECIES)
            {
                fg_string += "[" + stereochemistry + "]";
            }
                    
            return fg_string;
        }


        public int get_double_bonds(){
            int db = count * double_bonds.get_num();
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                foreach (FunctionalGroup func_group in kv.Value)
                {
                    db += func_group.get_double_bonds();
                }
            }
                    
            return db;
        }


        public void add_position(int pos)
        {
            position += (position >= pos) ? 1 : 0;
            
            foreach (KeyValuePair<string, List<FunctionalGroup> > kv in functional_groups)
            {
                foreach (FunctionalGroup fg in kv.Value)
                {
                    fg.add_position(pos);
                }
            }
        }


        public void add(FunctionalGroup fg){
            foreach (KeyValuePair<Element, int> kv in fg.elements)
            {
                elements[kv.Key] += kv.Value * fg.count;
            }
        }
    }

}
