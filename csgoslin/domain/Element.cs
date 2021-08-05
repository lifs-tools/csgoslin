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
using ElementTable = System.Collections.Generic.Dictionary<Element, int>;

namespace csgoslin
{
    public class Element
    {
        public enum Element {C, C13, H, H2, N, N15, O, O17, O18, P, P32, S, S34, S33, F, Cl, Br, I, As};

        static Dictionary<string, Element> element_positions = new Dictionary<string, Element>{{"C", C}, {"H", H}, {"N", N}, {"O", O}, {"P", P}, {"P'", P32}, {"S", S}, {"F", F}, {"Cl", Cl}, {"Br", Br}, {"I", I}, {"As", As}, {"S'", S34}, {"S''", S33}, {"H'", H2}, {"C'", C13}, {"N'", N15}, {"O'", O17}, {"O''", O18}, {"2H", H2}, {"13C", C13}, {"15N", N15}, {"17O", O17}, {"18O", O18}, {"32P", P32}, {"34S", S34}, {"33S", S33}, {"H2", H2}, {"C13", C13}, {"N15", N15}, {"O17", O17}, {"O18", O18}, {"P32", P32}, {"S34", S34}, {"S33", S33}};


        static Dictionary<Element, double> element_masses = new Dictionary<Element, double>{{C, 12.0},  {H, 1.007825035},  {N, 14.0030740}, {O, 15.99491463}, {P, 30.973762},  {S, 31.9720707}, {H2, 2.014101779},  {C13, 13.0033548378},  {N15, 15.0001088984}, {O17, 16.9991315}, {O18, 17.9991604}, {P32, 31.973907274}, {S33, 32.97145876}, {S34, 33.96786690}, {F, 18.9984031}, {Cl, 34.968853}, {Br, 78.918327}, {I, 126.904473}, {As, 74.921595}};



        const Dictionary<Element, string> element_shortcut = new Dictionary<Element, string>{{C, "C"}, {H, "H"}, {N, "N"}, {O, "O"}, {P, "P"}, {S, "S"}, {F, "F"}, {Cl, "Cl"}, {Br, "Br"}, {I, "I"}, {As, "As"}, {H2, "H'"}, {C13, "C'"}, {N15, "N'"}, {O17, "O'"}, {O18, "O''"}, {P32, "P'"}, {S33, "S'"}, {S34, "S''"}};

        static List<Element> element_order = {C, H, As, Br, Cl, F, I, N, O, P, S, H2, C13, N15, O17, O18, P32, S33, S34};
        }
    }
}
