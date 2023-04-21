using System;
using System.Collections.Generic;
using System.Text;

namespace NerdleWebApi
{
    public static class Constants
    {
        public const string AvailableOperators = "+-*/=";
        public const string AvailableChars = "0123456789" + AvailableOperators;
        public const int ExpressionLength = 8;
        public const int NumberOfGuesses = 8;
        public const char NumberStart = '<';
        public const char NumberEnd = '>';
        public const string FirstGuess = "9+8-7=10"; 
    }
}