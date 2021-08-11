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
    public class ShorthandParser : Parser<LipidAdduct>
    {
        public ShorthandParser() : base(new ShorthandParserEventHandler(), new GrammarString(KnownGrammars.shorthand_grammar), StringFunctions.DEFAULT_QUOTE)
        {
            
        }
    }
    
    
    public class GoslinParser : Parser<LipidAdduct>
    {
        public GoslinParser() : base(new GoslinParserEventHandler(), new GrammarString(KnownGrammars.goslin_grammar), StringFunctions.DEFAULT_QUOTE)
        {
            
        }
    }
    
    
    public class LipidMapsParser : Parser<LipidAdduct>
    {
        public LipidMapsParser() : base(new LipidMapsParserEventHandler(), new GrammarString(KnownGrammars.lipid_maps_grammar), StringFunctions.DEFAULT_QUOTE)
        {
            
        }
    }
    
    
    public class SwissLipidsParser : Parser<LipidAdduct>
    {
        public SwissLipidsParser() : base(new SwissLipidsParserEventHandler(), new GrammarString(KnownGrammars.swiss_lipids_grammar), StringFunctions.DEFAULT_QUOTE)
        {
            
        }
    }
    
    
    public class HmdbParser : Parser<LipidAdduct>
    {
        public HmdbParser() : base(new HmdbParserEventHandler(), new GrammarString(KnownGrammars.hmdb_grammar), StringFunctions.DEFAULT_QUOTE)
        {
            
        }
    }

    
    public class LipidParser
    {
        public List< Parser<LipidAdduct> > parser_list = new List< Parser<LipidAdduct> >();
        public Parser<LipidAdduct> lastSuccessfulParser = null;
        
        public LipidParser()
        {
            parser_list.Add(new ShorthandParser());
            parser_list.Add(new GoslinParser());
            //parser_list.Add(new FattyAcidParser());
            parser_list.Add(new LipidMapsParser());
            parser_list.Add(new SwissLipidsParser());
            parser_list.Add(new HmdbParser());
            
            lastSuccessfulParser = null;
        }
        
        
        public LipidAdduct parse(string lipid_name)
        {
            lastSuccessfulParser = null;
            
            foreach (Parser<LipidAdduct> parser in parser_list)
            {
                LipidAdduct lipid = parser.parse(lipid_name, false);
                if (lipid != null)
                {
                    lastSuccessfulParser = parser;
                    return lipid;
                }
            }
            throw new LipidException("Lipid not found");
        }
    }
}
