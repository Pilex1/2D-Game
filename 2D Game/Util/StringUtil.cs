using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Util {
    class StringUtil {

        public static bool IsStdKeySymbol(string s) {
            string select = "!@#$%^&*()_+-=[]{}\\|;:\'\"<>,.?/~`";
            foreach (char c in s) {
                if (!select.Contains(c)) return false;
            }
            return true;
        }
        public static bool IsStdKeySymbol(char c) { return IsStdKeySymbol(c.ToString()); }

        public static bool IsSymbol(string s) {
            foreach (char c in s) {
                if (!char.IsSymbol(c)) return false;
            }
            return true;
        }
        public static bool IsSymbol(char c) { return IsSymbol(c.ToString()); }

        public static bool IsLetter(string s) {
            foreach (char c in s) {
                if (!char.IsLetter(c)) return false;
            }
            return true;
        }
        public static bool IsLetter(char c) { return IsLetter(c.ToString()); }

        public static bool IsDigit(string s) {
            foreach (char c in s) {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
        public static bool IsDigit(char c) { return IsDigit(c.ToString()); }




        public static bool StartsWithLetter(string s) {
            if (s == "") return false;
            char c = s[0];
            if (char.IsLetter(c)) return true;
            return false;
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

        public static string TruncateTo(float x, int decimals) {
            StringBuilder sb = new StringBuilder("{0:0.");
            for (int i = 0; i < decimals; i++)
                sb.Append("0");
            sb.Append("}");
            return string.Format(sb.ToString(), x);
        }
    }
}
