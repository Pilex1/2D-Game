using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game.Util {
    class StringUtil {
        public static bool IsAlphaNumericSpace(char c) {
            return char.IsLetterOrDigit(c) || c == ' ';
        }
        public static bool StartsWithNumber(string s) {
            return Regex.IsMatch(s, @"^\d");
        }
    }
}
