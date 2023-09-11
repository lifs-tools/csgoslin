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
    
    public class LipidEnum
    {
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
        
        
        public static HashSet<LipidFaBondType> LCB_STATES = new HashSet<LipidFaBondType>(){LipidFaBondType.LCB_REGULAR, LipidFaBondType.LCB_EXCEPTION};
    }
    
    

    public enum LipidLevel
    {
        NO_LEVEL = 1,
        UNDEFINED_LEVEL = 2,
        CATEGORY = 4, // Mediators, Glycerolipids, Glycerophospholipids, Sphingolipids, Steroids, Prenols
        CLASS = 8, // Glyerophospholipids -> Glycerophosphoinositols PI
        SPECIES = 16, // Phosphatidylinositol (16:0) or PI 16:0;O
        MOLECULAR_SPECIES = 32, // Phosphatidylinositol 8:0-8:0;O or PI 8:0-8:0;O
        SN_POSITION = 64, // Phosphatidylinositol 8:0/8:0;O or PI 8:0/8:0;O
        STRUCTURE_DEFINED = 128, // Phosphatidylinositol 8:0/8:0;OH or PI 8:0/8:0;OH
        FULL_STRUCTURE = 256, // e.g. PI 18:0/22:6(4Z,7Z,10Z,13Z,16Z,19Z);5OH
        COMPLETE_STRUCTURE = 512 // e.g. PI 18:0/22:6(4Z,7Z,10Z,13Z,16Z,19Z);5OH[R]
    };
    
    public class LevelFunctions
    {
        public static bool is_level(LipidLevel l, LipidLevel pattern)
        {
            return (l & pattern) != 0;
        }
    }



    public class LipidClassMeta
    {
        public LipidCategory lipid_category;
        public string class_name;
        public string description;
        public int max_num_fa;
        public int possible_num_fa;
        public HashSet<string> special_cases;
        public ElementTable elements;
        public List<string> synonyms;
        
        public LipidClassMeta(LipidCategory _lipid_category, string _class_name, string _description, int _max_num_fa, int _possible_num_fa, HashSet<string> _special_cases, ElementTable _elements, List<string> _synonyms){
            lipid_category = _lipid_category;
            class_name = _class_name;
            description = _description;
            max_num_fa = _max_num_fa;
            possible_num_fa = _possible_num_fa;
            special_cases = _special_cases;
            elements = _elements;
            synonyms = _synonyms;
        }
    }



    public enum LipidFaBondType {
        LCB_REGULAR,
        LCB_EXCEPTION,
        ETHER_PLASMANYL,
        ETHER_PLASMENYL,
        ETHER,
        ETHER_UNSPECIFIED,
        ESTER,
        AMIDE,
        UNDEFINED_FA,
        NO_FA
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
