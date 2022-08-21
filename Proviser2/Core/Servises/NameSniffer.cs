using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public static class NameSniffer
    {
        public static async Task SetSniffer()
        {
            List<ConfigClass> sniffer = await App.DataBase.GetConfigAsync("sniffer");
            if (sniffer.Count == 0)
            {
                string result = await Shell.Current.DisplayPromptAsync("Реєстрація", "Введіть своє П.І.Б.", maxLength: 60);
                await App.DataBase.SaveConfigAsync(new ConfigClass
                {
                    Type = "sniffer",
                    Value = result
                });
                await Shell.Current.DisplayAlert("Реєстрація", "Данні для пошуку встановлені", "OK");
            }
        }

        public static async Task RunSniffer()
        {

            var snifferList = await GetSniffer();
            if (snifferList.Count == 0)
            {
                return;
            }
            else
            {
                List<string> casesFromCourtListByLittigans = new List<string>();
                foreach (ConfigClass sniffer in snifferList)
                {
                    List<CourtClass> courtsList = await App.DataBase.GetCourtsByLittigansAsync(sniffer.Value);

                    if (courtsList.Count > 0)
                    {
                        foreach (var item in courtsList)
                        {
                            casesFromCourtListByLittigans.Add(item.Case);
                        }
                        casesFromCourtListByLittigans = casesFromCourtListByLittigans.Distinct().ToList();
                    }
                }

                List<string> casesResult = new List<string>();

                if (casesFromCourtListByLittigans.Count == 0)
                {
                    return;
                }
                else
                {
                    var savedCases = await App.DataBase.GetCasesAsync();
                    if (savedCases.Count > 0)
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
                            else
                            {
                                sw = false;
                            }
                        }
                    }
                }

                if (casesResult.Count == 0)
                {
                    return;
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
            }
        }

        static async Task<List<ConfigClass>> GetSniffer()
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

    }
}
