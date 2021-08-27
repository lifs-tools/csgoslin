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
            
            
            
            ////////////////////////////////////////////////////////////////////////////
            // Test for correctness
            ////////////////////////////////////////////////////////////////////////////
            
            char[] trims = new char[]{'\"'};
            foreach (string lipid_row in lipid_data)
            {
                List<string> data = StringFunctions.split_string(lipid_row, ',', '"', true);
                string lmid = data[0].Trim(trims);
                string lipid_name = data[1].Trim(trims);
                string formula = data[2].Trim(trims);
                string expected_lipid_name = data[3].Trim(trims);
                
                formula = StringFunctions.compute_sum_formula(SumFormulaParser.get_instance().parse(formula));
                
                LipidAdduct lipid = fatty_acid_parser.parse(lipid_name);
                    
                string lipid_formula = lipid.get_sum_formula();
                
                Assert.True(expected_lipid_name.Equals(lipid.get_lipid_string()), lmid + " '" + lipid_name + "': " + expected_lipid_name + " != " + lipid.get_lipid_string() + " (computed)");
                
                 
                
                
                Assert.True(formula.Equals(lipid_formula), "formula " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
            
                if (lipid_name.ToLower().IndexOf("cyano") != -1) continue;
                
                
                LipidAdduct lipid2 = shorthand_parser.parse(lipid.get_lipid_string());
                lipid_formula = lipid2.get_sum_formula();
                
                
                Assert.True(formula.Equals(lipid_formula), "lipid " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
                
                lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.MOLECULAR_SUBSPECIES));
                lipid_formula = lipid2.get_sum_formula();
                
                Assert.True(formula.Equals(lipid_formula), "molecular " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
                
                lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.SPECIES));
                lipid_formula = lipid2.get_sum_formula();
                
                Assert.True(formula.Equals(lipid_formula), "species " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
            
                
            }
            
            Console.WriteLine("All tests passed without any problem");
        }
    }
}

