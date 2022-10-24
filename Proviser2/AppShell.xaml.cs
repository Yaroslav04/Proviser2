using Proviser2.Core.Servises;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Device;
using Proviser2.Core.View;
using Proviser2.Core.Model;
using Proviser2.Services;
using System.IO;

namespace Proviser2
{
    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            DeviceDisplay.KeepScreenOn = true;

            Routing.RegisterRoute(nameof(AddCasePage), typeof(AddCasePage));
            Routing.RegisterRoute(nameof(CasePage), typeof(CasePage));
            Routing.RegisterRoute(nameof(EventsListPage), typeof(EventsListPage));
            Routing.RegisterRoute(nameof(EventPage), typeof(EventPage));
            Routing.RegisterRoute(nameof(DecisionsListPage), typeof(DecisionsListPage));
            Routing.RegisterRoute(nameof(AddCourtPage), typeof(AddCourtPage));
            Routing.RegisterRoute(nameof(CourtsFromCaseListPage), typeof(CourtsFromCaseListPage));
            Routing.RegisterRoute(nameof(StanListPage), typeof(StanListPage));
            Routing.RegisterRoute(nameof(WitnessListPage), typeof(WitnessListPage));
            RunAsync();

        }

        public async void RunAsync()
        {
            FileManager.FileInit();

            await Sniffer.SetNameSniffer();
            await MailManager.SetMail();

            if (FileManager.FirstStart())
            {
                FileManager.WriteLog("system", "start", "");
                await RunSnifferFunctions();    
            }
        }

        public async Task RunSnifferFunctions()
        {
            IsBusy = true;
            try
            {
                await Sniffer.ReminderDownload();
                await Sniffer.RunNameSniffer();
                await Sniffer.ReminderHeaderRefresh();
                await Sniffer.ReminderPrisonNew();
                await Sniffer.ReminderPrisonRefresh();
                await Sniffer.UpdateCriminalNumberFromDecisions();
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ServiseButton_Clicked(object sender, EventArgs e)
        {
            ServiseButton_ClickedAsync();
        }

        private async void ServiseButton_ClickedAsync()
        {
            List<string> functions = new List<string> {
                "Завантажити засідання", "Завантажити судові рішення","Завантажити стан", "Запуск пошукового сервісу", "Додати дані для пошуку",
                "Відправити засідання на пошту", "Експорт всіх судових засідань", "Експорт моїх засідань", "Експорт станів",
                "Пошук учасників"
            };

            string result = await DisplayActionSheet("Меню", "Відміна", null, functions.ToArray());
            switch (result)
            {
                case "Завантажити засідання":
                    await Task.Run(() => ImportCourtsWebHook.Import());
                    await DisplayAlert("Завантаження", "Засідання завантажено", "OK");
                    break;

                case "Завантажити судові рішення":
                    await Task.Run(() => ImportDecisions.Import());
                    await DisplayAlert("Завантаження", "Рішення завантажено", "OK");
                    break;

                case "Завантажити стан":
                    await Task.Run(() => ImportStanWebHook.Import());
                    await DisplayAlert("Завантаження", "Стан завантажено", "OK");
                    break;

                case "Запуск пошукового сервісу":
                    await RunSnifferFunctions();
                    await DisplayAlert("Пошуковий сервіс", "Завершено", "OK");
                    break;

                case "Додати дані для пошуку":
                    await Sniffer.AddNameSniffer();
                    break;

                case "Відправити засідання на пошту":
                    await Task.Run(() => MailSender.SendCourtHearings());
                    await DisplayAlert("Відправка на пошту", "Засідання відправлено", "OK");
                    break;

                case "Експорт всіх судових засідань":
                    await Task.Run(() => ExportManager.ExportAllCourtHearings());
                    await DisplayAlert("Експорт", "Експорт всіх засідань виконано", "OK");
                    break;

                case "Експорт моїх засідань":
                    await Task.Run(() => ExportManager.ExportMyCourtHearings());
                    await DisplayAlert("Експорт", "Експорт моїх засідань виконано", "OK");
                    break;

                case "Експорт станів":
                    await Task.Run(() => ExportManager.ExportAllStan());
                    await DisplayAlert("Експорт", "Експорт станів виконано", "OK");
                    break;
                case "Пошук учасників":
                    await Sniffer.SearchLittigans();
                    await DisplayAlert("Пошук учасників", "Пошук учасників виконано", "OK");
                    break;
            }
        }
    }
}