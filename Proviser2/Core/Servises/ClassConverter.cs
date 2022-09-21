using Proviser2.Core.Model;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Proviser2.Core.Servises
{
    static class ClassConverter
    {

        public static async Task<CourtSoketClass> ConvertCourtClassToSoket(CourtClass _courtClass)
        {
            CourtSoketClass courtSoketClass = new CourtSoketClass(_courtClass);
            var cs = await App.DataBase.GetCasesByCaseAsync(_courtClass.Case);
            if (cs != null) 
            {
                courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(cs.PrisonDate);
                courtSoketClass.Header = cs.Header;
                courtSoketClass.Note = cs.Note;
            }
            courtSoketClass.DateSoket = TextManager.GetBeautifyCourtDate(_courtClass.Date);

            return courtSoketClass;
        }

        public static async Task<DecisionSoketClass> ConvertDecisionClassToSoket(DecisionClass _decisionClass)
        {
            DecisionSoketClass decisionSoketClass = new DecisionSoketClass(_decisionClass);
            decisionSoketClass.Header = await App.DataBase.GetHeaderAsync(_decisionClass.Case);
            decisionSoketClass.DecisionDateSoket = _decisionClass.DecisionDate.ToShortDateString();
            decisionSoketClass.LegalDateSoket = TextManager.GetBeautifyLegalDate(_decisionClass.LegalDate);
            decisionSoketClass.CategorySoket = TextManager.GetBeautifyDecisionCategory(_decisionClass.Category);

            return decisionSoketClass;
        }

        public static async Task<StanSoketClass> ConvertStanClassToSoket(StanClass _stanClass)
        {
            StanSoketClass stanSoketClass = new StanSoketClass(_stanClass);
            stanSoketClass.Header = await App.DataBase.GetHeaderAsync(_stanClass.Case);
            return stanSoketClass;
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

        public static StanClass TransformTextToStanClass(string _text)
        {
            StanClass _stan = new StanClass();
            _stan.Court = Exeption(_text, 0);
            _stan.Case = Exeption(_text, 1);
            _stan.SubCase = Exeption(_text, 2);
            _stan.RegDate = ExeptionDate(_text, 3);
            _stan.Judge = Exeption(_text, 4);
            _stan.SubJudge = Exeption(_text, 5);
            _stan.Littigans = Exeption(_text, 6);
            _stan.Date = ExeptionDate(_text, 7);
            _stan.Decision = Exeption(_text, 8);
            _stan.SubDecision = Exeption(_text, 9);
            _stan.Category = Exeption(_text, 10);
            _stan.Description = Exeption(_text, 11);
            return _stan;
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
