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
using System.Collections.Generic;

namespace csgoslin
{
    public abstract class BaseParserEventHandler<T>
    {
        public Dictionary<string, Action<TreeNode>> registered_events = new Dictionary<string, Action<TreeNode>>();
        public HashSet<string> rule_names = new HashSet<string>();
        public T content = default(T);
        public string debug = "";
        public string error_message;
        public bool word_in_grammar;
    
        public BaseParserEventHandler()
        {
            registered_events = new Dictionary<string, Action<TreeNode>>();
            rule_names = new HashSet<string>();
            debug = "";
            error_message = "";
            word_in_grammar = false;
        }
        
        
        // checking if all registered events are reasonable and orrur as rules in the grammar
        public void sanity_check()
        {
            foreach (string event_name in registered_events.Keys)
            {
                if (!event_name.EndsWith("_pre_event") && !event_name.EndsWith("_post_event"))
                {
                    throw new Exception("Parser event handler error: event '" + event_name + "' does not contain the suffix '_pre_event' or '_post_event'");
                }
                string rule_name = event_name.Replace("_pre_event", "").Replace("_post_event", "");
                if (!rule_names.Contains(rule_name))
                {
                    throw new Exception("Parser event handler error: rule '" + rule_name + "' in event '" + event_name + "' is not present in the grammar");
                }
            }
        }
        
        
        public void handle_event(string event_name, TreeNode node)
        {
            if (debug == "full")
            {
                string reg_event = registered_events.ContainsKey(event_name) ? "*" : "";
                Console.WriteLine(event_name + reg_event + ": \"" + node.get_text() + "\"");
            }
            
            if (registered_events.ContainsKey(event_name))
            {
                if (debug != "" && debug != "full")
                {
                    Console.WriteLine(event_name + ": \"" + node.get_text() + "\"");
                }
                registered_events[event_name](node);
            }
        }
    }    
}
