using Proviser2.Core.Model;
using Proviser2;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Proviser2.Services
{
    public static class MailManager
    {

        public static async Task SetMail()
        {
            List<ConfigClass> sniffer = await App.DataBase.GetConfigAsync("mail");
            if (sniffer.Count == 0)
            {
                string result = await Shell.Current.DisplayPromptAsync("Реєстрація", "Введіть свою електронну пошту", maxLength: 60);
                await App.DataBase.SaveConfigAsync(new ConfigClass
                {
                    Type = "mail",
                    Value = result
                });
                await Shell.Current.DisplayAlert("Реєстрація", "Пошта встановлена", "OK");
            }
        }

        public static async Task SendEmailAsync(string _header, string _text)
        {
            var x = await App.DataBase.GetConfigAsync("mail");
            if (x.Count == 0)
            {
                return;
            }
            else
            {
                var mail = x.LastOrDefault().Value;
                MailAddress from = new MailAddress("ischenkoyaroslav@gmail.com", "Proviser");
                MailAddress to = new MailAddress(mail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = _header;
                m.Body = _text;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("ischenkoyaroslav@gmail.com", "icksxcqinjfcbvpm");
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(m);
                return;
            }
        }
    }
}
