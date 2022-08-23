using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Proviser2.Core.Servises
{
    public static class NotificationManager
    {
        public static async Task<List<NotificationClass>> GetCourts()
        {
            List<NotificationClass> result = new List<NotificationClass>();
            var courts = await App.DataBase.GetLastCourtsAsync();
            if (courts.Count > 0)
            {
                foreach (var item in courts)
                {                  
                    NotificationClass notificationClass = new NotificationClass();
                    notificationClass.Id = item.N;
                    notificationClass.Type = "Судове засідання";
                    notificationClass.Date = item.SaveDate;
                    notificationClass.Description = $"📅{await App.DataBase.GetHeaderAsync(item.Case)} {item.Case}\n{item.Date}";
                    notificationClass.Court = item.Court;
                    notificationClass.Link = item.Case;
                    result.Add(notificationClass);
                }
            }
            return result;
        }

        public static async Task<List<NotificationClass>> GetDecisions()
        {
            List<NotificationClass> result = new List<NotificationClass>();
            var decisions = await App.DataBase.GetLastDecisionsAsync();
            if (decisions.Count > 0)
            {
                foreach (var item in decisions)
                {
                    Debug.WriteLine(item.Case);
                    NotificationClass notificationClass = new NotificationClass();
                    notificationClass.Id = item.N;
                    notificationClass.Type = "Судове рішення";
                    notificationClass.Date = item.SaveDate;
                    string legalDate = "";
                    if (item.LegalDate != DateTime.MinValue)
                    {
                        legalDate = item.LegalDate.ToShortDateString();
                    }
                    notificationClass.Description = $"📄{await App.DataBase.GetHeaderAsync(item.Case)} {item.Case}\n{item.DecisionDate.ToShortDateString()} - {legalDate}\n{item.DecisionType}";
                    notificationClass.Court = item.Court;
                    notificationClass.Link = item.URL;
                    result.Add(notificationClass);
                }
            }
            return result;
        }
    }
}
