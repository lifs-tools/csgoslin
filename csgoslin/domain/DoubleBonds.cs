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
//using ClassMap = System.Collections.Generic.Dictionary<LipidClass, LipidClassMeta>;

namespace csgoslin
{
    using ElementTable = System.Collections.Generic.Dictionary<Element, int>;
    
    public class DoubleBonds
    {
        public int num_double_bonds;
        public Dictionary<int, string> double_bond_positions;
        
        public DoubleBonds(int num = 0)
        {
            num_double_bonds = num;
            double_bond_positions = new Dictionary<int, string>();
        }


        public DoubleBonds copy(){
            DoubleBonds db = new DoubleBonds(num_double_bonds);
            foreach (KeyValuePair<int, string> kv in double_bond_positions)
            {
                db.double_bond_positions.Add(kv.Key, kv.Value);
            }
            return db;
        }


        public int get_num(){
            if (double_bond_positions.Count > 0 && double_bond_positions.Count != num_double_bonds)
            {
                throw new ConstraintViolationException("Number of double bonds '" + Convert.ToString(num_double_bonds) + "' does not match to number of double bond positions '" + Convert.ToString(double_bond_positions.Count) + "'");
            }
            return num_double_bonds;
        }
    }
}
