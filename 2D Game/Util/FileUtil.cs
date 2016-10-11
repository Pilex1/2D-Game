using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Util {
    static class FileUtil {

        public static string LoadFile(string source) {
            StreamReader reader = new StreamReader(source);
            StringBuilder sb = new StringBuilder();
            string s;
            while ((s = reader.ReadLine()) != null) {
                sb.Append(s);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
