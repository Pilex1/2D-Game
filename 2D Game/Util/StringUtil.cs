using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Game.Util {
    class StringUtil {
        public static bool StartsWithLetter(string s) {
            if (s == "") return false;
            char c = s[0];
            if (char.IsLetter(c)) return true;
            return false;
        }
        public static bool IsAlphaNumericSpace(char c) {
            return char.IsLetterOrDigit(c) || c == ' ';
        }
        public static bool IsNumeric(string s) {
            foreach (char c in s) {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
        public static bool StartsWithNumber(string s) {
            return Regex.IsMatch(s, @"^\d");
        }
        public static string RandNum(int length, Random rand) {
            string firstselect = "123456789";
            string select = "0123456789";
            StringBuilder sb = new StringBuilder();
            sb.Append(firstselect[MathUtil.RandInt(rand, 0, firstselect.Length - 1)]);
            for (int i = 1; i < length; i++) {
                sb.Append(select[MathUtil.RandInt(rand, 0, select.Length - 1)]);
            }
            return sb.ToString();
        }
    }
}
