using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Util {
    class StringUtil {

        /// <summary>
        /// Returns true if the entire array is comprised of empty lines
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static bool AreEmpty(string[] arr) {
            foreach (var s in arr) {
                if (s != "") return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the string contains symbols found on a standard keyboard except for the underscore
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainsStdSymbolU(string s) {
            string select = "!@#$%^&*()+-=[]{}\\|;:\'\"<>,.?/~`";
            foreach (char c in s) {
                if (!select.Contains(c)) return false;
            }
            return true;
        }
        public static bool ContainsStdSymbolU(char c) { return ContainsStdSymbol(c.ToString()); }

        /// <summary>
        /// Returns true if the string contains symbols found on a standard keyboard
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainsStdSymbol(string s) {
            string select = "!@#$%^&*()_+-=[]{}\\|;:\'\"<>,.?/~`";
            foreach (char c in s) {
                if (!select.Contains(c)) return false;
            }
            return true;
        }
        public static bool ContainsStdSymbol(char c) { return ContainsStdSymbol(c.ToString()); }

        /// <summary>
        /// Returns true if the string contains any Unicode defined symbols
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ContainsSymbol(string s) {
            foreach (char c in s) {
                if (!char.IsSymbol(c)) return false;
            }
            return true;
        }
        public static bool ContainsSymbol(char c) { return ContainsSymbol(c.ToString()); }

        public static bool ContainsLetter(string s) {
            foreach (char c in s) {
                if (!char.IsLetter(c)) return false;
            }
            return true;
        }
        public static bool IsLetter(char c) { return ContainsLetter(c.ToString()); }

        public static bool ContainsDigit(string s) {
            foreach (char c in s) {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
        public static bool IsDigit(char c) { return ContainsDigit(c.ToString()); }




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
