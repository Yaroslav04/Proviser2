﻿using Proviser2.Core.Model;
using System;
using System.Threading.Tasks;

namespace Proviser2.Core.Servises
{
    static class ClassConverter
    {

        public static async Task<CourtSoketClass> ConvertCourtClassToSoket(CourtClass courtClass)
        {
            CourtSoketClass courtSoketClass = new CourtSoketClass(courtClass);
            var cs = await App.DataBase.GetCasesByCaseAsync(courtClass.Case);
            if (cs != null) 
            {
                courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(cs.PrisonDate);
                courtSoketClass.Header = cs.Header;
                courtSoketClass.Note = cs.Note;
            }
            courtSoketClass.DateSoket = TextManager.GetBeautifyCourtDate(courtClass.Date);

            return courtSoketClass;
        }

        public static CourtClass TransformTextToCourtClass(string _text)
        {
            CourtClass courts = new CourtClass();
            courts.Date = ExeptionDate(_text, 0);
            courts.Judge = Exeption(_text, 1);
            courts.Case = Exeption(_text, 2);
            courts.Court = Exeption(_text, 3);
            //4
            courts.Littigans = Exeption(_text, 5);
            courts.Category = Exeption(_text, 6);
            return courts;
        }

        private static string Exeption(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return _array[_index].Replace("\"", "");
            }
            catch
            {
                return "";
            }
        }

        private static DateTime ExeptionDate(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return Convert.ToDateTime(_array[_index].Replace("\"", ""));
            }
            catch
            {
                return Convert.ToDateTime("01.01.2000 00:00:00");
            }
        }
    }
}
