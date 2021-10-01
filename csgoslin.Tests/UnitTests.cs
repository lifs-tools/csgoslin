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
    
    
    public class UnitTests
    {
        
        // FattyAcidUnitTest
        // HmdbUnitTest
        // LipidMapsUnitTest
        // ShorthandUnitTest
        // SumFormulaUnitTest
        // MassesUnitTest
        // SwissLipidsUnitTest
        
        /*
        
        [Fact]
        public void FattyAcidUnitTest()
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
                
                //Console.WriteLine(lipid_name);
                
                formula = StringFunctions.compute_sum_formula(SumFormulaParser.get_instance().parse(formula));
                
                LipidAdduct lipid = fatty_acid_parser.parse(lipid_name);
                    
                string lipid_formula = lipid.get_sum_formula();
                
                Assert.True(expected_lipid_name.Equals(lipid.get_lipid_string()), lmid + " '" + lipid_name + "': " + expected_lipid_name + " != " + lipid.get_lipid_string() + " (computed)");
                
                 
                
                
                Assert.True(formula.Equals(lipid_formula), "formula " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
            
                if (lipid_name.ToLower().IndexOf("cyano") != -1) continue;
                
                
                LipidAdduct lipid2 = shorthand_parser.parse(lipid.get_lipid_string());
                lipid_formula = lipid2.get_sum_formula();
                
                
                Assert.True(formula.Equals(lipid_formula), "lipid " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
                
                lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES));
                lipid_formula = lipid2.get_sum_formula();
                
                Assert.True(formula.Equals(lipid_formula), "molecular " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
                
                lipid2 = shorthand_parser.parse(lipid.get_lipid_string(LipidLevel.SPECIES));
                lipid_formula = lipid2.get_sum_formula();
                
                Assert.True(formula.Equals(lipid_formula), "species " + lmid + " '" + lipid_name + "': " + formula + " != " + lipid_formula + " (computed)");
            
                
            }
            
            Console.WriteLine("Fatty Acids Test: All tests passed without any problem");
        }

        
        
        
        
        
        
        [Fact]
        public void HmdbUnitTest()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "hmdb-test.csv");
        
            HmdbParser parser = new HmdbParser();
                
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
                
                try
                {
                    string lipid_name = lipid_row.Trim(trims);
                    LipidAdduct lipid = parser.parse(lipid_name);
                    Assert.True(lipid != null);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error: " + lipid_row.Trim(trims));
                    Environment.Exit(-1);
                }
                
            }
            
            Console.WriteLine("HMDB Test: All tests passed without any problem");
        }
        
        
        
        
        
        
        
        
        [Fact]
        public void SwissLipidsUnitTest()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "swiss-lipids-test.csv");
        
            SwissLipidsParser parser = new SwissLipidsParser();
                
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
                
                string lipid_name = lipid_row.Trim(trims);
                LipidAdduct lipid = parser.parse(lipid_name);
                
                Assert.True(lipid != null);
            }
            
            Console.WriteLine("SwissLipids Test: All tests passed without any problem");
        }
        
        */
        
        
        
        
        
        
        /*
        
        
        [Fact]
        public void LipidMapsUnitTest()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "lipid-maps-test.csv");
        
            LipidMapsParser lipid_maps_parser = new LipidMapsParser();
            
                
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
            int i = 0;
            foreach (string lipid_row in lipid_data)
            {
                
                List<string> data = StringFunctions.split_string(lipid_row, ',', '"', true);
                string lipid_name = data[0].Trim(trims);
                string correct_lipid_name = data[1].Trim(trims);
                
                try {
                    LipidAdduct lipid = lipid_maps_parser.parse(lipid_name);
                    if (!correct_lipid_name.Equals("Unsupported lipid") && correct_lipid_name.Length > 0)
                    {
                        Assert.True(correct_lipid_name.Equals(lipid.get_lipid_string()), lipid_name + " . " + lipid.get_lipid_string() + " != " + correct_lipid_name + " (reference)");
                    }
                }
                catch (LipidException e){
                    if (correct_lipid_name.Length > 0)
                    {
                        Console.WriteLine("Exception: " + i + ": " + lipid_name);
                        Console.WriteLine(e);
                        Assert.True(false);
                    }
                }
                
                ++i;
            }
            
            Console.WriteLine("LIPID MAPS Test: All tests passed without any problem");
        }
        
        */
        
        
        
        
        
        
        public static readonly List<LipidLevel> levels = new List<LipidLevel>{LipidLevel.FULL_STRUCTURE, LipidLevel.STRUCTURE_DEFINED, LipidLevel.SN_POSITION, LipidLevel.MOLECULAR_SPECIES, LipidLevel.SPECIES};

        
        
        [Fact]
        public void ShorthandUnitTest()
        {
            ShorthandParser parser = new ShorthandParser();
            
            /*
            //parser.parser_event_handler.debug = "a";
            LipidAdduct lip = parser.parse("Cer 18:1(5Z);1OH,3OH/14:0");
            Console.WriteLine(lip.get_sum_formula());
            return;
            */
            
            
            LipidAdduct l = parser.parse((string)"PE 18:1(8Z);1OH,3OH/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"PE 18:1(8Z);1OH,3OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"PE 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"PE 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"PE 18:1;O2_24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"PE 42:1;O2");
            
            
            l = parser.parse((string)"Cer 18:1(8Z);1OH,3OH/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Cer 18:1(8Z);1OH,3OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            
            l = parser.parse((string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            
            l = parser.parse((string)(string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            
            l = parser.parse("Cer 42:1;O2");
            Assert.Equal(l.get_lipid_string(), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            
            
            
            l = parser.parse((string)"Gal-Cer(1) 18:1(5Z);3OH/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Gal-Cer(1) 18:1(5Z);3OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Gal-Cer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            l = parser.parse((string)"Gal-Cer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Gal-Cer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            
            l = parser.parse((string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            
            l = parser.parse((string)"GalCer 42:1;O2");
            Assert.Equal(l.get_lipid_string(), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            l = parser.parse((string)"SPB 18:1(4Z);1OH,3OH");
            Assert.Equal(l.get_lipid_string(), (string)"SPB 18:1(4Z);1OH,3OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"SPB 18:1(4);(OH)2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C18H37NO2");
            
            
            l = parser.parse((string)"SPB 18:1(4);(OH)2");
            Assert.Equal(l.get_lipid_string(), (string)"SPB 18:1(4);(OH)2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C18H37NO2");
            
            
            l = parser.parse((string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C18H37NO2");
            
            

            
            l = parser.parse((string)"LSM(1) 17:1(4E);3OH");
            Assert.Equal(l.get_lipid_string(), (string)"LSM(1) 17:1(4E);3OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"LSM 17:1(4);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C22H47N2O5P");
            
            
            l = parser.parse((string)"LSM 17:1(4);OH");
            Assert.Equal(l.get_lipid_string(), (string)"LSM 17:1(4);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C22H47N2O5P");
            
            
            l = parser.parse((string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C22H47N2O5P");
            
            

            
            l = parser.parse((string)"EPC(1) 14:1(4E);3OH/20:1(11Z)");
            Assert.Equal(l.get_lipid_string(), (string)"EPC(1) 14:1(4E);3OH/20:1(11Z)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            l = parser.parse((string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            l = parser.parse((string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            l = parser.parse((string)"EPC 34:2;O2");
            Assert.Equal(l.get_lipid_string(), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "shorthand-test.csv");
            
                
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
            
            foreach (string lipid_row in lipid_data)
            {
                List<string> results = StringFunctions.split_string(lipid_row, ',', '"', true);
                for (int i = 0; i < results.Count; ++i) results[i] = results[i].Trim(new char[]{'"'});
                string lipid_name = results[0];
                
                int col_num = levels.Count;
                LipidAdduct lipid = parser.parse(lipid_name);
                string formula = (results.Count > col_num && results[col_num].Length > 0) ? results[col_num] : lipid.get_sum_formula();
                
                if (results.Count > col_num)
                {
                    Assert.True(formula.Equals(lipid.get_sum_formula()), "test on lipid '" + lipid_name + "': " + lipid.get_sum_formula() + " != " + formula + " (reference)");
                }
                
                for (int lev = 0; lev < levels.Count; ++lev)
                {
                    LipidLevel lipid_level = levels[lev];
                    string n = lipid.get_lipid_string(lipid_level);
                    Assert.True(results[lev].Equals(n), "test 1 on lipid '" + lipid_name + "' and level '" + Convert.ToString(lipid_level) + "'");
                    Assert.True(formula.Equals(lipid.get_sum_formula()), "test 2 on lipid '" + lipid_name + "' and level '" + Convert.ToString(lipid_level) + "'");

                    
                    LipidAdduct lipid2 = parser.parse(n);
                    for (int ll = lev; ll < col_num; ++ll)
                    {   
                        Assert.True(results[ll].Equals(lipid2.get_lipid_string(levels[ll])), "test 3 on lipid '" + lipid_name + "' and level '" + Convert.ToString(levels[lev]) + "' to '" + Convert.ToString(levels[ll]) + "' / input '" + n + "': " + lipid2.get_lipid_string(levels[ll]) + " != " + results[ll] + " (reference)");
                        Assert.True(formula.Equals(lipid2.get_sum_formula()), "test 4 on lipid '" + lipid_name + "' and level '" + Convert.ToString(levels[lev]) + "' mapped to level '" + Convert.ToString(levels[ll]) + "'");
                    }
                }
            }
            
            Console.WriteLine("Shorthand Test: All tests passed without any problem");
        }
        
        
        
        
        
        
        
        /*

        [Fact]
        public void SumFormulaUnitTest()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "formulas-lipid-maps.csv");
        
            LipidMapsParser lipid_maps_parser = new LipidMapsParser();
                
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
                
                
                string lipid_name = data[0].Trim(trims);
                string correct_formula = data[1].Trim(trims);
                LipidAdduct lipid = lipid_maps_parser.parse(lipid_name);
                
                Assert.True(lipid != null);
                Assert.True(lipid.get_sum_formula().Equals(correct_formula), "Error for lipid '" + lipid_name + "': " + lipid.get_sum_formula() + " != " + correct_formula + " (reference)");
                
            }
            
            Console.WriteLine("Sum Formula (LIPID MAPS) Test: All tests passed without any problem");
        }

        
        
        
        
        [Fact]
        public void SumFormulaUnitTest2()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "formulas-swiss-lipids.csv");
        
            SwissLipidsParser lipid_maps_parser = new SwissLipidsParser();
                
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
                
                
                string lipid_name = data[0].Trim(trims);
                string correct_formula = data[1].Trim(trims);
                LipidAdduct lipid = lipid_maps_parser.parse(lipid_name);
                
                Assert.True(lipid != null);
                Assert.True(lipid.get_sum_formula().Equals(correct_formula), "Error for lipid '" + lipid_name + "': " + lipid.get_sum_formula() + " != " + correct_formula + " (reference)");
                
            }
            
            Console.WriteLine("Sum Formula (SwissLipids) Test: All tests passed without any problem");
        }
        
        
        
        
        
        
        
        
        

        [Fact]
        public void MassesUnitTest()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "lipid-masses.csv");
        
            GoslinParser parser = new GoslinParser();
                
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
            int i = 0;
            LipidAdduct lipid;
            foreach (string lipid_row in lipid_data)
            {
                if (i++ == 0) continue;
                List<string> data = StringFunctions.split_string(lipid_row, ',', '"', true);
                string lipid_class = data[0].Trim(trims);
                string lipid_name = data[1].Trim(trims);
                string lipid_formula = data[2].Trim(trims);
                string lipid_adduct = data[3].Trim(trims);
                double lipid_mass = Convert.ToDouble(data[4].Trim(trims), System.Globalization.CultureInfo.InvariantCulture);
                int lipid_charge = Convert.ToInt32(data[5].Trim(trims));
                
                string full_lipid_name = lipid_name + lipid_adduct;
                lipid = parser.parse(full_lipid_name);
                
                Assert.True(lipid.get_lipid_string(LipidLevel.CLASS).Equals(lipid_class), lipid.get_lipid_string(LipidLevel.CLASS) + " != " + lipid_class + "(reference)");
                Assert.True(StringFunctions.compute_sum_formula(lipid.lipid.get_elements()).Equals(lipid_formula), "1");
                Assert.True(Math.Abs(lipid.get_mass() - lipid_mass) < 0.001, "lipid: " + Convert.ToString(lipid.get_mass()) + " != " + Convert.ToString(lipid_mass) + " (reference)");
                Assert.True(lipid.adduct.get_charge() == lipid_charge, "3");
            }
            
            Console.WriteLine("Masses Test: All tests passed without any problem");
        }
        */
    }
}

