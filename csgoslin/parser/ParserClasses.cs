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
    public class GrammarString
    {
        string s;
        public GrammarString(string _s)
        {
            s = _s;
        }
            
        public override string ToString(){
            return s;
        }
    }
    
    // DP stands for dynamic programming
    public class DPNode
    {
        public ulong rule_index_1;
        public ulong rule_index_2;
        public DPNode left = null;
        public DPNode right = null;
        
        public DPNode(ulong _rule1, ulong _rule2, DPNode _left, DPNode _right)
        {
            rule_index_1 = _rule1;
            rule_index_2 = _rule2;
            left = _left;
            right = _right;
        }
    }

    public class TreeNode
    {
        public ulong rule_index;
        public TreeNode left;
        public TreeNode right;
        public char terminal;
        public bool fire_event;
        public static readonly char EOF_SIGN = '\0';
        public static readonly string one_str = Convert.ToString(EOF_SIGN);
            
        public TreeNode(ulong _rule, bool _fire_event)
        {
            rule_index = _rule;
            left = null;
            right = null;
            terminal = '\0';
            fire_event = _fire_event;
        }
        
        public string get_text()
        {
            if (terminal == '\0'){
                string left_str = left.get_text();
                string right_str = right != null ? right.get_text() : "";
                return (!left_str.Equals(one_str) ? left_str : "") + (!right_str.Equals(one_str) ? right_str : "");
            }
            return Convert.ToString(terminal);
        }
    }
    
    // this class is dedicated to have an efficient sorted set class storing
    // values within 0..n-1 and fast sequencial iterator
    public class Bitfield
    {
        public ulong[] field;
        public ulong[] superfield;
        static readonly ulong multiplicator = 0x022fdd63cc95386dUL;
        static readonly public int[] positions = new int[64] // from http://chessprogramming.wikispaces.com/De+Bruijn+Sequence+Generator
        { 
            0, 1,  2, 53,  3,  7, 54, 27, 4, 38, 41,  8, 34, 55, 48, 28,
            62,  5, 39, 46, 44, 42, 22,  9, 24, 35, 59, 56, 49, 18, 29, 11,
            63, 52,  6, 26, 37, 40, 33, 47, 61, 45, 43, 21, 23, 58, 17, 10,
            51, 25, 36, 32, 60, 20, 57, 16, 50, 31, 19, 15, 30, 14, 13, 12
        };
        
        public Bitfield(int length)
        {
            int l = 1 + ((length + 1) >> 6);
            int s = 1 + ((l + 1) >> 6);
            field = new ulong[l];
            superfield = new ulong[s];
            for (int i = 0; i < l; ++i) field[i] = 0;
            for (int i = 0; i < s; ++i) superfield[i] = 0;
        }
        
        public void Add(int pos)
        {
            field[pos >> 6] |= (ulong)(1UL << (pos & 63));
            superfield[pos >> 12] |= (ulong)(1UL << ((pos >> 6) & 63));
        }
        
        
        public bool find(int pos)
        {
            return ((field[pos >> 6] >> (pos & 63)) & 1UL) == 1UL;
        }
        
        
        public bool isNotSet(int pos)
        {
            return ((field[pos >> 6] >> (pos & 63)) & 1UL) == 0UL;
        }
        
        
        public System.Collections.Generic.IEnumerable<int> getBitPositions()
        {
            int spre = 0;
            foreach (ulong cell in superfield)
            {
                ulong sv = cell;
                while (sv != 0)
                {
                    // algorithm for getting least significant bit position
                    ulong sv1 = (ulong)((long)sv & -(long)sv);
                    int pos = spre + positions[(ulong)(sv1 * multiplicator) >> 58];
                    
                    ulong v = field[pos];
                    while (v != 0)
                    {
                        // algorithm for getting least significant bit position
                        ulong v1 = (ulong)((long)v & -(long)v);
                        yield return unchecked(pos << 6) + positions[unchecked((v1 * multiplicator) >> 58)];
                        v &= v - 1;
                    }
                    
                    sv &= sv - 1;
                }
                spre += 64;
            }
        }
        
        
        public static System.Collections.Generic.IEnumerable<int> getPositions(long x)
        {
            while (x != 0)
            {
                // algorithm for getting least significant bit position
                ulong v1 = (ulong)((long)x & -(long)x);
                yield return positions[unchecked((v1 * multiplicator) >> 58)];
                x &= x - 1;
            }
        }
    }
}
