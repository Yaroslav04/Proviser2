using Proviser2.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Proviser2.Core.Servises
{
    public static class CourtMigration
    {
        public static async Task Migrate()
        {
            var oldCourts = await App.DataBase.GetOldCourtsAsync();
            Debug.WriteLine(oldCourts.Count);
            foreach (var item in oldCourts)
            {
                try
                {
                    await App.DataBase.SaveCourtAsync(ConvertToNewClass(item));
                    Debug.Write($"{item.Date} {item.Case}");
                }
                catch(Exception ex)
                {
                    Debug.Write(ex.Message);
                }
            }
        }
        private static CourtClass ConvertToNewClass(Courts oldCourt)
        {
            CourtClass courtClass = new CourtClass();
            courtClass.Date = oldCourt.Date;
            courtClass.Judge = oldCourt.Judge;
            courtClass.Case = oldCourt.Case;
            courtClass.Court = oldCourt.Court;
            courtClass.Littigans = oldCourt.Littigans;
            courtClass.Category = oldCourt.Category;
            courtClass.Origin = oldCourt.Status;
            courtClass.SaveDate = Convert.ToDateTime("01.08.2022 12:00");
            return courtClass;
        }
    }

    public class Courts
    {
        [AutoIncrement]
        [PrimaryKey]
        [NotNull]
        public int Id { get; set; }
        [Indexed(Name = "ListingID", Order = 1, Unique = true)]
        public DateTime Date { get; set; }
        public string Judge { get; set; }

        [Indexed(Name = "ListingID", Order = 2, Unique = true)]
        public string Case { get; set; }
        public string Court { get; set; }
        public string Littigans { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string SoketHeader { get; set; }
        public string SoketNote { get; set; }
        public string SoketPrisonDate { get; set; }
    }
}
