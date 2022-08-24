using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public static class ExportManager
    {
        public static async Task ExportAllCourtHearings()
        {
            foreach (var item in await App.DataBase.GetCourtsAsync())
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter(FileManager.GeneralPath("export.csv"), true))
                    {
                        string _text = $"{item.Date.ToString().Replace("\t", "")}\t{item.Court.Replace("\t", "")}\t{item.Judge.Replace("\t", "")}\t{item.Case.Replace("\t", "")}\t{item.Littigans.Replace("\t", "")}\t{item.Category.Replace("\t", "")}";
                        sr.WriteLine(_text);
                    }
                }
                catch
                {

                }
            }
        }
    }
}
