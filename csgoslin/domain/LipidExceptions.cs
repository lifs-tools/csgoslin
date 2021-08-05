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

namespace csgoslin
{
    public class LipidException : Exception
    {
        public LipidException()
        {
        }
        
        public LipidException(string message) : base(message)
        {
        }
    }
    
    
    public class IllegalArgumentException : LipidException
    {
        public IllegalArgumentException()
        {
        }
        
        public IllegalArgumentException(string message) : base("IllegalArgumentException: " + message)
        {
        }
    }


    public class ConstraintViolationException : LipidException
    {
        public ConstraintViolationException()
        {
        }
        
        public ConstraintViolationException(string message) : base("ConstraintViolationException: " + message)
        {
        }
    }


    public class RuntimeException : LipidException
    {
        public RuntimeException()
        {
        }
        
        public RuntimeException(string message) : base("RuntimeException: " + message)
        {
        }
    }


    public class UnsupportedLipidException : LipidException
    {
        public UnsupportedLipidException()
        {
        }
        
        public UnsupportedLipidException(string message) : base("UnsupportedLipidException: " + message)
        {
        }
    }


    public class LipidParsingException : LipidException
    {
        public LipidParsingException(){
        }
        
        public LipidParsingException(string message) : base("LipidParsingException: " + message)
        {
        }
    }
}
