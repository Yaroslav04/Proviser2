using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Shapes;
using Path = System.IO.Path;

namespace Proviser2.Core.Servises
{
    public static class FileManager
    {
        public static string GeneralPath()
        {
            return @"/storage/emulated/0/Proviser2/";
        }

        public static string GeneralPath(string _file)
        {
            return Path.Combine(@"/storage/emulated/0/Proviser2/", _file);
        }
        public static bool FileInit()
        {
            bool refresh = false;

            if (!Directory.Exists(GeneralPath()))
            {
                Directory.CreateDirectory(GeneralPath());
                refresh = true;
            }

            if (!File.Exists(GeneralPath("log.txt")))
            {
                File.Create(GeneralPath("log.txt"));
                refresh = true;
            }

            if (!Directory.Exists(GeneralPath("Capture")))
            {
                Directory.CreateDirectory(GeneralPath("Capture"));
                refresh = true;
            }

            if (refresh == true)
            {
                return true;
            }

            return false;
        }

        public static void WriteLog(string _teg, string _case, string _value)
        {
            using (StreamWriter sw = new StreamWriter(GeneralPath("log.txt"), append: true))
            {
                sw.WriteLine($"{DateTime.Now}\t{_teg}\t{_case}\t{_value}");
            }
        }

        public static List<string> GetSavedStan()
        {
            List<string> savedStan = new List<string>();
            using (StreamReader sr = new StreamReader(GeneralPath("log.txt")))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var array = line.Split("\t");
                    if (array[2] == "stan")
                    {
                        savedStan.Add(array[3].Replace("\n", "").Replace("\r", ""));
                    }
                }
            }
            return savedStan;
        }

        public static bool FirstStart()
        {
            if (File.GetLastWriteTime(GeneralPath("log.txt")).DayOfYear != DateTime.Now.DayOfYear)
            {
                return true;
            }

            return false;
        }
    }
}
