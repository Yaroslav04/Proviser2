using Proviser2.Core.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Proviser2.Core.Servises
{
    public static class TextManager
    {

        public static string Header(string _text)
        {
            Regex regex = new Regex(@"(обвинувачений:) \w+");
            MatchCollection matches = regex.Matches(_text);
            if (matches.Count > 0)
            {
                return matches[0].Value.Replace("обвинувачений:", "").Replace(" ", "");
            }

            Regex regex1 = new Regex(@"(Обвинувачений:) \w+");
            MatchCollection matches1 = regex1.Matches(_text);
            if (matches1.Count > 0)
            {
                return matches1[0].Value.Replace("Обвинувачений:", "").Replace(" ", "");
            }

            Regex regex2 = new Regex(@"(підозрюваний:) \w+");
            MatchCollection matches2 = regex2.Matches(_text);
            if (matches2.Count > 0)
            {
                return matches2[0].Value.Replace("підозрюваний:", "").Replace(" ", "");
            }

            Regex regex3 = new Regex(@"(Підозрюваний:) \w+");
            MatchCollection matches3 = regex3.Matches(_text);
            if (matches3.Count > 0)
            {
                return matches3[0].Value.Replace("Підозрюваний:", "").Replace(" ", "");
            }

            return "";
        }
        public static bool DateTime(string _text)
        {
            Regex regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d( )\d\d(:)\d\d");
            return regex.IsMatch(_text);        
        }

        public static bool Date(string _text)
        {
            Regex regex = new Regex(@"\d\d(.)\d\d(.)\d\d\d\d");
            return regex.IsMatch(_text);
        }

        public static string GetBeautifyPrisonDate(DateTime _date)
        {
            if (_date > System.DateTime.MinValue)
            {
                if ((System.DateTime.Now - _date).TotalDays < 30)
                {
                    return $"Дата тримання під вартою: {_date.ToShortDateString()}";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        public static string GetBeautifyLegalDate(DateTime _date)
        {
            if (_date > System.DateTime.MinValue)
            {
                return $"Дата набрання законної сили:{_date.ToShortDateString()}";
            }
            else
            {
                return "";
            }
        }

        public static string GetBeautifyDecisionCategory(string _text)
        {
            if (string.IsNullOrWhiteSpace(_text))
            {
                return "";
            }
            else
            {
                List<string> categoties = new List<string>(_text.Split(";"));
                if (categoties.Count == 0)
                {
                    return _text;
                }
                else
                {
                    return categoties.LastOrDefault().Trim();
                }
            }
        }
    }
}
