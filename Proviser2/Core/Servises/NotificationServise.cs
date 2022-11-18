using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    public static class NotificationServise
    {
        public static string GetTitleFromNotificationType(string _value)
        {
            if (_value == App.NotificationType[0])
            {
                return "Судове засідання";
            }

            if (_value == App.NotificationType[1])
            {
                return "Тримання під вартою";
            }

            if (_value == App.NotificationType[2])
            {
                return "Пошук по користувачу";
            }

            return "";
        }

        public static async Task NotificationShowUpdate(NotificationClass notificationClass)
        {
            try
            {
                notificationClass.IsExecute = false;
                notificationClass.IsNotificate = false;
                await App.DataBase.Notification.UpdateAsync(notificationClass);
            }
            catch
            {

            }
        }
    }
}
