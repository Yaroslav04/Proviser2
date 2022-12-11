using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public abstract class SnifferServise
    {
        public static async Task CourtSniffer()
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
        public static async Task PrisonSniffer()
        {
            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate != DateTime.MinValue)
                    {
                        if ((item.PrisonDate - DateTime.Now).Days <= 13 & (item.PrisonDate - DateTime.Now).Days >= 10)
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
        public static async Task HearingSniffer()
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
                        if ((DateTime.Now > lastCourt.Date) & (DateTime.Now - lastCourt.Date).TotalDays <= 3)
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

        #region NameSniffer
        public static async Task NameSniffer()
        {
            List<CourtClass> courtsResult = new List<CourtClass>();
            var snifferList = await GetSnifferNameList();

            if (snifferList == null)
            {
                return;
            }
            else
            {
                foreach (string sniffer in snifferList)
                {
                    var subResult = await App.DataBase.GetNotExistCourtsByLittigans(sniffer);
                    if (subResult == null)
                    {
                        
                    }
                    else
                    {
                        courtsResult.AddRange(subResult);
                    }
                }
            }

            if (courtsResult.Count == 0)
            {
                return;
            }
            else
            {
                var ex = await App.DataBase.Log.GetListSnifferExeption();
                if (ex.Count > 0)
                {
                    List<CourtClass> rem = new List<CourtClass>();
                    foreach (var c in courtsResult)
                    {
                        foreach(var e in ex)
                        {
                            if (c.Case == e.Value)
                            {
                                rem.Add(c);                               
                            }
                        }
                    }

                    if (rem.Count> 0) 
                    {
                        foreach(var r in rem)
                        {
                            courtsResult.Remove(r);
                        }
                    }
                }

                if (courtsResult.Count == 0)
                {
                    return;
                }

                foreach (var item in courtsResult)
                {
                    NotificationClass notificationClass = new NotificationClass
                    {
                        Type = App.NotificationType[2],
                        Description = $"Знайдено співпадіння: {item.Court}\n{item.Littigans}\nсудове засідання {item.Date.ToShortDateString()}\n{item.Case} {item.Category}\nЗареєструвати?",
                        Case = item.Case,
                        SaveDate = DateTime.Now,
                        Day = DateTime.Now.DayOfYear,
                        IsNotificate = false,
                        IsExecute = true,
                    };
                    try
                    {
                        await App.DataBase.Notification.SaveAsync(notificationClass);
                    }
                    catch
                    {

                    }
                }
            }
        }
        public static async Task<bool> SaveCaseFromName(string _case)
        {
            var _listt = await App.DataBase.GetCourtsAsync(_case);
            if (_listt.Count > 0)
            {
                CourtClass courts = _listt.LastOrDefault();
                CaseClass cases = new CaseClass()
                {
                    Case = courts.Case,
                    Header = TextManager.Header(courts.Littigans),
                    Note = "",
                    PrisonDate = DateTime.MinValue
                };
                try
                {
                    await App.DataBase.SaveCasesAsync(cases);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        static async Task<List<string>> GetSnifferNameList()
        {
            var configs = await App.DataBase.GetConfigAsync("sniffer");
            if (configs.Count > 0)
            {
                List<string> names = new List<string>();
                foreach (var s in configs)
                {
                    names.Add(s.Value);
                }
                return names;
            }
            else
            {
                return null;
            }
        
        }
        public static async Task<bool> IsSetNameSniffer()
        {
            List<ConfigClass> sniffer = await App.DataBase.GetConfigAsync("sniffer");
            if (sniffer.Count == 0)
            {
                return true;
            }
            return false;
        }
        public static async Task AddNameSniffer(string _value)
        {
            try
            {
                await App.DataBase.SaveConfigAsync(new ConfigClass
                {
                    Type = "sniffer",
                    Value = _value
                });
            }
            catch
            {

            }
        }
        public static async Task<bool> SaveSnifferExeption(string _case)
        {
            try
            {
                await App.DataBase.Log.SaveAsync(new LogClass
                {
                    Type = "sniffer_exeption",
                    Value = _case, 
                    Date= DateTime.Now
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        public async static Task CriminalNumberSniffer(string _case)
        {
            var cs = await App.DataBase.GetCasesByCaseAsync(_case);

            var decisions = await App.DataBase.GetDecisionsAsync(cs.Case);
            if (decisions.Count > 0)
            {
                decisions = decisions.OrderByDescending(x => x.DecisionDate).ToList();

                var criminalNumber = "";

                foreach (var d in decisions)
                {
                    if (!String.IsNullOrWhiteSpace(d.CriminalNumber))
                    {
                        criminalNumber = d.CriminalNumber;
                        break;
                    }
                }

                if (criminalNumber != "")
                {
                    cs.CriminalNumber = criminalNumber;
                    await App.DataBase.UpdateCaseAsync(cs);
                }
            }
        }
    }
}
