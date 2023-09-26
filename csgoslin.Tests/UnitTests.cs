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
        
        public bool test_adducts = true;
        public bool test_parser = true;
        public bool test_fatty_acid = true;
        public bool test_hmdb = true;
        public bool test_goslin = true;
        public bool test_lipid_maps = true;
        public bool test_swiss_lipids = true;
        public bool test_shorthand = true;
        public bool test_sum_formula = true;
        public bool test_masses = true;
        
        
        
        [Fact]
        public void AdductUnitTest()
        {
            if (!test_adducts) return;
            
            LipidMapsParser p_lm = new LipidMapsParser();
            LipidAdduct lipid = p_lm.parse("PE(16:0/18:0)[M-4H]4-");
            Assert.True(Math.Abs(lipid.get_mass() - 178.8794) < 0.001);
            
            lipid = p_lm.parse("PE(16:0/18:0)[M+HCOO]-");
            Assert.True(Math.Abs(lipid.get_mass() - 764.5447) < 0.001);
            
            lipid = p_lm.parse("PE(16:0/18:0)[M+H-H2O]1+");
            Assert.True(Math.Abs(lipid.get_mass() - 702.5432) < 0.001);
            
            lipid = p_lm.parse("PE(16:0/18:0)[M+2H]2+");
            Assert.True(Math.Abs(lipid.get_mass() - 360.7805) < 0.001);
            
            
            SwissLipidsParser p_sw = new SwissLipidsParser();
            lipid = p_sw.parse("PE(16:0/18:0)[M-4H]4-");
            Assert.True(Math.Abs(lipid.get_mass() - 178.8794) < 0.001);
            
            lipid = p_sw.parse("PE(16:0/18:0)[M+HCOO]-");
            Assert.True(Math.Abs(lipid.get_mass() - 764.5447) < 0.001);
            
            lipid = p_sw.parse("PE(16:0/18:0)[M+H-H2O]1+");
            Assert.True(Math.Abs(lipid.get_mass() - 702.5432) < 0.001);
            
            lipid = p_sw.parse("PE(16:0/18:0)[M+2H]2+");
            Assert.True(Math.Abs(lipid.get_mass() - 360.7805) < 0.001);
            
            
            HmdbParser p_hmdb = new HmdbParser();
            lipid = p_hmdb.parse("PE(16:0/18:0)[M-4H]4-");
            Assert.True(Math.Abs(lipid.get_mass() - 178.8794) < 0.001);
            
            lipid = p_hmdb.parse("PE(16:0/18:0)[M+HCOO]-");
            Assert.True(Math.Abs(lipid.get_mass() - 764.5447) < 0.001);
            
            lipid = p_hmdb.parse("PE(16:0/18:0)[M+H-H2O]1+");
            Assert.True(Math.Abs(lipid.get_mass() - 702.5432) < 0.001);
            
            lipid = p_hmdb.parse("PE(16:0/18:0)[M+2H]2+");
            Assert.True(Math.Abs(lipid.get_mass() - 360.7805) < 0.001);
            
            
            GoslinParser p_g = new GoslinParser();
            lipid = p_g.parse("PE 16:0-18:0[M-4H]4-");
            Assert.True(Math.Abs(lipid.get_mass() - 178.8794) < 0.001);
            
            lipid = p_g.parse("PE 16:0/18:0[M+HCOO]-");
            Assert.True(Math.Abs(lipid.get_mass() - 764.5447) < 0.001);
            
            lipid = p_g.parse("PE 16:0/18:0[M+H-H2O]1+");
            Assert.True(Math.Abs(lipid.get_mass() - 702.5432) < 0.001);
            
            lipid = p_g.parse("PE 16:0-18:0[M+2H]2+");
            Assert.True(Math.Abs(lipid.get_mass() - 360.7805) < 0.001);
            
            
            ShorthandParser p_s = new ShorthandParser();
            lipid = p_s.parse("PE 16:0_18:0[M-4H]4-");
            Assert.True(Math.Abs(lipid.get_mass() - 178.8794) < 0.001);
            
            lipid = p_s.parse("PE 16:0/18:0[M+HCOO]-");
            Assert.True(Math.Abs(lipid.get_mass() - 764.5447) < 0.001);
            
            lipid = p_s.parse("PE 16:0/18:0[M+H-H2O]1+");
            Assert.True(Math.Abs(lipid.get_mass() - 702.5432) < 0.001);
            
            lipid = p_s.parse("PE 16:0_18:0[M+2H]2+");
            Assert.True(Math.Abs(lipid.get_mass() - 360.7805) < 0.001);
        }
        
        
        
        [Fact]
        public void ParserUnitTest()
        {
            if (!test_parser) return;
            
            LipidParser lipid_parser = new LipidParser();
            LipidAdduct lipid = lipid_parser.parse("Cer 36:1;2");
            int ohCount = lipid.lipid.info.get_total_functional_group_count("OH");
            Assert.True(2 == ohCount);
            int asdCount = lipid.lipid.info.get_total_functional_group_count("ASD");
            Assert.True(0 == asdCount);
            lipid = lipid_parser.parse("Cer d36:1");
            ohCount = lipid.lipid.info.get_total_functional_group_count("OH");
            Assert.True(2 == ohCount);
            lipid = lipid_parser.parse("Cer 18:1;2/18:0");
            ohCount = lipid.lipid.info.get_total_functional_group_count("OH");
            Assert.True(2 == ohCount);
            lipid = lipid_parser.parse("Cer d18:1/18:0");
            ohCount = lipid.lipid.info.get_total_functional_group_count("OH");
            Assert.True(2 == ohCount);
            lipid = lipid_parser.parse("Cer 18:1;(OH)2/18:0");
            ohCount = lipid.lipid.info.get_total_functional_group_count("OH");
            Assert.True(2 == ohCount);
        }
        
        
        
        [Fact]
        public void FattyAcidUnitTest()
        {
            if (!test_fatty_acid) return;
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "fatty-acids-test.csv");
        
            FattyAcidParser fatty_acid_parser = new FattyAcidParser();
            ShorthandParser shorthand_parser = new ShorthandParser();
            
            
            
            LipidAdduct lpd = fatty_acid_parser.parse("Methyl 7Z,11Z-Hexadecadienoate");
            Assert.Equal("WE 1:0/16:2(7Z,11Z)", lpd.get_lipid_string());
            
            
                
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
            if (!test_hmdb) return;
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "hmdb-test.csv");
        
            HmdbParser parser = new HmdbParser();
            
    
            LipidAdduct l = parser.parse((string)"Cer(d18:1(8Z)/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            
            l = parser.parse((string)"GalCer(d18:1(5Z)/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"GalCer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            l = parser.parse((string)"LysoSM(d17:1(4E))");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"LSM 17:1(4);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C22H47N2O5P");
            

            l = parser.parse((string)"PE-Cer(d14:1(4E)/20:1(11Z))");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            l = parser.parse((string)"MIPC(t18:0/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"MIPC 18:0;(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"MIPC 42:0;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C54H106NO17P");
            
            
            l = parser.parse((string)"PE-Cer(d16:2(4E,6E)/22:1(13Z)(2OH))");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 16:2(4,6);OH/22:1(13);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 38:3;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C40H77N2O7P");
            
        
            l = parser.parse((string)"DG(a-21:0/20:5(5Z,8Z,10E,14Z,17Z)+=O(12S)/0:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.FULL_STRUCTURE), (string)"DG 20:0;18Me/20:5(5Z,8Z,10E,14Z,17Z);12oxo/0:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"DG 20:0;Me/20:5(5,8,10,14,17);oxo/0:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"DG 21:0/20:6;O/0:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"DG 21:0_20:6;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"DG 41:6;O");
            Assert.Equal(l.get_sum_formula(), (string)"C44H74O6");
            
            l = parser.parse((string)"PIP(16:2(9Z,12Z)/PGJ2)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"PIP 16:2/20:5;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"PIP 16:2_20:5;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"PIP 36:7;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C45H74O18P2");
            
            l = parser.parse((string)"PC(14:0/20:5(7Z,9Z,11E,13E,17Z)-3OH(5,6,15))");
            Assert.Equal(l.get_lipid_string(LipidLevel.FULL_STRUCTURE), (string)"PC 14:0/20:5(7Z,9Z,11E,13E,17Z);5OH,6OH,15OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"PC 14:0/20:5(7,9,11,13,17);(OH)3");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"PC 14:0/20:5;O3");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"PC 14:0_20:5;O3");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"PC 34:5;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C42H74NO11P");
            
            
                
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
                    Console.WriteLine("Error: " + lipid_row.Trim(trims)+"\n" + e);
                    Environment.Exit(-1);
                }
                
            }
            
            Console.WriteLine("HMDB Test: All tests passed without any problem");
        }
        
        
        
        
        
        
        
        
        [Fact]
        public void SwissLipidsUnitTest()
        {
            if (!test_swiss_lipids) return;
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "swiss-lipids-test.csv");
        
            SwissLipidsParser parser = new SwissLipidsParser();
            
                    
            LipidAdduct l = parser.parse((string)"Cer(d18:1(8Z)/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            l = parser.parse((string)"GalCer(d18:1(5Z)/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"GalCer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");

            l = parser.parse((string)"PE-Cer(d14:1(4E)/20:1(11Z))");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            l = parser.parse((string)"MIPC(t18:0/24:0)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"MIPC 18:0;(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"MIPC 42:0;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C54H106NO17P");
            
            l = parser.parse((string)"PE-Cer(d16:2(4E,6E)/22:1(13Z)(2OH))");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 16:2(4,6);OH/22:1(13);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 38:3;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C40H77N2O7P");
                
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
        
        
        
        [Fact]
        public void LipidMapsUnitTest()
        {
            if (!test_lipid_maps) return;
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "lipid-maps-test.csv");
        
            LipidMapsParser parser = new LipidMapsParser();
            
            
            LipidAdduct lipid = parser.parse((string)"Cer(d18:1(8Z)/24:0)");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(lipid.get_sum_formula(), (string)"C42H83NO3");
            
            
            lipid = parser.parse((string)"GalCer(d18:1(5Z)/24:0)");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"GalCer 18:1(5);OH/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"GalCer 18:1;O2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"GalCer 42:1;O2");
            Assert.Equal(lipid.get_sum_formula(), (string)"C48H93NO8");
            
            
            lipid = parser.parse((string)"LysoSM(d17:1(4E))");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"LSM 17:1(4);OH");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"LSM 17:1;O2");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(lipid.get_sum_formula(), (string)"C22H47N2O5P");
            

            lipid = parser.parse((string)"PE-Cer(d14:1(4E)/20:1(11Z))");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(lipid.get_sum_formula(), (string)"C36H71N2O6P");
            
            
            lipid = parser.parse((string)"MIPC(t18:0/24:0)");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"MIPC 18:0;(OH)2/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"MIPC 42:0;O3");
            Assert.Equal(lipid.get_sum_formula(), (string)"C54H106NO17P");
            
            
            lipid = parser.parse((string)"PE-Cer(d16:2(4E,6E)/22:1(13Z)(2OH))");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 16:2(4,6);OH/22:1(13);OH");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(lipid.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 38:3;O3");
            Assert.Equal(lipid.get_sum_formula(), (string)"C40H77N2O7P");
            
            

            lipid = parser.parse((string)"GalNAcβ1-4(Galβ1-4GlcNAcβ1-3)Galβ1-4Glcβ-Cer(d18:1/24:1(15Z))");
            Assert.Equal("Hex3HexNAc2Cer 42:2;O2", lipid.get_lipid_string(LipidLevel.SPECIES));
            
                
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
                    lipid = parser.parse(lipid_name);
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
        
        
        
        
        
        [Fact]
        public void GoslinUnitTest()
        {
            if (!test_goslin) return;
            
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "goslin-test.csv");
        
            GoslinParser parser = new GoslinParser();
            
            LipidAdduct l = parser.parse((string)"Cer 18:1(8Z);2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"Cer 18:1(8);(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"Cer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"Cer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C42H83NO3");
            
            l = parser.parse((string)"HexCer 18:1(5Z);2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"HexCer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"HexCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            l = parser.parse((string)"LSM 17:1(4E);2");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"LSM 17:1(4);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"LSM 17:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C22H47N2O5P");
            
            l = parser.parse((string)"LCB 18:1(4E);2");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"SPB 18:1(4);(OH)2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"SPB 18:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C18H37NO2");

            l = parser.parse((string)"EPC 14:1(4E);2/20:1(11Z)");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 14:1(4);OH/20:1(11)");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 14:1;O2/20:1");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 34:2;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C36H71N2O6P");
            
            l = parser.parse((string)"MIPC 18:0;3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"MIPC 18:0;(OH)2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"MIPC 18:0;O3/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"MIPC 42:0;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C54H106NO17P");
            
            l = parser.parse((string)"EPC 16:2(4E,6E);2/22:1(13Z);1");
            Assert.Equal(l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED), (string)"EPC 16:2(4,6);OH/22:1(13);OH");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"EPC 16:2;O2/22:1;O");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"EPC 38:3;O3");
            Assert.Equal(l.get_sum_formula(), (string)"C40H77N2O7P");

            l = parser.parse((string)"BMP 18:1-18:1");
            Assert.Equal("BMP 18:1_18:1", l.get_lipid_string());
            Assert.Equal("C42H79O10P", l.get_sum_formula());
            Assert.Equal("FA1", l.lipid.fa_list[0].name);
            Assert.Equal(-1, l.lipid.fa_list[0].position);
            Assert.Equal(1, l.lipid.fa_list[0].double_bonds.num_double_bonds);
            Assert.Equal("FA2", l.lipid.fa_list[1].name);
            Assert.Equal(-1, l.lipid.fa_list[1].position);
            Assert.Equal(1, l.lipid.fa_list[1].double_bonds.num_double_bonds);
            Assert.Equal("FA3", l.lipid.fa_list[2].name);
            Assert.Equal(-1, l.lipid.fa_list[2].position);
            Assert.Equal(0, l.lipid.fa_list[2].num_carbon);
            Assert.Equal(0, l.lipid.fa_list[2].double_bonds.num_double_bonds);
            Assert.Equal("FA4", l.lipid.fa_list[3].name);
            Assert.Equal(-1, l.lipid.fa_list[3].position);
            Assert.Equal(0, l.lipid.fa_list[3].num_carbon);
            Assert.Equal(0, l.lipid.fa_list[3].double_bonds.num_double_bonds);

            l = parser.parse((string)"TAG 18:1/0:0/16:0");
            Assert.Equal("DG 18:1/0:0/16:0", l.get_lipid_string());
            Assert.Equal("C37H70O5", l.get_sum_formula());
            Assert.Equal(3, l.lipid.fa_list.Count);
            Assert.Equal("FA1", l.lipid.fa_list[0].name);
            Assert.Equal(1, l.lipid.fa_list[0].position);
            Assert.Equal(18, l.lipid.fa_list[0].num_carbon);
            Assert.Equal(1, l.lipid.fa_list[0].double_bonds.num_double_bonds);
            Assert.Equal("FA2", l.lipid.fa_list[1].name);
            Assert.Equal(2, l.lipid.fa_list[1].position);
            Assert.Equal(0, l.lipid.fa_list[1].num_carbon);
            Assert.Equal(0, l.lipid.fa_list[1].double_bonds.num_double_bonds);
            Assert.Equal("FA3", l.lipid.fa_list[2].name);
            Assert.Equal(3, l.lipid.fa_list[2].position);
            Assert.Equal(16, l.lipid.fa_list[2].num_carbon);
            Assert.Equal(0, l.lipid.fa_list[2].double_bonds.num_double_bonds);
            
            l = parser.parse((string)"15S-HETE-d8");
            Assert.Equal("FA 20:4;OH[M[2]H8]", l.get_lipid_string());
            Assert.Equal("C20H24O3H'8", l.get_sum_formula());
            
    
            l = parser.parse((string)"NO2-OA");
            Assert.Equal("FA 18:1;NO2", l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED));
            
            l = parser.parse("7(R),14(S)-DiHDHA");
            Assert.Equal("FA 22:6;(OH)2", l.get_lipid_string(LipidLevel.STRUCTURE_DEFINED));
    
            LipidAdduct lipid;
                
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
            
            
            foreach (string lipid_name in lipid_data)
            {
                lipid = parser.parse(lipid_name);
                Assert.True(lipid != null);
            }
            
            Console.WriteLine("Goslin Test: All tests passed without any problem");
        }
        
        
        
        
        
        public static readonly List<LipidLevel> levels = new List<LipidLevel>{LipidLevel.FULL_STRUCTURE, LipidLevel.STRUCTURE_DEFINED, LipidLevel.SN_POSITION, LipidLevel.MOLECULAR_SPECIES, LipidLevel.SPECIES};

        
        
        [Fact]
        public void ShorthandUnitTest()
        {
            if (!test_shorthand) return;
            
            ShorthandParser parser = new ShorthandParser();
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
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"HexCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            l = parser.parse((string)"Gal-Cer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"Gal-Cer 18:1(5);OH/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SN_POSITION), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.MOLECULAR_SPECIES), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"HexCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            
            l = parser.parse((string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(), (string)"HexCer 18:1;O2/24:0");
            Assert.Equal(l.get_lipid_string(LipidLevel.SPECIES), (string)"HexCer 42:1;O2");
            Assert.Equal(l.get_sum_formula(), (string)"C48H93NO8");
            
            
            
            l = parser.parse((string)"HexCer 42:1;O2");
            Assert.Equal(l.get_lipid_string(), (string)"HexCer 42:1;O2");
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
        
        
        
        
        
        
        
        

        [Fact]
        public void SumFormulaUnitTest()
        {
            if (!test_sum_formula) return;
            
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
            if (!test_sum_formula) return;
            
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
            if (!test_masses) return;
            
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
    }
}

