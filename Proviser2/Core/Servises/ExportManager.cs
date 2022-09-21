using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    using (StreamWriter sr = new StreamWriter(FileManager.GeneralPath("court.csv"), true))
                    {
                        sr.WriteLine(item.ToString());
                    }
                }
                catch
                {

                }
            }
        }

        public static async Task ExportMyCourtHearings()
        {
            List<CourtClass> list = new List<CourtClass>();
            foreach(var c in await App.DataBase.GetCasesAsync())
            {
                var r = await App.DataBase.GetCourtsAsync(c.Case);
                if (r.Count > 0)
                {
                    list.AddRange(r);
                }
            }

            if (list.Count == 0)
            {
                return;
            }
            else
            {
                list = list.OrderBy(x => x.Date).ToList();
            }

            foreach (var item in list)
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter(FileManager.GeneralPath("mycourt.csv"), true))
                    {
                        sr.WriteLine(item.ToString());
                    }
                }
                catch
                {

                }
            }
        }

        public static async Task ExportAllStan()
        {
            foreach (var item in await App.DataBase.GetStansAsync())
            {
                try
                {
                    using (StreamWriter sr = new StreamWriter(FileManager.GeneralPath("stan.csv"), true))
                    {
                        sr.WriteLine(item.ToString());
                    }
                }
                catch
                {

                }
            }
        }
    }
}
