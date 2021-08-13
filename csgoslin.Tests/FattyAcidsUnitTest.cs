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
using Xunit;
using System.IO;
using System.Collections.Generic;

namespace csgoslin.Tests
{
    using ElementTable = System.Collections.Generic.Dictionary<Element, int>;
    public class FattyAcidUnitTest
    {

        [Fact]
        public void FattyAcidUnitTest1()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "fatty-acids-test.csv");
        
            FattyAcidParser fatty_acid_parser = new FattyAcidParser();
            ShorthandParser shorthand_parser = new ShorthandParser();
            
                
            // test several more lipid names
            List<string> lipid_data = new List<string>();
            if (File.Exists(test_file_name))
            {
                int lineCounter = 0;
                try
                {
                    string line;
                    using (StreamReader sr = new StreamReader(test_file_name))
                    {
                        while((line = sr.ReadLine()) != null)
                        {
                            lipid_data.Add(line.Trim(new char[]{' '}));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("The file '" + test_file_name + "' in line '" + lineCounter + "' could not be read:\n" + e);
                }
            }
            
            else
            {
                throw new Exception("File '" + test_file_name + "' does not exist.");
            }
            
            
            int not_implemented = 0;
            int failed = 0;
            int failed_sum = 0;
            
            
            
            ////////////////////////////////////////////////////////////////////////////
            // Test for correctness
            ////////////////////////////////////////////////////////////////////////////
            
            int i = -1;
            char[] trims = new char[]{'\"'};
            foreach (string lipid_name in lipid_data)
            {
                ++i;
                
                List<string> data = StringFunctions.split_string(lipid_name, ',', '"', true);
                string name = data[3].Trim(trims);
                if (name.Length == 0)
                {
                    continue;
                }
                
                if (name.IndexOf("yn") != -1 || name.IndexOf("furan") != -1 || name.EndsWith("ane") || name.EndsWith("one") || name.IndexOf("phosphate") != -1 || name.IndexOf("pyran") != -1 || name.EndsWith("olide") || name.EndsWith("-one"))
                {
                    not_implemented += 1;
                    continue;
                }
                
                
                LipidAdduct lipid = null;
                try 
                {
                    //Console.WriteLine(name);
                    lipid = fatty_acid_parser.parse(name);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("This-Error 1: '" + name + "'");
                    Environment.Exit(-1);
                }
                catch
                {
                    failed += 1;
                    continue;
                }
                    
                string lipid_formula = lipid.get_sum_formula();
                ElementTable e = SumFormulaParser.get_instance().parse(data[2]);
                string formula = StringFunctions.compute_sum_formula(e);
                
                if (!formula.Equals(lipid_formula))
                {
                    Console.WriteLine(i + ", " + lipid_name + ": " + formula + " / " + lipid_formula);
                    failed_sum += 1;
                    Assert.True(false);
                }
                    
                if (name.ToLower().IndexOf("cyano") != -1)
                {
                    continue;
                }
                
                LipidAdduct lipid2 = null;
                try 
                {
                    lipid2 = shorthand_parser.parse(lipid.get_lipid_string());
                    lipid_formula = lipid2.get_sum_formula();
                }
                catch (Exception ex2)
                {
                    Console.WriteLine(ex2);
                    Console.WriteLine("This-Error 2: '" + name + "'");
                    Environment.Exit(-1);
                }
                
                if (!formula.Equals(lipid_formula))
                {
                    Console.WriteLine("current, " + i + ", " + lipid_name + ": " + formula + " != " + lipid_formula + " / " + lipid.get_lipid_string()); 
                    failed_sum += 1;
                }
                
                try 
                {
                    lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.MOLECULAR_SUBSPECIES));
                    lipid_formula = lipid2.get_sum_formula();
                }
                catch (Exception ex3)
                {
                    Console.WriteLine(ex3);
                    Console.WriteLine("This-Error 3: '" + name + "'");
                    Environment.Exit(-1);
                }
                if (!formula.Equals(lipid_formula))
                {
                    Console.WriteLine("molecular subspecies, " + i + ", " + lipid_name + ": " + formula + " != " + lipid_formula);
                    failed_sum += 1;
                }
                
                try 
                {
                    lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.SPECIES));
                    lipid_formula = lipid2.get_sum_formula();
                
                }
                catch (Exception ex4)
                {
                    Console.WriteLine(ex4);
                    Console.WriteLine("This-Error 4: '" + name + "'");
                    Environment.Exit(-1);
                }
                if (!formula.Equals(lipid_formula))
                {
                    Console.WriteLine("species, " + i + ", " + lipid_name + ": " + formula + " != " + lipid_formula);
                    failed_sum += 1;
                }
                
            }
            
            
            Console.WriteLine("In the test, " + not_implemented + " of " + lipid_data.Count + " lipids can not be described by nomenclature");
            Console.WriteLine("In the test, " + failed + " of " + (lipid_data.Count - not_implemented) + " lipids failed");
            Console.WriteLine("In the test, " + failed_sum + " of " + (lipid_data.Count - not_implemented) + " lipid sum formulas failed");
        }
    }
}

