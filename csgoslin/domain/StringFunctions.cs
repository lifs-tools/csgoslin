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
     
    
    
    public class ExendedList<T> : List<T>
    {
        public T back()
        {
            if (Count > 0) return this[Count - 1];
            throw new Exception("List is empty");
        }
        
        public T PopBack()
        {
            if (Count > 0) 
            {
                T t = back();
                RemoveAt(Count - 1);
                return t;
            }
            throw new Exception("List is empty");
        }
    }
    
    
    public class Dict : Dictionary<string, Object>
    {
        public new void Add(string s, Object o)
        {
            if (ContainsKey(s)) this[s] = o;
            else base.Add(s, o);
        }
    }
    
    public class StringFunctions
    {
        
        public static char DEFAULT_QUOTE = '\'';
        public static string compute_sum_formula(ElementTable elements)
        {
            StringBuilder ss = new StringBuilder();
            
            foreach (Element e in Elements.element_order)
            {
                if (elements[e] > 0) ss.Append(Elements.element_shortcut[e]);
                if (elements[e] > 1) ss.Append(elements[e]);
            }
            return ss.ToString();
        }



        public static ElementTable create_empty_table(){
            return new ElementTable{{Element.C, 0}, {Element.C13, 0}, {Element.H, 0}, {Element.H2, 0}, {Element.N, 0}, {Element.N15, 0}, {Element.O, 0}, {Element.O17, 0}, {Element.O18, 0}, {Element.P, 0}, {Element.P32, 0}, {Element.S, 0}, {Element.S34, 0}, {Element.S33, 0}, {Element.F, 0}, {Element.Cl, 0}, {Element.Br, 0}, {Element.I, 0}, {Element.As, 0}};
        }



        public static List<string> split_string(string text, char separator = ',', char? _quote = null, bool with_empty = false)
        {
            _quote = _quote ?? DEFAULT_QUOTE;
            bool in_quote = false;
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            char last_char = '\0';
            bool last_escaped_backslash = false;
            
            for (int i = 0; i < text.Length; ++i)
            {
                char c = text[i];
                bool escaped_backslash = false;
                if (!in_quote)
                {
                    if (c == separator)
                    {
                        string sb_string = sb.ToString();
                        if (sb_string.Length > 0 || with_empty) tokens.Add(sb_string);
                        sb = new StringBuilder();
                    }
                    else
                    {
                        if (c == _quote) in_quote = !in_quote;
                        sb.Append(c);
                    }
                }
                else 
                {
                    if (c == '\\' && last_char == '\\' && !last_escaped_backslash)
                    {
                        escaped_backslash = true;
                    }
                    else if (c == _quote && !(last_char == '\\' && !last_escaped_backslash))
                    {
                        in_quote = !in_quote;
                    }
                    sb.Append(c);
                }
                
                last_escaped_backslash = escaped_backslash;
                last_char = c;
            }
            
            string sb_string_end = sb.ToString();
            
            if (sb_string_end.Length > 0 || (last_char == ',' && with_empty))
            {
                tokens.Add(sb_string_end);
            }
            if (in_quote){
                throw new RuntimeException("Error: corrupted token in grammar");
            }
            return tokens;
        }
    }
}
