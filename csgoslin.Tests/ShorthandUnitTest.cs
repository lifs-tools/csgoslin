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
using System.Collections.Generic;

namespace csgoslin.Tests
{
    public class ShorthandUnitTest
    {
        public static readonly List<LipidLevel> levels = new List<LipidLevel>{LipidLevel.ISOMERIC_SUBSPECIES, LipidLevel.STRUCTURAL_SUBSPECIES, LipidLevel.MOLECULAR_SUBSPECIES, LipidLevel.SPECIES};

        public static readonly Dictionary<string, List<string>> 
        data = new Dictionary<string, List<string> >
        {
            {"PC 18:1(11Z)/16:0", new List<string>{"PC 18:1(11Z)/16:0", "PC 18:1(11)/16:0", "PC 18:1_16:0", "PC 34:1", "C42H82NO8P"}},
                        
            {"CL O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me", new List<string>{"CL O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me/O-16:0;3Me,7Me,11Me,15Me", "CL O-16:0;(Me)4/O-16:0;(Me)4/O-16:0;(Me)4/O-16:0;(Me)4", "CL O-20:0_O-20:0_O-20:0_O-20:0", "CL eO-80:0"}}, // CL eO-80:0
                
            {"PC O-16:0/O-18:1(9Z)", new List<string>{"PC O-16:0/O-18:1(9Z)", "PC O-16:0/O-18:1(9)", "PC O-16:0_O-18:1", "PC dO-34:1"}}, // PC dO-34:1
            
            {"Cer 18:0;1OH,3OH/16:0", new List<string>{"Cer 18:0;1OH,3OH/16:0", "Cer 18:0;(OH)2/16:0", "Cer 18:0;O2/16:0", "Cer 34:0;O2"}}, // Cer 34:0;O2
                
            {"IPC(1) 18:1(8E);3OH,4OH/24:0;2OH", new List<string>{"IPC(1) 18:1(8E);3OH,4OH/24:0;2OH", "IPC 18:1(8);(OH)2/24:0;OH", "IPC 18:1;O2/24:0;O", "IPC 42:1;O4"}}, // IPC 42:1;O4
            
            {"CerP(1) 18:1(4E);3OH/16:0;2OH", new List<string>{"CerP(1) 18:1(4E);3OH/16:0;2OH", "CerP 18:1(4);OH/16:0;OH", "CerP 18:1;O/16:0;O", "CerP 34:1;O3", "C34H68NO7P"}}, // CerP 34:1;O3 / C34H68NO7P
            
            {"Gal-Gal-Glc-Cer(1) 18:1(4E);3OH/26:1(17Z)", new List<string>{"Gal-Gal-Glc-Cer(1) 18:1(4E);3OH/26:1(17Z)", "Gal-Gal-Glc-Cer 18:1(4);OH/26:1(17)", "GalGalGlcCer 18:1;O/26:1", "GalGalGlcCer 44:2;O2", "C62H115NO18"}}, // GalGalGlcCer 42:1;O2 / C60H113NO18
            
            {"PC 16:0/20:2(5Z,13E);[8-12cy5;11OH;9oxo];15OH", new List<string>{"PC 16:0/20:2(5Z,13E);[8-12cy5:0;11OH;9oxo];15OH", "PC 16:0/20:2(5,13);[cy5:0;OH;oxo];OH", "PC 16:0_20:4;O3", "PC 36:4;O3"}}, // PC 36:4;O3
            
            {"PE-N(FA 8:0) 30:5(12Z,15Z,18Z,21Z,24Z)/18:0", new List<string>{"PE-N(FA 8:0) 30:5(12Z,15Z,18Z,21Z,24Z)/18:0", "PE-N(FA 8:0) 30:5(12,15,18,21,24)/18:0", "PE-N(FA 8:0) 30:5_18:0", "PE-N(FA) 56:5", "C61H110NO9P"}}, // PE-N(FA) 56:5
            
            {"LPC O-16:1(11Z)/0:0", new List<string>{"LPC O-16:1(11Z)/0:0", "LPC O-16:1(11)/0:0", "LPC O-16:1", "LPC O-16:1"}}, // LPC O-16:1 / C24H50NO6P
            
            {"PE P-16:0/18:1(9Z)", new List<string>{"PE P-16:0/18:1(9Z)", "PE P-16:0/18:1(9)", "PE P-16:0_18:1", "PE O-34:2"}}, // PE O-34:2
            
            {"PE O-18:1(11E);5OMe/22:0;3OH", new List<string>{"PE O-18:1(11E);5OMe/22:0;3OH", "PE O-18:1(11);OMe/22:0;OH", "PE O-19:1;O_22:0;O", "PE O-41:1;O2", "C46H92NO9P"}}, // PE O-41:1;O2
            
            {"M(IP)2C(1) 20:0;3OH,4OH/26:0;2OH", new List<string>{"M(IP)2C(1) 20:0;3OH,4OH/26:0;2OH", "M(IP)2C 20:0;(OH)2/26:0;OH", "M(IP)2C 20:0;O2/26:0;O", "M(IP)2C 46:0;O4"}}, // M(IP)2C 46:0;O4
            
            {"SPB 18:0;3OH", new List<string>{"SPB 18:0;3OH", "SPB 18:0;OH", "SPB 18:0;O", "SPB 18:0;O"}}, // SPB 18:0;O
            
            {"SPB 18:0;1OH;3oxo", new List<string>{"SPB 18:0;1OH;3oxo", "SPB 18:0;OH;oxo", "SPB 18:1;O2", "SPB 18:1;O2"}}, // SPB 18:1;O2
            
            {"PIP(3') 16:0/18:1(9Z)", new List<string>{"PIP(3') 16:0/18:1(9Z)", "PIP(3') 16:0/18:1(9)", "PIP(3') 16:0_18:1", "PIP 34:1"}}, // PIP 34:1
            
            {"Cer 18:0;1OH,3OH,4OH/26:0;2OH,3OH", new List<string>{"Cer 18:0;1OH,3OH,4OH/26:0;2OH,3OH", "Cer 18:0;(OH)3/26:0;(OH)2", "Cer 18:0;O3/26:0;O2", "Cer 44:0;O5"}}, // Cer 44:0;O5
            
            {"Cer 18:1(5Z);1OH,3OH/14:0", new List<string>{"Cer 18:1(5Z);1OH,3OH/14:0", "Cer 18:1(5);(OH)2/14:0", "Cer 18:1;O2/14:0", "Cer 32:1;O2", "C32H63NO3"}}, // Cer 32:1;O2 / C32H63NO3
            
            {"PIP2(3',5') 17:0/20:4(5Z,8Z,11Z,14Z)", new List<string>{"PIP2(3',5') 17:0/20:4(5Z,8Z,11Z,14Z)", "PIP2(3',5') 17:0/20:4(5,8,11,14)", "PIP2(3',5') 17:0_20:4", "PIP2 37:4", "C46H83O19P3"}}, // PIP2 37:4 / C46H83O19P3
            
            {"PE O-16:0/18:2(9Z,12Z)", new List<string>{"PE O-16:0/18:2(9Z,12Z)", "PE O-16:0/18:2(9,12)", "PE O-16:0_18:2", "PE O-34:2", "C39H76NO7P"}}, // PE O-34:2 / C39H76NO7P
            
            {"PS-N(6:0) 16:0/18:3(9Z,12Z,15Z)", new List<string>{"PS-N(6:0) 16:0/18:3(9Z,12Z,15Z)", "PS-N(6:0) 16:0/18:3(9,12,15)", "PS-N(6:0) 16:0_18:3", "PS-N(Alk) 40:3"}}, // PS-N(Alk) 40:3
            
            {"TG 16:0;5O(FA 16:0[R])/18:1(9Z)/18:1(9Z)", new List<string>{"TG 16:0;5O(FA 16:0)/18:1(9Z)/18:1(9Z)", "TG 16:0;O(FA 16:0)/18:1(9)/18:1(9)", "TG 32:1;O2_18:1_18:1", "TG 68:3;O2"}}, // TG 68:3;O2
            
            {"TG O-18:1(9Z)/O-16:0/O-18:1(9Z)", new List<string>{"TG O-18:1(9Z)/O-16:0/O-18:1(9Z)", "TG O-18:1(9)/O-16:0/O-18:1(9)", "TG O-18:1_O-16:0_O-18:1", "TG tO-52:2"}}, // TG tO-52:2
            
            {"FA 22:4(4Z,7Z,10Z,18E);[13-17cy5;14OH,16OH];20OH", new List<string>{"FA 22:4(4Z,7Z,10Z,18E);[13-17cy5:0;14OH,16OH];20OH", "FA 22:4(4,7,10,18);[cy5:0;(OH)2];OH", "FA 22:5;O3", "FA 22:5;O3", "C22H34O5"}}, // FA 22:5;O3 / C22H34O5
            
            {"FA 20:2(5Z,13E);[8-13cy6;9OH,11OH;11oxy];15OH", new List<string>{"FA 20:2(5Z,13E);[8-13cy6:0;9OH,11OH;11oxy];15OH", "FA 20:2(5,13);[cy6:0;(OH)2;oxy];OH", "FA 20:3;O4", "FA 20:3;O4", "C20H34O6"}}, // FA 20:3;O4 / C20H34O6
            
            {"FA 20:4(6Z,8E,10E,14Z);5OH,12OH", new List<string>{"FA 20:4(6Z,8E,10E,14Z);5OH,12OH", "FA 20:4(6,8,10,14);(OH)2", "FA 20:4;O2", "FA 20:4;O2", "C20H32O4"}}, // FA 20:4;O2 / C20H32O4
            
            {"FA 22:0;4O(FA 10:0),5O(FA 10:0)", new List<string>{"FA 22:0;4O(FA 10:0),5O(FA 10:0)", "FA 22:0;O(FA 10:0),O(FA 10:0)", "FA 42:2;O4", "FA 42:2;O4", "C42H80O6"}}, //  42:2;O4
            
            {"FA 22:0;4O(FA 10:0);5O(10:0)", new List<string>{"FA 22:0;4O(FA 10:0);5O(10:0)", "FA 22:0;O(FA 10:0);O(10:0)", "FA 42:1;O3", "FA 42:1;O3"}}, //  42:0;O4
            
            {"DG 20:1(11Z)/22:2(13Z,16Z)/0:0", new List<string>{"DG 20:1(11Z)/22:2(13Z,16Z)/0:0", "DG 20:1(11)/22:2(13,16)/0:0", "DG 20:1_22:2", "DG 42:3", "C45H82O5"}},
            
            {"MG 0:0/O-6:0/0:0", new List<string>{"MG 0:0/O-6:0/0:0", "MG 0:0/O-6:0/0:0", "MG O-6:0", "MG O-6:0", "C9H20O3"}},
            
            {"MG 18:0/0:0/0:0", new List<string>{"MG 18:0/0:0/0:0", "MG 18:0/0:0/0:0", "MG 18:0", "MG 18:0", "C21H42O4"}},
            
            {"LCL 18:2(9Z,12Z)/18:2(9Z,12Z)/18:2(9Z,12Z)/0:0", new List<string>{"LCL 18:2(9Z,12Z)/18:2(9Z,12Z)/18:2(9Z,12Z)/0:0", "LCL 18:2(9,12)/18:2(9,12)/18:2(9,12)/0:0", "LCL 18:2_18:2_18:2", "LCL 54:6", "C63H112O16P2"}},
            
            {"MIPC(1) 20:0;3OH,4OH/20:0;2OH", new List<string>{"MIPC(1) 20:0;3OH,4OH/20:0;2OH", "MIPC 20:0;(OH)2/20:0;OH", "MIPC 20:0;O2/20:0;O", "MIPC 40:0;O4", "C52H102NO18P"}},

            {"LPC 20:1(11Z)/0:0", new List<string>{"LPC 20:1(11Z)/0:0", "LPC 20:1(11)/0:0", "LPC 20:1", "LPC 20:1", "C28H56NO7P"}},
            
            {"SPB 18:0;1OH,3OH", new List<string>{"SPB 18:0;1OH,3OH", "SPB 18:0;(OH)2", "SPB 18:0;O2", "SPB 18:0;O2", "C18H39NO2"}},
            
            {"LPIM1 19:1(9Z)/0:0", new List<string>{"LPIM1 19:1(9Z)/0:0", "LPIM1 19:1(9)/0:0", "LPIM1 19:1", "LPIM1 19:1", "C34H63O17P"}},

            {"Hex2Cer(1) 17:1(5E);15Me;3OH,4OH/22:0;2OH", new List<string>{"Hex2Cer(1) 17:1(5E);15Me;3OH,4OH/22:0;2OH", "Hex2Cer 17:1(5);Me;(OH)2/22:0;OH", "Hex2Cer 18:1;O2/22:0;O", "Hex2Cer 40:1;O4", "C52H99NO15"}},
            
            {"Glc-Cer(1) 21:0;[13-15cy3:0];3OH,4OH/22:1(16E);2OH;15oxo", new List<string>{"Glc-Cer(1) 21:0;[13-15cy3:0];3OH,4OH/22:1(16E);2OH;15oxo", "Glc-Cer 21:0;[cy3:0];(OH)2/22:1(16);OH;oxo", "GlcCer 21:1;O2/22:2;O2", "GlcCer 43:3;O5", "C49H91NO11"}},
            
            {"FA 8:0;[6-8SScy5:0]", new List<string>{"FA 8:0;[6-8SScy5:0]", "FA 8:0;[SScy5:0]", "FA 8:1;S2", "FA 8:1;S2", "C8H14O2S2"}},
            
            {"FA 20:0;[12-15Ocy5:2(12E,13E);13Me,14Me]", new List<string>{"FA 20:0;[12-15Ocy5:2(12E,13E);13Me,14Me]", "FA 20:0;[Ocy5:2(12,13);(Me)2]", "FA 22:3;O", "FA 22:3;O", "C22H38O3"}},
            
            {"DG O-16:0/0:0/O-16:1(9Z)", new List<string>{"DG O-16:0/0:0/O-16:1(9Z)", "DG O-16:0/0:0/O-16:1(9)", "DG O-16:0_O-16:1", "DG dO-32:1", "C35H70O3"}}
        };
        
        
        [Fact]
        public void ShorthandUnitTest1()
        {
            ShorthandParser parser = new ShorthandParser();
            
            foreach (KeyValuePair<string, List<string> > row in data)
            {
                string lipid_name = row.Key;
                List<string> results = row.Value;
                
                LipidAdduct lipid = parser.parse(lipid_name);
                string formula = results.Count > 4 ? results[4] : lipid.get_sum_formula();
                
                
                if (results.Count > 4)
                {
                    Assert.True(formula.Equals(lipid.get_sum_formula()), "test on lipid '" + lipid_name + "'");
                }
                
                for (int l = 0; l < levels.Count; ++l)
                {
                    LipidLevel lipid_level = levels[l];
                    string n = lipid.get_lipid_string(lipid_level);
                    Assert.True(results[l].Equals(n), "test 1 on lipid '" + lipid_name + "' and level '" + Convert.ToString(lipid_level) + "'");
                    Assert.True(formula.Equals(lipid.get_sum_formula()), "test 2 on lipid '" + lipid_name + "' and level '" + Convert.ToString(lipid_level) + "'");

                    
                    LipidAdduct lipid2 = parser.parse(n);
                    for (int ll = l; ll < 4; ++ll)
                    {
                        Assert.True(results[ll].Equals(lipid2.get_lipid_string(levels[ll])), "test 3 on lipid '" + lipid_name + "' and level '" + Convert.ToString(levels[ll]) + "'");
                        Assert.True(formula.Equals(lipid2.get_sum_formula()), "test 4 on lipid '" + lipid_name + "' and level '" + Convert.ToString(levels[l]) + "' mapped to level '" + Convert.ToString(levels[ll]) + "'");
                    }
                }
            }
            
            Console.WriteLine("Shorthand Test: All tests passed without any problem");
        }
    }
}
