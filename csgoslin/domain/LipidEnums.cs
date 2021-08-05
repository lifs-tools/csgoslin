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
    
    public enum LipidCategory
    {
        NO_CATEGORY,
        UNDEFINED,
        GL, // SLM:000117142 Glycerolipids
        GP, // SLM:000001193 Glycerophospholipids
        SP, // SLM:000000525 Sphingolipids
        ST, // SLM:000500463 Steroids and derivatives
        FA, // SLM:000390054 Fatty acyls and derivatives
        PK, // polyketides
        SL // Saccharo lipids
    };
    
    public class LipidEnum {
        public static Dictionary<LipidCategory, string> CategoryString = new Dictionary<LipidCategory, string>
        {
            {LipidCategory.NO_CATEGORY, "NO_CATEGORY"},
            {LipidCategory.UNDEFINED, "UNDEFINED"},
            {LipidCategory.GL, "GL"},
            {LipidCategory.GP, "GP"},
            {LipidCategory.SP, "SP"},
            {LipidCategory.ST, "ST"},
            {LipidCategory.FA, "FA"},
            {LipidCategory.SL, "SL"}
        };
    }

    public enum LipidLevel {
        NO_LEVEL,
        UNDEFINED_LEVEL,
        CATEGORY, // Mediators, Glycerolipids, Glycerophospholipids, Sphingolipids, Steroids, Prenols
        CLASS, // Glyerophospholipids -> Glycerophosphoinositols (PI)
        SPECIES, // Phosphatidylinositol (16:0) or PI(16:0)
        MOLECULAR_SUBSPECIES, // Phosphatidylinositol (8:0-8:0) or PI(8:0-8:0)
        STRUCTURAL_SUBSPECIES, // Phosphatidylinositol (8:0/8:0) or PI(8:0/8:0)
        ISOMERIC_SUBSPECIES // e.g. Phosphatidylethanolamine (P-18:0/22:6(4Z,7Z,10Z,13Z,16Z,19Z))
    };



    public struct LipidClassMeta {
        public LipidCategory lipid_category;
        public string class_name;
        public int max_num_fa;
        public int possible_num_fa;
        public HashSet<string> special_cases;
        public ElementTable elements;
        public List<string> synonyms;
    }



    public enum LipidFaBondType {
        NO_FA,
        UNDEFINED_FA,
        ESTER,
        ETHER_PLASMANYL,
        ETHER_PLASMENYL,
        ETHER_UNSPECIFIED,
        AMINE
    };

    /*
    public class LipidClasses {
        public:
            static LipidClasses& get_instance()
            {
                static LipidClasses instance;
                return instance;
            }
            
        private:
            LipidClasses();
            
        public:
            ClassMap lipid_classes;
            LipidClasses(LipidClasses const&) = delete;
            void operator=(LipidClasses const&) = delete;
    };
    */
}
