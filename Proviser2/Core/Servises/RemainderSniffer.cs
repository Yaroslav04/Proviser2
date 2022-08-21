using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    public static class RemainderSniffer
    {
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

            var _cases = await App.DataBase.GetCasesAsync();
            if (_cases.Count > 0)
            {
                foreach (var item in _cases)
                {
                    var _courts = await App.DataBase.GetCourtsAsync(item.Case);
                    if (_courts.Count > 0)
                    {
                        if ((DateTime.Now - _courts.LastOrDefault().Date).TotalDays > 7)
                        {
                            if ((DateTime.Now - _courts.LastOrDefault().Date).TotalDays < 30)
                            {
                                messages.Add($"Необхідно оновити засідання:\n{item.Header} {item.Case}\n{_courts.LastOrDefault().Date}");
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
    }
}
