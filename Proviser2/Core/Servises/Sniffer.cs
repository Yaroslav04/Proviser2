using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public static class Sniffer
    {

        #region NameSniffer
        public static async Task SetNameSniffer()
        {
            List<ConfigClass> sniffer = await App.DataBase.GetConfigAsync("sniffer");
            if (sniffer.Count == 0)
            {
                await AddNameSniffer();
            }
        }

        public static async Task AddNameSniffer()
        {
            string result = await Shell.Current.DisplayPromptAsync("Реєстрація", "Введіть своє П.І.Б.", maxLength: 60);
            await App.DataBase.SaveConfigAsync(new ConfigClass
            {
                Type = "sniffer",
                Value = result
            });
            await Shell.Current.DisplayAlert("Реєстрація", "Данні для пошуку встановлені", "OK");
        }


        public static async Task<List<CourtClass>> GetNotExistCourtsByLittigans(string _name)
        {
            List<string> casesFromCourtListByLittigans = new List<string>();

            List<CourtClass> courtsList = await App.DataBase.GetCourtsByLittigansAsync(_name);

            if (courtsList.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var item in courtsList)
                {
                    casesFromCourtListByLittigans.Add(item.Case);
                }

                casesFromCourtListByLittigans = casesFromCourtListByLittigans.Distinct().ToList();
            }

            List<string> casesResult = new List<string>();

            if (casesFromCourtListByLittigans.Count == 0)
            {
                return null;
            }
            else
            {
                var savedCases = await App.DataBase.GetCasesAsync();

                if (savedCases.Count == 0)
                {
                    foreach (var item in casesFromCourtListByLittigans)
                    {
                        casesResult.Add(item);
                    }
                }
                else
                {
                    foreach (var item in casesFromCourtListByLittigans)
                    {

                        bool sw = false;

                        foreach (var subitem in savedCases)
                        {
                            if (item == subitem.Case)
                            {
                                sw = true;
                            }
                        }

                        if (sw == false)
                        {
                            casesResult.Add(item);
                        }
                    }
                }
            }

            if (casesResult.Count == 0)
            {
                return null;
            }
            else
            {
                List<CourtClass> courtsResult = new List<CourtClass>();
               
                foreach (var c in casesResult)
                {
                    CourtClass r = await App.DataBase.GetLastLocalCourtAsync(c);
                    if (r != null)
                    {
                        courtsResult.Add(r);
                    }
                }

                if (courtsResult.Count == 0)
                {
                    return null;
                }
                courtsResult = courtsResult.OrderByDescending(x => x.Date).ToList();
                return courtsResult;
            }
        }

        public static async Task RunNameSniffer()
        {
            List<CourtClass> courtsResult = new List<CourtClass>();
            var snifferList = await GetNameSniffer();
            if (snifferList.Count == 0)
            {
                return;
            }
            else
            {
                foreach (ConfigClass sniffer in snifferList)
                {
                    var subResult = await GetNotExistCourtsByLittigans(sniffer.Value);
                    if (subResult.Count > 0)
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
                courtsResult = courtsResult.Where(x => (DateTime.Now - x.Date).TotalDays < 30).ToList();
            }

            if (courtsResult.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var m in courtsResult)
                {
                    bool answer = await Shell.Current.DisplayAlert("Пошук по користувачу", $"Знайдено співпадінь:\n{m.Case} {m.Littigans}\nЗареєструвати?", "Так", "Ні");

                    if (answer)
                    {
                        var _listt = await App.DataBase.GetCourtsAsync(m.Case);
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
                                FileManager.WriteLog("add case", cases.Case, "");
                                await Shell.Current.DisplayAlert("Успішно", "Зареєстровано", "OK");
                            }
                            catch
                            {
                                await Shell.Current.DisplayAlert("Помилка", "Вже зареєстровано або помилка", "OK");
                            }
                        }
                    }
                }
            }
        }

        static async Task<List<ConfigClass>> GetNameSniffer()
        {
            var result = await App.DataBase.GetConfigAsync("sniffer");
            if (result.Count > 0)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Reminder

        public static async Task ReminderPrisonNew()
        {
            List<string> messages = new List<string>();

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate > DateTime.Now)
                    {
                        if ((item.PrisonDate - DateTime.Now).Days < 15)
                        {
                            if ((item.PrisonDate - DateTime.Now).Days > 10)
                            {
                                messages.Add($"Необхідно продовжити тримання під вартою:\n{item.Header} {item.Case}\n{item.PrisonDate.ToShortDateString()}");
                            }
                        }
                    }
                }
                if (messages.Count > 0)
                {
                    foreach (var mes in messages)
                    {
                        await Shell.Current.DisplayAlert("Нагадування", mes, "OK");
                    }
                }
            }
        }

        public static async Task ReminderPrisonRefresh()
        {
            List<string> messages = new List<string>();

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    if (item.PrisonDate < DateTime.Now)
                    {
                        if ((DateTime.Now - item.PrisonDate).Days < 30)
                        {
                            messages.Add($"Необхідно оновити під вартою:\n{item.Header} {item.Case}\n{item.PrisonDate.ToShortDateString()}");
                        }
                    }
                }
                if (messages.Count > 0)
                {
                    foreach (var mes in messages)
                    {
                        await Shell.Current.DisplayAlert("Нагадування", mes, "OK");
                    }
                }
            }
        }

        public static async Task ReminderHeaderRefresh()
        {

            List<string> messages = new List<string>();

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
                        if ((DateTime.Now - lastCourt.Date).TotalDays > 3)
                        {
                            if ((DateTime.Now - lastCourt.Date).TotalDays < 30)
                            {
                                messages.Add($"Необхідно оновити засідання:\n{cs.Header} {cs.Case}\n{lastCourt.Date}");
                            }
                        }
                    }
                }
            }

            if (messages.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var mes in messages)
                {
                    await Shell.Current.DisplayAlert("Нагадування", mes, "OK");
                }
            }
        }


        #endregion

        #region CriminalNumberSniffer
        public async static Task UpdateCriminalNumberFromDecisions()
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
                    if (String.IsNullOrWhiteSpace(cs.CriminalNumber))
                    {
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
        }

        #endregion
    }
}
