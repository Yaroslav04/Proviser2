using Proviser2.Core.Model;
using Proviser2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        courtSoketClass.CriminalNumber = subCase.CriminalNumber;
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
                    _text = _text + $"{item.Date} судове засідання\n" +
                        $"обвинувачений: {item.Header}\n" +
                        $"суд: {item.Court}, номер судової справи: {item.Case}\n" +
                        $"суддя: {item.Judge}\n" +
                        $"номер кримінального провадження: {item.CriminalNumber}\n";
                    if (item.PrisonDate != "")
                    {
                        _text = _text + $"дата тримання під вартою: {item.PrisonDate}\n";
                    }
               
                    var witness = await App.DataBase.GetWitnessByCaseAsync(item.Case);
                    witness = witness.Where(x => x.Status == true).ToList();
                    if (witness.Count > 0)
                    {
                        _text = _text + "Необхідно забезпечити явку до судового засідання:\n";
                        int k = 1;
                        foreach (var w in witness)
                        {
                            _text = _text + $"{k}.{w.Type}: {w.Name} {w.BirthDate}\n" +
                                $"місце мешкання: {w.Location}\n" +
                                $"місце роботи: {w.Work}\n" +
                                $"засоби зв'язку: {w.Contact}\n";
                            k++;
                        }
                    }

                    //if (item.Note != "")
                    //{
                    //    _text = _text + $"Примітка: {item.Note}\n";
                    //}

                    _text = _text + "\n";
                }

                await MailManager.SendEmailAsync($"Судові засідання {DateTime.Now.ToShortDateString()}", _text);
                return;
            }
        }
    }
}
