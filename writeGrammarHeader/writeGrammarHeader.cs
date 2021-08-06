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
using System.IO;
using System.Text;
using System.Collections.Generic;


class WriteLipidEnum
{    
    
    public static List<string> split_string(string text, char separator = ',', char _quote = '"', bool with_empty = true)
    {
        bool in_quote = false;
        List<string> tokens = new List<string>();
        StringBuilder sb = new StringBuilder();
        char last_char = '\0';
        bool last_escaped_backslash = false;
        
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];
            bool escaped_backslash = false;
            if (!in_quote)
            {
                if (c == separator)
                {
                    string sb_string = sb.ToString();
                    if (sb_string.Length > 0 || with_empty) tokens.Add(sb_string.Trim(_quote));
                    sb = new StringBuilder();
                }
                else
                {
                    if (c == _quote) in_quote = !in_quote;
                    sb.Append(c);
                }
            }
            else 
            {
                if (c == '\\' && last_char == '\\' && !last_escaped_backslash)
                {
                    escaped_backslash = true;
                }
                else if (c == _quote && !(last_char == '\\' && !last_escaped_backslash))
                {
                    in_quote = !in_quote;
                }
                sb.Append(c);
            }
            
            last_escaped_backslash = escaped_backslash;
            last_char = c;
        }
        
        string sb_string_end = sb.ToString();
        
        if (sb_string_end.Length > 0 || (last_char == ',' && with_empty))
        {
            tokens.Add(sb_string_end.Trim(_quote));
        }
        if (in_quote){
            throw new Exception("Error: corrupted token in line");
        }
        return tokens;
    }
    
    
    
    static string changeChar(string s, int pos, char c){
        char[] ch = s.ToCharArray();
        ch[pos] = c;
        return new string(ch);
    }
    
    
    
    static void Main(string[] args)
    {
        string prefixPath = Environment.CurrentDirectory;
        string lipid_list_file = Path.Combine(prefixPath, "data", "goslin", "lipid-list.csv");
        
        int SYNONYM_START_INDEX = 7;
        Dictionary<string, int> enum_names = new Dictionary<string, int>{{"GL", 1}, {"GP", 1}, {"SP", 1}, {"ST", 1}, {"FA", 1}, {"PK", 1}, {"SL", 1}, {"UNDEFINED", 1}};
        
        Dictionary<string, List<string> > data = new Dictionary<string, List<string> >();
        List< List<string> > functional_data = new List< List<string> >();
        HashSet<string> functional_data_set = new HashSet<string>();
        HashSet<string> keys = new HashSet<string>();
        List<string> list_keys = new List<string>();
        
        if (File.Exists(lipid_list_file))
        {
            int lineCounter = 1;
            try
            {
                using (StreamReader sr = new StreamReader(lipid_list_file))
                {
                    String line = sr.ReadLine(); // omit titles
                    while((line = sr.ReadLine()) != null)
                    {
                        lineCounter++;
                        List<string> tokens = split_string(line);
                        
                        if (keys.Contains(tokens[0]))
                        {
                            throw new Exception("Error: lipid name '" + tokens[0] + "' occurs multiple times in the lipid list.");
                        }
                        keys.Add(tokens[0]);
                        
                        
                        for (int i = SYNONYM_START_INDEX; i < tokens.Count; ++i)
                        {
                            string test_lipid_name = tokens[i];
                            if (test_lipid_name.Length == 0) continue;
                            if (keys.Contains(test_lipid_name))
                            {
                                throw new Exception("Error: lipid name '" + test_lipid_name + "' occurs multiple times in the lipid list.");
                            }
                            keys.Add(test_lipid_name);
                        }
                        
                        
                        string enum_name = tokens[0];
                        
                        for (int i = 0; i < enum_name.Length; ++i)
                        {
                            char c = enum_name[i];
                            if ('A' <= c && c <= 'Z')
                            {
                                
                            }
                            else if ('0' <= c && c <= '9')
                            {
                                
                            }
                            else if ('a' <= c && c <= 'z')
                            {
                                enum_name = changeChar(enum_name, i, (char)(c - ('a' - 'A')));
                            }
                            else
                            {
                                enum_name = changeChar(enum_name, i, '_');
                            }
                        }
                        
                        
                        if (enum_name[0] == '_')
                        {
                            enum_name = "L" + enum_name;
                        }
                        
                        if (enum_name[0] < 'A' || 'Z' < enum_name[0])
                        {
                            enum_name = "L" + enum_name;
                        }
                        
                        if (!enum_names.ContainsKey(enum_name))
                        {
                            enum_names.Add(enum_name, 1);
                        }
                        else
                        {
                            int cnt = enum_names[enum_name]++;
                            enum_names[enum_name] = cnt;
                            enum_name += ('A' + cnt - 1);
                            enum_names.Add(enum_name, 1);
                        }
                        
                        data.Add(enum_name, tokens);
                        list_keys.Add(enum_name);
                    }
                }
                
                list_keys.Sort();
                string output_enums_file = Path.Combine(prefixPath, "csgoslin", "domain", "ClassesEnum.cs");
                using (StreamWriter sw = new StreamWriter(output_enums_file))
                {
                    sw.Write("/*\n");
                    sw.Write("MIT License\n");
                    sw.Write("\n");
                    sw.Write("Copyright (c) the authors (listed in global LICENSE file)\n");
                    sw.Write("\n");
                    sw.Write("Permission is hereby granted, free of charge, to any person obtaining a copy\n");
                    sw.Write("of this software and associated documentation files (the \"Software\"), to deal\n");
                    sw.Write("in the Software without restriction, including without limitation the rights\n");
                    sw.Write("to use, copy, modify, merge, publish, distribute, sublicense, and/or sell\n");
                    sw.Write("copies of the Software, and to permit persons to whom the Software is\n");
                    sw.Write("furnished to do so, subject to the following conditions:\n");
                    sw.Write("\n");
                    sw.Write("The above copyright notice and this permission notice shall be included in all\n");
                    sw.Write("copies or substantial portions of the Software.\n");
                    sw.Write("\n");
                    sw.Write("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR\n");
                    sw.Write("IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,\n");
                    sw.Write("FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE\n");
                    sw.Write("AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER\n");
                    sw.Write("LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,\n");
                    sw.Write("OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE\n");
                    sw.Write("SOFTWARE.\n");
                    sw.Write("*/\n");
                    sw.Write("\n");
                    sw.Write("\n");
                    sw.Write("using System;\n");
                    sw.Write("\n");
                    sw.Write("namespace csgoslin\n");
                    sw.Write("{\n");
                    sw.Write("    public enum LipidClass\n");
                    sw.Write("    {\n");
                    sw.Write("        NO_CLASS, UNDEFINED_CLASS");
                    foreach (string list_key in list_keys)
                    {
                        sw.Write(", ");
                        sw.Write(list_key);
                    }
                    sw.Write("\n    };\n");
                    sw.Write("}");
                }
            }
            
            catch (Exception e)
            {
                throw new Exception("The file '" + lipid_list_file + "' in line '" + lineCounter + "' could not be read:\n" + e);
            }
        }
        else
        {
            throw new Exception("File '" + lipid_list_file + "' does not exist.");
        }
    }
}
