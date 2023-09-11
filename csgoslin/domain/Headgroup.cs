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
    using ClassMap = System.Collections.Generic.Dictionary<LipidClass, LipidClassMeta>;
    using static LevelFunctions;
    
    public class Headgroup
    {
        public static Dictionary<string, LipidCategory> StringCategory = new Dictionary<string, LipidCategory>();
        public static Dictionary<string, LipidClass> StringClass = new Dictionary<string, LipidClass>();
        public static Dictionary<LipidClass, string> ClassString = new Dictionary<LipidClass, string>();
        public string headgroup;
        public LipidCategory lipid_category;
        public LipidClass lipid_class;
        public bool use_headgroup;
        public List<HeadgroupDecorator> decorators;
        public bool sp_exception;
        public static readonly HashSet<string> exception_headgroups = new HashSet<string>{"Cer", "SPB"};
        
        public static readonly Dictionary<LipidCategory, string> CategoryString = new Dictionary<LipidCategory, string>{
            {LipidCategory.NO_CATEGORY, "NO_CATEGORY"},
            {LipidCategory.UNDEFINED, "UNDEFINED"},
            {LipidCategory.GL, "GL"},
            {LipidCategory.GP, "GP"},
            {LipidCategory.SP, "SP"},
            {LipidCategory.ST, "ST"},
            {LipidCategory.FA, "FA"},
            {LipidCategory.SL, "SL"}
        };
        
        public Headgroup(string _headgroup, List<HeadgroupDecorator> _decorators = null, bool _use_headgroup = false)
        {
            headgroup = _headgroup;
            lipid_category = get_category(_headgroup);
            lipid_class = get_class(headgroup);
            use_headgroup = _use_headgroup;
            decorators = new List<HeadgroupDecorator>();
            if (_decorators != null)
            {
                foreach (HeadgroupDecorator hgd in _decorators) decorators.Add(hgd);
            }
            sp_exception = lipid_category == LipidCategory.SP && LipidClasses.lipid_classes[lipid_class].special_cases.Contains("SP_Exception") && decorators.Count == 0;
        }
        
        
        public Headgroup(Headgroup h)
        {
            headgroup = h.headgroup;
            lipid_category = h.lipid_category;
            lipid_class = h.lipid_class;
            use_headgroup = h.use_headgroup;
            decorators = new List<HeadgroupDecorator>();
            foreach (HeadgroupDecorator hgd in h.decorators) decorators.Add((HeadgroupDecorator)hgd.copy());
            sp_exception = h.sp_exception;
        }
                
        public static void init()
        {
            lock(StringCategory)
            {
                if (StringCategory.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        LipidCategory category = kvp.Value.lipid_category;
                        foreach (string hg in kvp.Value.synonyms)
                        {
                            StringCategory.Add(hg, category);
                        }
                    }
                }
            }
            
            lock(StringClass)
            {
                if (StringClass.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        LipidClass l_class = kvp.Key;
                        foreach (string hg in kvp.Value.synonyms)
                        {
                            StringClass.Add(hg, l_class);
                        }
                    }
                }
            }
            
            lock(ClassString)
            {
                if (ClassString.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        ClassString.Add(kvp.Key, kvp.Value.synonyms[0]);
                    }
                }
            }
        }

        public static LipidCategory get_category(string _headgroup)
        {
            lock(StringCategory)
            {
                if (StringCategory.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        LipidCategory category = kvp.Value.lipid_category;
                        foreach (string hg in kvp.Value.synonyms)
                        {
                            StringCategory.Add(hg, category);
                        }
                    }
                }
            }

            return StringCategory.ContainsKey(_headgroup) ? StringCategory[_headgroup] : LipidCategory.UNDEFINED;
        }



        public static LipidClass get_class(string _headgroup)
        {
            lock(StringClass)
            {
                if (StringClass.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        LipidClass l_class = kvp.Key;
                        foreach (string hg in kvp.Value.synonyms)
                        {
                            StringClass.Add(hg, l_class);
                        }
                    }
                }
            }
            
            return StringClass.ContainsKey(_headgroup) ? StringClass[_headgroup] : LipidClass.UNDEFINED_CLASS;
        }


        public static string get_class_string(LipidClass _lipid_class)
        {
            lock(ClassString)
            {
                if (ClassString.Count == 0)
                {
                    foreach (KeyValuePair<LipidClass, LipidClassMeta> kvp in LipidClasses.lipid_classes)
                    {
                        ClassString.Add(kvp.Key, kvp.Value.synonyms[0]);
                    }
                }
            }
            
            return ClassString.ContainsKey(_lipid_class) ? ClassString[_lipid_class] : "UNDEFINED";
        }


        public string get_class_name()
        {
            
            return  LipidClasses.lipid_classes.ContainsKey(lipid_class) ? LipidClasses.lipid_classes[lipid_class].class_name : "UNDEFINED";
        }


        public static string get_category_string(LipidCategory _lipid_category)
        {
            return CategoryString[_lipid_category];
        }        
                
                
        public string get_lipid_string(LipidLevel level)
        {
            if (level == LipidLevel.CATEGORY){
                return get_category_string(lipid_category);
            }
            
            string hgs = use_headgroup ? headgroup : get_class_string(lipid_class);
            
            
            StringBuilder headgoup_string = new StringBuilder();
                    
            // adding prefixes to the headgroup
            if (!is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE | LipidLevel.STRUCTURE_DEFINED))
            {
                List<HeadgroupDecorator> decorators_tmp = new List<HeadgroupDecorator>();
                foreach (HeadgroupDecorator hgd in decorators)
                {
                    if (!hgd.suffix) decorators_tmp.Add((HeadgroupDecorator)hgd.copy());
                }
                decorators_tmp.Sort(delegate(HeadgroupDecorator hi, HeadgroupDecorator hj){
                    return hi.name.CompareTo(hj.name);
                });
                for (int i = decorators_tmp.Count - 1; i > 0; i--)
                {
                    HeadgroupDecorator hge = decorators_tmp[i];
                    HeadgroupDecorator hge_before = decorators_tmp[i - 1];
                    if (hge.name.Equals(hge_before.name))
                    {
                        hge_before.count += hge.count;
                        decorators_tmp.RemoveAt(i);
                    }
                }
                foreach (HeadgroupDecorator hge in decorators_tmp) headgoup_string.Append(hge.to_string(level));
            }
            else
            {
                foreach (HeadgroupDecorator hgd in decorators)
                {
                    if (!hgd.suffix) headgoup_string.Append(hgd.to_string(level)).Append("-");
                }
            }
                
            // adding headgroup
            headgoup_string.Append(hgs);
                
            // ading suffixes to the headgroup
            foreach (HeadgroupDecorator hgd in decorators)
            {
                if (hgd != null && hgd.suffix) headgoup_string.Append(hgd.to_string(level));
            }
            if (is_level(level, LipidLevel.COMPLETE_STRUCTURE | LipidLevel.FULL_STRUCTURE) && lipid_category == LipidCategory.SP && !sp_exception)
            {
                headgoup_string.Append("(1)");
            }
            
            return headgoup_string.ToString();
        }
                
                
        public ElementTable get_elements(){
            ClassMap lipid_classes = LipidClasses.lipid_classes;
            
            if (use_headgroup || !lipid_classes.ContainsKey(lipid_class))
            {
                throw new RuntimeException("Element table cannot be computed for lipid '" + headgroup + "'");
            }
            
            ElementTable elements = StringFunctions.create_empty_table();
            
            foreach (KeyValuePair<Element, int> kv in lipid_classes[lipid_class].elements)
            {
                elements[kv.Key] += kv.Value;
            }
            
            
            foreach (HeadgroupDecorator hgd in decorators)
            {
                ElementTable hgd_elements = hgd.get_elements();
                foreach (KeyValuePair<Element, int> kv in hgd_elements)
                {
                    elements[kv.Key] += kv.Value * hgd.count;
                }
            }
            
            return elements;
        }
    }

}
