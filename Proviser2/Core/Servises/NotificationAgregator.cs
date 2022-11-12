using Proviser2.Core.Model;
using Proviser2.Droid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SQLite.SQLite3;

namespace Proviser2.Core.Servises
{
    public static class NotificationAgregator
    {
        public static async Task Run()
        {
            await Download();
            await Sniffer();
        }

        static async Task Download()
        {
            if (DateTime.Now.Hour >= 2 & DateTime.Now.Hour <= 5)
            {
                //stan
                if (await App.DataBase.Log.IsDownloadNeed("stan"))
                {
                    await ImportStanWebHook.Import();
                }

                //court
                if (await App.DataBase.Log.IsDownloadNeed("court"))
                {
                    await ImportCourtsWebHook.Import();
                }

                //decision
                foreach (var item in await App.DataBase.GetCasesAsync())
                {
                    if (await App.DataBase.Log.IsDownloadDecisionNeed(item.Case))
                    {
                        await ImportDecisions.Import(item.Case);
                    }
                }
            }
        }
        static async Task Sniffer()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday & DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
            {
                if (DateTime.Now.Hour >= 10 & DateTime.Now.Hour <= 18)
                {
                    await CourtSniffer();
                    await PrisonSniffer();
                    await HearingSniffer();
                }
            }
        }
        static async Task CourtSniffer()
        {
            var items = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    if ((item.Date - DateTime.Now).TotalHours <= 2)
                    {

                        try
                        {
                            NotificationClass notificationClass = new NotificationClass
                            {
                                Type = App.NotificationType[0],
                                Description = $"{await App.DataBase.GetHeaderAsync(item.Case)} {item.Date.ToShortDateString()} {item.Date.ToShortTimeString()}",
                                Case = item.Case,
                                SaveDate = DateTime.Now,
                                Day = DateTime.Now.DayOfYear,
                                IsNotificate = true,
                                IsExecute = true,
                            };

                            await App.DataBase.Notification.SaveAsync(notificationClass);
                        }
                        catch { }
                    }
                }
            }
        }
        static async Task PrisonSniffer()
        {
            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate != DateTime.MinValue)
                    {
                        if ((item.PrisonDate - DateTime.Now).Days < 15 & (item.PrisonDate - DateTime.Now).Days >= 1)
                        {
                            try
                            {
                                NotificationClass notificationClass = new NotificationClass
                                {
                                    Type = App.NotificationType[1],
                                    Description = $"Строк {item.Header} до {item.PrisonDate.ToShortDateString()}",
                                    Case = item.Case,
                                    SaveDate = DateTime.Now,
                                    Day = DateTime.Now.DayOfYear,
                                    IsNotificate = true,
                                    IsExecute = true,
                                };

                                await App.DataBase.Notification.SaveAsync(notificationClass);
                            }
                            catch { }
                        }
                    }

                    if ((item.PrisonDate != DateTime.MinValue) & (DateTime.Now > item.PrisonDate)
                        & (DateTime.Now - item.PrisonDate).TotalDays <= 15)
                    {
                        try
                        {
                            NotificationClass notificationClass = new NotificationClass
                            {
                                Type = App.NotificationType[1],
                                Description = $"Оновити строк {item.Header} {item.PrisonDate.ToShortDateString()}",
                                Case = item.Case,
                                SaveDate = DateTime.Now,
                                Day = DateTime.Now.DayOfYear,
                                IsNotificate = true,
                                IsExecute = true,
                            };

                            await App.DataBase.Notification.SaveAsync(notificationClass);
                        }
                        catch { }
                    }
                }
            }
        }
        static async Task HearingSniffer()
        {
            var cases = await App.DataBase.GetCasesAsync();
            if (cases.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var cs in cases)
                {
                    var courts = await App.DataBase.GetCourtsAsync(cs.Case);
                    if (courts.Count > 0)
                    {
                        var lastCourt = courts.OrderByDescending(x => x.Date).FirstOrDefault();
                        if ((DateTime.Now - lastCourt.Date).TotalDays > 3 & (DateTime.Now - lastCourt.Date).TotalDays < 30)
                        {
                            try
                            {
                                var header = await App.DataBase.GetHeaderAsync(cs.Case);
                                NotificationClass notificationClass = new NotificationClass
                                {
                                    Type = App.NotificationType[0],
                                    Description = $"Оновити засідання {header}, останнє {lastCourt.Date.ToShortDateString()}",
                                    Case = cs.Case,
                                    SaveDate = DateTime.Now,
                                    Day = DateTime.Now.DayOfYear,
                                    IsNotificate = true,
                                    IsExecute = true,
                                };

                                await App.DataBase.Notification.SaveAsync(notificationClass);
                            }
                            catch { }

                        }
                    }
                }
            }
        }
    }
}
