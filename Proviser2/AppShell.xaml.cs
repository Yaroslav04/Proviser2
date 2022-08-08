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

namespace Proviser2
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            DeviceDisplay.KeepScreenOn = true;

            Routing.RegisterRoute(nameof(AddCasePage), typeof(AddCasePage));

            FileManager.FileInit();
        }

        private void ServiseButton_Clicked(object sender, EventArgs e)
        {
            ServiseButton_ClickedAsync();
        }

        private async void ServiseButton_ClickedAsync()
        {
            List<string> functions = new List<string> {
                "Завантажити засідання", "Завантажити судові рішення", "Запуск пошукового сервісу", 
                "Відправити на пошту", "Експорт всіх судових засідань"
            };
            string result = await DisplayActionSheet("Меню", "Відміна", null, functions.ToArray());
            switch (result)
            {
                case "Завантажити засідання":
                    await Task.Run(() => ImportCourtsWebHook.Import());
                    break;
            }
        }
    }
}