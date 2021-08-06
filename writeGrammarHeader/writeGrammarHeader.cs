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


class WriteGrammarHeader
{    
    public static void addingGrammar(StreamWriter sw, string grammarName, string grammarFilename)
    {
        string prefixPath = Environment.CurrentDirectory;
        string grammar_file_name = Path.Combine(prefixPath, "data", "goslin", grammarFilename);

        if (File.Exists(grammar_file_name))
        {
            int lineCounter = 0;
            try
            {
                using (StreamReader sr = new StreamReader(grammar_file_name))
                {
                    bool initialWrite = false;
                    sw.Write("        public static string " + grammarName + " = ");
                    string line = "";
                    while((line = sr.ReadLine()) != null)
                    {
                        lineCounter++;
                        if (line.Length == 0) continue;
                        if (initialWrite) sw.Write(" + Environment.NewLine + \n");
                        initialWrite = true;
                            
                        sw.Write("\"");
                        line = line.Replace("\\", "\\\\");
                        line = line.Replace("\"", "\\\"");
                        sw.Write(line + "\"");
                    }
                    sw.Write(";\n\n\n");
                }
            }
            catch (Exception e)
            {
                throw new Exception("The file '" + grammar_file_name + "' in line '" + lineCounter + "' could not be read:\n" + e);
            }
        }
        
        else
        {
            throw new Exception("File '" + grammar_file_name + "' does not exist.");
        }
    }
    
    static void Main(string[] args)
    {
        
        try
        {
            string prefixPath = Environment.CurrentDirectory;
            
            //string output_enums_file = Path.Combine(prefixPath, "csgoslin", "domain", "ClassesEnum.cs");
            string output_enums_file = Path.Combine(prefixPath, "csgoslin", "parser", "KnownGrammars.cs");
            using (StreamWriter sw = new StreamWriter(output_enums_file))
            {
                sw.Write("/* DO NOT CHANGE THE FILE, IT IS AUTOMATICALLY GENERATED */\n\n");
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
                sw.Write("    public class KnownGrammars\n");
                sw.Write("    {\n");
                
                addingGrammar(sw, "shorthand_grammar", "Shorthand2020.g4");
                addingGrammar(sw, "goslin_grammar", "Goslin.g4");
                addingGrammar(sw, "lipid_maps_grammar", "LipidMaps.g4");
                addingGrammar(sw, "swiss_lipids_grammar", "SwissLipids.g4");
                addingGrammar(sw, "hmdb_grammar", "HMDB.g4");
                addingGrammar(sw, "sum_formula_grammar", "SumFormula.g4");
                addingGrammar(sw, "fatty_acid_grammar", "FattyAcids.g4");
                
                sw.Write("\n    }\n");
                sw.Write("}");
            }
        }
        
        catch (Exception e)
        {
            throw new Exception("The file csgoslin/parser/KnownGrammars.cs' could not be written:\n" + e);
        }
          
    }
}
