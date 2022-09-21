using Proviser2.Core.Servises;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser2
{
    public partial class App : Application
    {
        static DataBase dataBase;
        public static DataBase DataBase
        {
            get
            {
                if (dataBase == null)
                {
                    dataBase = new DataBase(FileManager.GeneralPath(), new List<string> { "CourtsDataBase.db3", "CasesDataBase.db3", "DecisionDataBase.db3", "EventDataBase.db3", "ConfigDataBase.db3", "StanDataBase.db3" });
                }
                return dataBase;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
