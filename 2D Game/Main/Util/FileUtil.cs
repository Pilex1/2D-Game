using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public static bool IsDirectoryEmpty(string path) {
            IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
            using (IEnumerator<string> en = items.GetEnumerator()) {
                return !en.MoveNext();
            }
        }

        public static void EmptyDirectory(string path) {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles()) {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories()) {
                dir.Delete(true);
            }
        }

    }
}
