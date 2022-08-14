using System;
using System.Collections.Generic;
using System.Text;

namespace Proviser2.Core.Servises.UserInterfaseConverters
{
    public static class PrisonDateConverter
    {
        public static string GetBeautifyPrisonDate(DateTime date)
        {
            if (date > DateTime.MinValue)
            {
                return $"Дата тримання під вартою: {date.ToShortDateString()}";
            }
            else
            {
                return "";
            }
        }
    }
}
