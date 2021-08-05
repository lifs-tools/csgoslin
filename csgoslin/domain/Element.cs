﻿/*
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
//using ElementTable = System.Collections.Generic.Dictionary<Element, int>;

namespace csgoslin
{
    public enum Element {C, C13, H, H2, N, N15, O, O17, O18, P, P32, S, S34, S33, F, Cl, Br, I, As};
    
    public class Elements
    {
        public static Dictionary<string, Element> element_positions = new Dictionary<string, Element>{{"C", Element.C}, {"H", Element.H}, {"N", Element.N}, {"O", Element.O}, {"P", Element.P}, {"P'", Element.P32}, {"S", Element.S}, {"F", Element.F}, {"Cl", Element.Cl}, {"Br", Element.Br}, {"I", Element.I}, {"As", Element.As}, {"S'", Element.S34}, {"S''", Element.S33}, {"H'", Element.H2}, {"C'", Element.C13}, {"N'", Element.N15}, {"O'", Element.O17}, {"O''", Element.O18}, {"2H", Element.H2}, {"13C", Element.C13}, {"15N", Element.N15}, {"17O", Element.O17}, {"18O", Element.O18}, {"32P", Element.P32}, {"34S", Element.S34}, {"33S", Element.S33}, {"H2", Element.H2}, {"C13", Element.C13}, {"N15", Element.N15}, {"O17", Element.O17}, {"O18", Element.O18}, {"P32", Element.P32}, {"S34", Element.S34}, {"S33", Element.S33}};
        

        public static  Dictionary<Element, double> element_masses = new Dictionary<Element, double>{{Element.C, 12.0},  {Element.H, 1.007825035},  {Element.N, 14.0030740}, {Element.O, 15.99491463}, {Element.P, 30.973762},  {Element.S, 31.9720707}, {Element.H2, 2.014101779},  {Element.C13, 13.0033548378},  {Element.N15, 15.0001088984}, {Element.O17, 16.9991315}, {Element.O18, 17.9991604}, {Element.P32, 31.973907274}, {Element.S33, 32.97145876}, {Element.S34, 33.96786690}, {Element.F, 18.9984031}, {Element.Cl, 34.968853}, {Element.Br, 78.918327}, {Element.I, 126.904473}, {Element.As, 74.921595}};


        
        public static  Dictionary<Element, string> element_shortcut = new Dictionary<Element, string>{{Element.C, "C"}, {Element.H, "H"}, {Element.N, "N"}, {Element.O, "O"}, {Element.P, "P"}, {Element.S, "S"}, {Element.F, "F"}, {Element.Cl, "Cl"}, {Element.Br, "Br"}, {Element.I, "I"}, {Element.As, "As"}, {Element.H2, "H'"}, {Element.C13, "C'"}, {Element.N15, "N'"}, {Element.O17, "O'"}, {Element.O18, "O''"}, {Element.P32, "P'"}, {Element.S33, "S'"}, {Element.S34, "S''"}};

        public static List<Element> element_order = new List<Element>{Element.C, Element.H, Element.As, Element.Br, Element.Cl, Element.F, Element.I, Element.N, Element.O, Element.P, Element.S, Element.H2, Element.C13, Element.N15, Element.O17, Element.O18, Element.P32, Element.S33, Element.S34};
    }
}
