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

namespace csgoslin
{
    public enum Context {NoContext, InLineComment, InLongComment, InQuote};
    public enum MatchWords {NoMatch, LineCommentStart, LineCommentEnd, LongCommentStart, LongCommentEnd, Quote};
        
    public class Parser<T>
    {
        public static readonly int SHIFT = 32;
        public static readonly ulong MASK = (1UL << SHIFT) - 1;
        public static readonly char RULE_ASSIGNMENT = ':';
        public static readonly char RULE_SEPARATOR = '|';
        public static readonly char RULE_TERMINAL = ';';
        public static readonly char EOF_SIGN = (char)1;
        public static readonly ulong EOF_RULE = 1L;
        public static readonly ulong START_RULE = 2L;
        public static readonly string EOF_RULE_NAME = "EOF";
        public static readonly char[] SPACE_TRIM = new char[]{' '};
        
        public ulong next_free_rule_index;
        public Dictionary<char, HashSet<ulong>> TtoNT = new Dictionary<char, HashSet<ulong>>();
        public Dictionary<char, ulong> originalTtoNT = new Dictionary<char, ulong>();
        public Dictionary<ulong, HashSet<ulong>> NTtoNT = new Dictionary<ulong, HashSet<ulong>>();
        public Dictionary<ulong, string> NTtoRule = new Dictionary<ulong, string>();
        public Dictionary<ulong, List<ulong> > substitution = new Dictionary<ulong, List<ulong> >();
        public List<Bitfield> right_pair = new List<Bitfield>();
        public int avg_pair;
        public char quote;
        public BaseParserEventHandler<T> parser_event_handler = null;
        public bool word_in_grammar = false;
        public string grammar_name = "";
        public bool used_eof = false;
        
        
        
        
        public Parser(BaseParserEventHandler<T> _parserEventHandler, GrammarString grammar_string, char _quote = (char)0)
        {
            
            quote = (_quote != 0) ? _quote : StringFunctions.DEFAULT_QUOTE;
            parser_event_handler = _parserEventHandler;
            
            read_grammar(grammar_string.ToString());
        }


        public Parser(BaseParserEventHandler<T> _parserEventHandler, string grammar_filename, char _quote = (char)0)
        {
            
            quote = (_quote != 0) ? _quote : StringFunctions.DEFAULT_QUOTE;
            parser_event_handler = _parserEventHandler;
            
            
            if (File.Exists(grammar_filename))
            {
                string grammar = File.ReadAllText(grammar_filename);
                read_grammar(grammar);
            }
            else
            {
                throw new RuntimeException("Error: file '" + grammar_filename + "' does not exist.");
            }
        }
        
