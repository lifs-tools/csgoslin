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
    public class HmdbUnitTest
    {

        [Fact]
        public void HmdbUnitTest1()
        {
            string prefixPath = Environment.CurrentDirectory;
            string test_file_name = Path.Combine(prefixPath, "..", "..", "..", "..", "data", "goslin", "testfiles", "hmdb-test.csv");
        
            HmdbParser lipid_maps_parser = new HmdbParser();
                
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
                LipidAdduct lipid = lipid_maps_parser.parse(lipid_name);
                
                Assert.True(lipid != null);
            }
            
            Console.WriteLine("HMDB Test: All tests passed without any problem");
        }
    }
}

