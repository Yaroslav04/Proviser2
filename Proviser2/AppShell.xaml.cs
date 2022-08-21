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

            RunAsync();

        }

        public async void RunAsync()
        {
            FileManager.FileInit();

            await NameSniffer.SetSniffer();
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
                await NameSniffer.RunSniffer();
                await RemainderSniffer.ReminderHeaderRefresh();
                await RemainderSniffer.ReminderPrisonNew();
                await RemainderSniffer.ReminderPrisonRefresh();
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
                "Завантажити засідання", "Завантажити судові рішення", "Запуск пошукового сервісу", 
                "Відправити на пошту", "Експорт всіх судових засідань", "Міграція"
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

                case "Міграція":
                    await Task.Run(() => CourtMigration.Migrate());
                    await DisplayAlert("Міграція", "Перенесено", "OK");
                    break;
            }
        }
    }
}