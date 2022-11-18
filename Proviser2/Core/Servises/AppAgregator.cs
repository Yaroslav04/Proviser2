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
            //Hearing sniffer
            //Name sniffer
            //Criminl case sniffer
            //Other
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
    }
}
