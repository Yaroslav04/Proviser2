using Proviser2.Core.Model;
using Proviser2.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Proviser2.Core.Servises
{
    public static class MailSender
    {

        public static async Task SendCourtHearings()
        {
            List<CourtSoketClass> courtSoketClasses = new List<CourtSoketClass>();
            var courts = await App.DataBase.GetCourtsHearingOrderingByDateAsync();
            if (courts.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var item in courts)
                {
                    try
                    {
                        var subCase = await App.DataBase.GetCasesByCaseAsync(item.Case);
                        CourtSoketClass courtSoketClass = new CourtSoketClass(item);
                        courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(subCase.PrisonDate);
                        courtSoketClass.Header = subCase.Header;
                        courtSoketClass.Note = subCase.Note;
                        courtSoketClass.PrisonDate = TextManager.GetBeautifyPrisonDate(subCase.PrisonDate);
                        courtSoketClasses.Add(courtSoketClass);
                    }
                    catch
                    {

                    }
                }
            }

            if (courtSoketClasses.Count == 0)
            {
                return;
            }
            else
            {
                string _text = "";

                foreach (var item in courtSoketClasses)
                {
                    _text = _text + $"{item.Date}\n{item.Header}\n{item.Case} {item.Judge}\n";
                    if (item.PrisonDate != "")
                    {
                        _text = _text + item.PrisonDate + "\n";
                    }
                    if (item.Note != "")
                    {
                        _text = _text + item.Note + "\n";
                    }
                    _text = _text + "\n";
                }

                await MailManager.SendEmailAsync($"Судові засідання {DateTime.Now.ToShortDateString()}", _text);
                return;
            }
        }
    }
}
