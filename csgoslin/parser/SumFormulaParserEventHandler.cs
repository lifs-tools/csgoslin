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
    
    public class SumFormulaParserEventHandler : BaseParserEventHandler<ElementTable>
    {
        Element element;
        int count;
        
        public SumFormulaParserEventHandler() : base()
        {
            content = StringFunctions.create_empty_table();
            element = Element.H;
            count = 0;
            
            registered_events.Add("molecule_pre_event", reset_parser);
            registered_events.Add("element_group_post_event", element_group_post_event);
            registered_events.Add("element_pre_event", element_pre_event);
            registered_events.Add("single_element_pre_event", single_element_group_pre_event);
            registered_events.Add("count_pre_event", count_pre_event);
        }
        
    
        void reset_parser(TreeNode node)
        {
            content = StringFunctions.create_empty_table();
        }


        void element_group_post_event(TreeNode node)
        {
            content[element] += count;
        }


        void element_pre_event(TreeNode node)
        {
            string parsed_element = node.get_text();
            
            if (Elements.element_positions.ContainsKey(parsed_element))
            {
                element = Elements.element_positions[parsed_element];
            }
                    
            else {
                throw new LipidException("Error: element '" + parsed_element + "' is unknown");
            }
        }


        void single_element_group_pre_event(TreeNode node)
        {
            string parsed_element = node.get_text();
            if (Elements.element_positions.ContainsKey(parsed_element))
            {
                element = Elements.element_positions[parsed_element];
                content[element] += 1;
            }
                
            else {
                throw new LipidException("Error: element '" + parsed_element + "' is unknown");
            }
        }


        void count_pre_event(TreeNode node)
        {
            count = node.int();
        }
    
    }
}
