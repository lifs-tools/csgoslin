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
    
    public class LipidCompleteStructure : LipidFullStructure
    {
        public LipidCompleteStructure(Headgroup _headgroup, List<FattyAcid> _fa = null) : base(_headgroup, _fa)
        {
            info.level = LipidLevel.COMPLETE_STRUCTURE;
        }


        public override LipidLevel get_lipid_level()
        {
            return LipidLevel.COMPLETE_STRUCTURE;
        }


        public override string get_lipid_string(LipidLevel level = LipidLevel.NO_LEVEL)
        {
            switch(level)
            {
                case LipidLevel.NO_LEVEL:
                case LipidLevel.COMPLETE_STRUCTURE:
                    return base.build_lipid_subspecies_name(LipidLevel.COMPLETE_STRUCTURE);
                    
                case LipidLevel.FULL_STRUCTURE:
                case LipidLevel.STRUCTURE_DEFINED:
                case LipidLevel.SN_POSITION:
                case LipidLevel.MOLECULAR_SPECIES:
                case LipidLevel.SPECIES:
                case LipidLevel.CLASS:
                case LipidLevel.CATEGORY:
                    return base.get_lipid_string(level);
            
                default:
                    throw new IllegalArgumentException("LipidCompleteStructure does not know how to create a lipid string for level " + Convert.ToString(level));
            }
        }
        
    }

}