        ulong get_next_free_rule_index()
        {
            if (next_free_rule_index <= MASK){
                return next_free_rule_index++;
            }
            throw new RuntimeException("Error: grammar is too big.");
        }

            
        public void read_grammar(string grammar)
        {
            next_free_rule_index = START_RULE;
            word_in_grammar = false;
            grammar_name = "";
            used_eof = false;
            Dictionary<string, ulong> ruleToNT = new Dictionary<string, ulong>();
            
            
            // interpret the rules and create the structure for parsing
            List<string> rules = extract_text_based_rules(grammar, quote);
            List<string> tokens = StringFunctions.split_string(rules[0], ' ', quote);
            grammar_name = tokens[1];
            
            rules.RemoveAt(0);
            ruleToNT.Add(EOF_RULE_NAME, EOF_RULE);
            TtoNT.Add(EOF_SIGN, new HashSet<ulong>());
            TtoNT[EOF_SIGN].Add(EOF_RULE);
            
            foreach (string rule_line in rules)
            {
                List<string> tokens_level_1 = new List<string>();
                List<string> line_tokens = StringFunctions.split_string(rule_line, RULE_ASSIGNMENT, quote);
                foreach (string t in line_tokens) tokens_level_1.Add(t.Trim(SPACE_TRIM));
                    
                if (tokens_level_1.Count != 2)
                {
                    throw new RuntimeException("Error: corrupted token in grammar rule: '" + rule_line + "'");
                }
                
                List<string> rule_tokens = StringFunctions.split_string(tokens_level_1[0], ' ', quote);
                if (rule_tokens.Count > 1)
                {
                    throw new RuntimeException("Error: several rule names on left hand side in grammar rule: '" + rule_line + "'");
                }

                string rule = tokens_level_1[0];
                
                if (rule == EOF_RULE_NAME)
                {
                    throw new RuntimeException("Error: rule name is not allowed to be called EOF");
                }
                
                List<string> products = StringFunctions.split_string(tokens_level_1[1], RULE_SEPARATOR, quote);
                for (int i = 0; i < products.Count; ++i)
                {
                    products[i] = products[i].Trim(SPACE_TRIM);
                }
                
                if (!ruleToNT.ContainsKey(rule))
                {
                    ruleToNT.Add(rule, get_next_free_rule_index());
                }
                ulong new_rule_index = ruleToNT[rule];
                
                if (!NTtoRule.ContainsKey(new_rule_index))
                {
                    NTtoRule.Add(new_rule_index, rule);
                }
                
                
                foreach (string product in products)
                {
                    List<string> non_terminals = new List<string>();
                    List<ulong> non_terminal_rules = new List<ulong>();
                    List<string> product_rules = StringFunctions.split_string(product, ' ', quote);
                    foreach (string NT in product_rules)
                    {
                        string stripedNT = NT.Trim(SPACE_TRIM);
                        if (is_terminal(stripedNT, quote)) stripedNT = de_escape(stripedNT, quote);
                        non_terminals.Add(stripedNT);
                        used_eof |= (stripedNT == EOF_RULE_NAME);
                    }
                    
                    string NTFirst = non_terminals[0];
                    if (non_terminals.Count > 1 || !is_terminal(NTFirst, quote) || NTFirst.Length != 3)
                    {
                        foreach (string non_terminal in non_terminals)
                        {
                            
                            if (is_terminal(non_terminal, quote))
                            {
                                non_terminal_rules.Add(add_terminal(non_terminal));
                            }
                                
                            else
                            {
                                if (!ruleToNT.ContainsKey(non_terminal))
                                {
                                    ruleToNT.Add(non_terminal, get_next_free_rule_index());
                                }
                                non_terminal_rules.Add(ruleToNT[non_terminal]);
                            }
                        }
                    }
                    else
                    {
                        char c = NTFirst[1];
                        ulong tRule = 0;
                        if (!TtoNT.ContainsKey(c))
                        {
                            tRule = get_next_free_rule_index();
                            TtoNT.Add(c, new HashSet<ulong>());
                            TtoNT[c].Add(tRule);
                            
                        }
                        else {
                            tRule = (new List<ulong>(TtoNT[c]))[0];
                        }
                        
                        if (!NTtoNT.ContainsKey(tRule)) NTtoNT.Add(tRule, new HashSet<ulong>());
                        NTtoNT[tRule].Add(new_rule_index);
                    }
                    
                    // more than two rules, insert intermediate rule indexes
                    while (non_terminal_rules.Count > 2)
                    {
                        ulong rule_index_2 = non_terminal_rules[non_terminal_rules.Count - 1];
                        non_terminal_rules.RemoveAt(non_terminal_rules.Count - 1);
                        ulong rule_index_1 = non_terminal_rules[non_terminal_rules.Count - 1];
                        non_terminal_rules.RemoveAt(non_terminal_rules.Count - 1);
                        
                        ulong key = compute_rule_key(rule_index_1, rule_index_2);
                        ulong next_index = get_next_free_rule_index();
                        if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<ulong>());
                        NTtoNT[key].Add(next_index);
                        non_terminal_rules.Add(next_index);
                    }
                        
                    // two product rules
                    if (non_terminal_rules.Count == 2)
                    {
                        ulong rule_index_2 = non_terminal_rules[1];
                        ulong rule_index_1 = non_terminal_rules[0];
                        ulong key = compute_rule_key(rule_index_1, rule_index_2);
                        if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<ulong>());
                        NTtoNT[key].Add(new_rule_index);
                    }
                    
                    // only one product rule
                    else if (non_terminal_rules.Count == 1)
                    {
                        ulong rule_index_1 = non_terminal_rules[0];
                        if (rule_index_1 == new_rule_index)
                        {
                            throw new RuntimeException("Error: corrupted token in grammar: rule '" + rule + "' is not allowed to refer soleley to itself.");
                        }
                        
                        if (!NTtoNT.ContainsKey(rule_index_1)) NTtoNT.Add(rule_index_1, new HashSet<ulong>());
                        NTtoNT[rule_index_1].Add(new_rule_index);
                    }
                }
            }
            
