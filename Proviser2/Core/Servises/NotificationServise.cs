using System;
using System.Collections.Generic;
using System.Text;

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

            return "";
        }
    }
}
