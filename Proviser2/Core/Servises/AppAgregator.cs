using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public static class AppAgregator
    {
        public static async Task Run()
        {
            await Sniffer();
        }

        private static async Task Sniffer()
        {
            await PrisonSniffer();
            await HearingSniffer();
            await NameSniffer();
        }

        private static async Task PrisonSniffer()
        {
            var list = await App.DataBase.Notification.GetExecuteListAsync(App.NotificationType[1], DateTime.Now.DayOfYear);
            if (list.Count > 0) 
            {
                foreach(var item in list)
                {
                    await PromtService.ShowPrisonSniffer(item);
                }
            }
        }

        private static async Task HearingSniffer()
        {
            var list = await App.DataBase.Notification.GetExecuteListAsync(App.NotificationType[0], DateTime.Now.DayOfYear);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    await PromtService.ShowPrisonSniffer(item);
                }
            }
        }

        private static async Task NameSniffer()
        {
            var list = await App.DataBase.Notification.GetExecuteListAsync(App.NotificationType[2]);
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var ansver = await PromtService.IsNameSnifferSave(item);
                    if (ansver)
                    {
                        if (await SnifferServise.SaveCaseFromName(item.Case))
                        {
                            await PromtService.SimpleMessage("Реєстрація справи", "Зареєстровано");
                        }
                        else
                        {
                            await PromtService.SimpleMessage("Реєстрація справи", "Помилка");
                        }                 
                    }
                    else
                    {
                        await SnifferServise.SaveSnifferExeption(item.Case);
                        await PromtService.SimpleMessage("Реєстрація справи", "Відхилено");
                    }
                }
            }
        }
    }
}