            // adding all rule names into the event handler
            foreach (KeyValuePair<string, ulong> rule_name in ruleToNT) parser_event_handler.rule_names.Add(rule_name.Key);
                
            parser_event_handler.sanity_check();
            
            
            // keeping the original terminal dictionary
            foreach (KeyValuePair<char, HashSet<ulong>> kv in TtoNT)
            {
                foreach (ulong rule in kv.Value)
                {
                    originalTtoNT.Add(kv.Key, rule);
                    break;
                }
            }
            
            
            // creating substitution dictionary for adding single rule chains into the parsing tree
            HashSet<ulong> visited = new HashSet<ulong>();
            foreach (KeyValuePair<ulong, HashSet<ulong>> kv in NTtoNT)
            {
                HashSet<ulong> values = new HashSet<ulong>();
                values.Add(kv.Key);
                foreach (ulong rule in values)
                {
                    if (visited.Contains(rule)) continue;
                    visited.Add(rule);
                    
                    List<ulong> topnodes = collect_one_backwards(rule);
                    foreach (ulong rule_top in topnodes)
                    {
                        List< List<ulong> > chains = collect_backwards(rule, rule_top);
                        
                        foreach (List<ulong> cchain in chains)
                        {
                            List<ulong> chain = cchain;
                            while (chain.Count > 1)
                            {
                                ulong top = chain[0];
                                chain.RemoveAt(0);
                                ulong key = kv.Key + unchecked(top << 16);
                                if (!substitution.ContainsKey(key))
                                {
                                    substitution.Add(key, chain);
                                
                                    if (chain.Count > 1){
                                        List<ulong> new_chain = new List<ulong>();
                                        foreach (ulong e in chain) new_chain.Add(e);
                                        chain = new_chain;
                                    }
                                }
                                else {
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // expanding terminal dictionary for single rule chains
            HashSet<char> keys = new HashSet<char>();
            foreach (KeyValuePair<char, HashSet<ulong>> key in TtoNT) keys.Add(key.Key);
            foreach (char c in keys)
            {
                HashSet<ulong> k_rules = new HashSet<ulong>();
                foreach (ulong rule in TtoNT[c]) k_rules.Add(rule);
                                    
                foreach (ulong rule in k_rules)
                {
                    List<ulong> backward_rules = collect_one_backwards(rule);
                    foreach (ulong p in backward_rules) TtoNT[c].Add(p);
                }
            }
            
            
            
            // expanding non-terminal dictionary for single rule chains
            HashSet<ulong> keysNT = new HashSet<ulong>();
            foreach (KeyValuePair<ulong, HashSet<ulong> > k in NTtoNT) keysNT.Add(k.Key);
            foreach (ulong r in keysNT)
            {
                HashSet<ulong> k_rules = new HashSet<ulong>();
                foreach (ulong rr in NTtoNT[r]) k_rules.Add(rr);
                                                                        
                foreach (ulong rule in k_rules)
                {
                    List<ulong> backward_rules = collect_one_backwards(rule);
                    foreach (ulong p in backward_rules) NTtoNT[r].Add(p);
                }
            }

            
            // creating lookup table for right index pairs to a given left index
            for (ulong i = 0; i < next_free_rule_index; ++i){
                right_pair.Add(new Bitfield((int)next_free_rule_index));
            }
            
            
            foreach (KeyValuePair<ulong, HashSet<ulong> > kvp in NTtoNT)
            {
                if (kvp.Key <= MASK) continue;
                right_pair[(int)(unchecked(kvp.Key >> SHIFT))].Add((int)(kvp.Key & MASK));
            }
        }


        public List<string> extract_text_based_rules(string grammar, char _quote)
        {
            List<string> rules = null;
            int grammar_length = grammar.Length;
            
            /*
            deleting comments to prepare for splitting the grammar in rules.
            Therefore, we have to consider three different contexts, namely
            within a quote, within a line comment, within a long comment.
            As long as we are in one context, key words for starting / ending
            the other contexts have to be ignored.
            */
            StringBuilder sb = new StringBuilder();
            Context current_context = Context.NoContext;
            int current_position = 0;
            int last_escaped_backslash = -1;
            
            for (int i = 0; i < grammar_length - 1; ++i)
            {
                MatchWords match = MatchWords.NoMatch;
                
                if (i > 0 && grammar[i] == '\\' && grammar[i - 1] == '\\' && last_escaped_backslash != i - 1)
                {
                    last_escaped_backslash = i;
                    continue;
                }
                
                if (grammar[i] == '/' && grammar[i + 1] == '/') match = MatchWords.LineCommentStart;
                else if (grammar[i] == '\n') match = MatchWords.LineCommentEnd;
                else if (grammar[i] == '/' && grammar[i + 1] == '*') match = MatchWords.LongCommentStart;
                else if (grammar[i] == '*' && grammar[i + 1] == '/') match = MatchWords.LongCommentEnd;
                else if (grammar[i] == _quote &&  !(i >= 1 && grammar[i - 1] == '\\' && i - 1 != last_escaped_backslash)) match = MatchWords.Quote;
                
                if (match != MatchWords.NoMatch)
                {
                    switch (current_context)
                    {
                        case Context.NoContext:
                            switch (match){
                                case MatchWords.LongCommentStart:
                                    sb.Append(grammar.Substring(current_position, i - current_position));
                                    current_context = Context.InLongComment;
                                    break;
                                    
                                case MatchWords.LineCommentStart:
                                    sb.Append(grammar.Substring(current_position, i - current_position));
                                    current_context = Context.InLineComment;
                                    break;
                                    
                                case MatchWords.Quote:
                                    current_context = Context.InQuote;
                                    break;
                                    
                                default:
                                    break;
                            }
                            break;
                            
                        case Context.InQuote:
                            if (match == MatchWords.Quote)
                            {
                                current_context = Context.NoContext;
                            }
                            break;
                            
                            
                        case Context.InLineComment:
                            if (match == MatchWords.LineCommentEnd)
                            {
                                current_context = Context.NoContext;
                                current_position = i + 1;
                            }
                            break;
                            
                        case Context.InLongComment:
                            if (match == MatchWords.LongCommentEnd)
                            {
                                current_context = Context.NoContext;
                                current_position = i + 2;
                            }
                            break;
                            
                        default:
                            break;
                    }
                }
            }
            
            if (current_context == Context.NoContext)
            {
                sb.Append(grammar.Substring(current_position, grammar_length - current_position));
            }
            else
            {
                throw new RuntimeException("Error: corrupted grammar, ends either in comment or quote");
            }
            
            grammar = sb.ToString();
            grammar = grammar.Replace("\r\n", "");
            grammar = grammar.Replace("\n", "");
            grammar = grammar.Replace("\r", "");
            grammar = grammar.Trim(SPACE_TRIM);
            
            
            if (grammar[grammar.Length - 1] != RULE_TERMINAL)
            {
                throw new RuntimeException("Error: corrupted grammar, last rule has no termininating sign, was: '" + grammar[grammar.Length - 1] + "'");
            }
            
            rules = StringFunctions.split_string(grammar, RULE_TERMINAL, _quote);
            
            if (rules.Count < 1)
            {
                throw new RuntimeException("Error: corrupted grammar, grammar is empty");
            }
            List<string> grammar_name_rule = StringFunctions.split_string(rules[0], ' ', _quote);
            
            if (grammar_name_rule.Count > 0 && grammar_name_rule[0] != "grammar")
            {
                throw new RuntimeException("Error: first rule must start with the keyword 'grammar'");
            }
            
            
            else if (grammar_name_rule.Count != 2)
            {
                throw new RuntimeException("Error: incorrect first rule");
            }
            
            return rules;
        }


        public ulong compute_rule_key(ulong rule_index_1, ulong rule_index_2)
        {
            return (unchecked(rule_index_1 << SHIFT)) | rule_index_2;
        }


        // checking if string is terminal
        public bool is_terminal(string product_token, char _quote)
        {
            return product_token[0] == _quote && product_token[product_token.Length - 1] == _quote && product_token.Length > 2;
        }




        public string de_escape(string text, char _quote)
        {
            // remove the escape chars
            StringBuilder sb = new StringBuilder();
            bool last_escape_char = false;
            for (int i = 0; i < text.Length; ++i){
                char c = text[i];
                bool escape_char = false;
                
                if (c != '\\') sb.Append(c);
                    
                else
                {
                    if (!last_escape_char)
                    {
                        escape_char = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                
                last_escape_char = escape_char;
            
            }
            return sb.ToString();
        }


        // splitting the whole terminal in a tree structure where characters of terminal are the leafs and the inner nodes are added non terminal rules
        public ulong add_terminal(string text)
        {
            List<ulong> terminal_rules = new List<ulong>();
            for (int i = 1; i < text.Length - 1; ++i)
            {
                char c = text[i];
                ulong tRule = 0;
                if (!TtoNT.ContainsKey(c)){
                    tRule = get_next_free_rule_index();
                    TtoNT.Add(c, new HashSet<ulong>());
                    TtoNT[c].Add(tRule);
                }
                else
                {
                    tRule = (new List<ulong>(TtoNT[c]))[0];
                }
                terminal_rules.Add(tRule);
            }
            
            while (terminal_rules.Count > 1)
            {
                ulong rule_index_2 = terminal_rules[terminal_rules.Count - 1];
                terminal_rules.RemoveAt(terminal_rules.Count - 1);
                ulong rule_index_1 = terminal_rules[terminal_rules.Count - 1];
                terminal_rules.RemoveAt(terminal_rules.Count - 1);
                
                ulong next_index = get_next_free_rule_index();
                
                ulong key = compute_rule_key(rule_index_1, rule_index_2);
                if (!NTtoNT.ContainsKey(key)) NTtoNT.Add(key, new HashSet<ulong>());
                NTtoNT[key].Add(next_index);
                terminal_rules.Add(next_index);
            }
            return terminal_rules[0];
        }


        public List<ulong> top_nodes(ulong rule_index)
        {
            List<ulong> collection = new List<ulong>();
            List<ulong> collection_top = new List<ulong>();
            collection.Add(rule_index);
            int i = 0;
            while (i < collection.Count)
            {
                ulong current_index = collection[i];
                if (!NTtoNT.ContainsKey(current_index))
                {
                    foreach (ulong previous_index in NTtoNT[current_index]) collection.Add(previous_index);
                }
                else 
                {
                    collection_top.Add(current_index);
                }
                ++i;
            }
            
            return collection_top;
        }


        // expanding singleton rules, e.g. S . A, A . B, B . C
        public List<ulong> collect_one_backwards(ulong rule_index){
            List<ulong> collection = new List<ulong>();
            collection.Add(rule_index);
            int i = 0;
            while (i < collection.Count)
            {
                ulong current_index = collection[i];
                if (NTtoNT.ContainsKey(current_index))
                {
                    foreach (ulong previous_index in NTtoNT[current_index]) collection.Add(previous_index);
                }
                ++i;
            }
            
            return collection;
        }



        public List< List<ulong> > collect_backwards(ulong child_rule_index, ulong parent_rule_index)
        {
            HashSet<ulong> visited = new HashSet<ulong>();
            List<ulong> path = new List<ulong>();
            List< List<ulong> > collection = new List< List<ulong> >();
            
            return collect_backwards(child_rule_index, parent_rule_index, visited, path, collection);
        }


        public List< List<ulong> > collect_backwards(ulong child_rule_index, ulong parent_rule_index, HashSet<ulong> visited, List<ulong> path, List< List<ulong> > collection){
            // provides all single linkage paths from a child rule to a parent rule,
            // and yes, there can be several paths
            
            if (!NTtoNT.ContainsKey(child_rule_index))
            {
                return collection;
            }
            
            
            visited.Add(child_rule_index);
            path.Add(child_rule_index);
            
            foreach (ulong previous_rule in NTtoNT[child_rule_index])
            {
                if (!visited.Contains(previous_rule))
                {
                    if (previous_rule == parent_rule_index)
                    {
                        List<ulong> found_path = new List<ulong>();
                        found_path.Add(parent_rule_index);
                        for (int i = path.Count - 1; i >= 0; --i) found_path.Add(path[i]);
                        collection.Add(found_path);
                    }
                    
                    else
                    {
                        collection = collect_backwards(previous_rule, parent_rule_index, visited, path, collection);
                    }
                }
            }
            path.RemoveAt(path.Count - 1);
            visited.Remove(child_rule_index);
            
            return collection;
        }
            
            
            

        public void raise_events(TreeNode node)
        {
            if (node != null)
            {
                string node_rule_name = node.fire_event ? NTtoRule[node.rule_index] : "";
                if (node.fire_event) parser_event_handler.handle_event(node_rule_name + "_pre_event", node);
                
                if (node.left != null)
                { // node.terminal is != None when node is leaf
                    raise_events(node.left);
                    if (node.right != null) raise_events(node.right);
                }
                    
                if (node.fire_event) parser_event_handler.handle_event(node_rule_name + "_post_event", node);
            }
        }





        // filling the syntax tree including events
        public void fill_tree(TreeNode node, DPNode dp_node){
            // checking and extending nodes for single rule chains
            
            ulong bottom_rule = 0, top_rule = 0;
            if (dp_node.left != null)
            {
                bottom_rule = compute_rule_key(dp_node.rule_index_1, dp_node.rule_index_2);
                top_rule = node.rule_index;
            }
            else
            {
                top_rule = dp_node.rule_index_2;
                bottom_rule = originalTtoNT[(char)dp_node.rule_index_1];
            }
            
            ulong subst_key = bottom_rule + unchecked(top_rule << 16);
            
            if ((bottom_rule != top_rule) && (substitution.ContainsKey(subst_key)))
            {
                foreach (ulong rule_index in substitution[subst_key])
                {
                    node.left = new TreeNode(rule_index, NTtoRule.ContainsKey(rule_index));
                    node = node.left;
                }
            }


            
            if (dp_node.left != null) // None => leaf
            {
                node.left = new TreeNode(dp_node.rule_index_1, NTtoRule.ContainsKey(dp_node.rule_index_1));
                node.right = new TreeNode(dp_node.rule_index_2, NTtoRule.ContainsKey(dp_node.rule_index_2));
                fill_tree(node.left, dp_node.left);
                fill_tree(node.right, dp_node.right);
            }
            else
            {
                // I know, it is not 100% clean to store the character in an integer
                // especially when it is not the dedicated attribute for, but the heck with it!
                node.terminal = (char)dp_node.rule_index_1;
            }
        }



        // re-implementation of Cocke-Younger-Kasami algorithm
        public virtual T parse(string text_to_parse, bool throw_error = true)
        {
            string old_lipid = text_to_parse;
            if (used_eof) text_to_parse += EOF_SIGN;
            parser_event_handler.content = default(T);
            
            parse_regular(text_to_parse);
            if (throw_error && !word_in_grammar)
            {
                throw new LipidParsingException("Lipid '" + old_lipid + "' can not be parsed by grammar '" + grammar_name + "'");
            }
            
            return parser_event_handler.content;
        }
            
            
            
            
        public void parse_regular(string text_to_parse)
        {
            word_in_grammar = false;
            
            int n = text_to_parse.Length;
            // dp stands for dynamic programming, nothing else
            Dictionary<ulong, DPNode>[][] DP = new Dictionary<ulong, DPNode>[n][];
            
            // Ks is a lookup, which fields in the DP are filled
            Bitfield[] Ks = new Bitfield[n];
            
            
            // init the tables
            for (int i = 0; i < n; ++i){
                DP[i] = new Dictionary<ulong, DPNode>[n - i];
                for (int j = 0; j < n - i; ++j){
                    DP[i][j] = new Dictionary<ulong, DPNode>();
                }
                Ks[i] = new Bitfield(n);
            }
            
            bool requirement_fulfilled = true;
            for (int i = 0; i < n; ++i)
            {
                char c = text_to_parse[i];
                if (!TtoNT.ContainsKey(c))
                {
                    requirement_fulfilled = false;
                    break;
                }
                    
                foreach (ulong T_rule_index in TtoNT[c])
                {
                    DPNode dp_node = new DPNode(c, T_rule_index, null, null);
                    DP[i][0].Add(T_rule_index, dp_node);
                }
                Ks[i].Add(0);
            }
            
            if (requirement_fulfilled)
            {
                for (int i = 1; i < n; ++i)
                {
                    int im1 = i - 1;
                    
                    for (int j = 0; j < n - i; ++j)
                    {
                        
                        Dictionary<ulong, DPNode>[] DPj = DP[j];
                        Dictionary<ulong, DPNode> DPji = DPj[i];
                        int jp1 = j + 1;
                        
                        foreach (int k in Ks[j].getBitPositions())
                        {
                            int jpok = jp1 + k;
                            int im1mk = im1 - k;
                            if (Ks[jpok].find(im1mk))
                            {
                                foreach (KeyValuePair<ulong, DPNode> index_pair_1 in DP[j][k])
                                {
                                    Bitfield b = right_pair[(int)index_pair_1.Key];
                                    foreach (KeyValuePair<ulong, DPNode> index_pair_2 in DP[jpok][im1mk])
                                    {
                                        
                                        if (b.find((int)index_pair_2.Key))
                                        {
                                            ulong key = compute_rule_key(index_pair_1.Key, index_pair_2.Key);
                                            
                                            DPNode content = new DPNode(index_pair_1.Key, index_pair_2.Key, index_pair_1.Value, index_pair_2.Value);
                                            foreach (ulong rule_index in NTtoNT[key])
                                            {
                                                if (!DPji.ContainsKey(rule_index))
                                                {
                                                    DPji.Add(rule_index, content);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                        if (DPji.Count > 0) Ks[j].Add(i);
                    }
                }
                
                
                for (int i = n - 1; i > 0; --i)
                {
                    if (DP[0][i].ContainsKey(START_RULE))
                    {
                        word_in_grammar = true;
                        TreeNode parse_tree = new TreeNode(START_RULE, NTtoRule.ContainsKey(START_RULE));
                        fill_tree(parse_tree, DP[0][i][START_RULE]);
                        raise_events(parse_tree);
                        break;
                    }
                }
            
            }
        }
    }

}
