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
                    dataBase = new DataBase(FileManager.GeneralPath(), new List<string> {
                        "CourtsDataBase.db3", "CasesDataBase.db3", "DecisionDataBase.db3",
                        "EventDataBase.db3", "ConfigDataBase.db3", "StanDataBase.db3", "WitnessDataBase.db3",
                        "LogDataBase.db3"
                    });
                }
                return dataBase;
            }
        }

        public static List<string> WitnessTypes
        {
            get
            {
                return new List<string>
                {
                "обвинувачений",
                "потерпілий",
                "свідок",
                "експерт/спеціаліст",
                };
            }
        }

        public static List<string> EventsTypes
        {
            get
            {
                return new List<string>
                {
                "перенесено засідання",
                "допит в суді",
                "дослідження матеріалів",
                "проведено підготовче",            
                "долучено матеріали провадження",
                "інше",
                };
            }
        }

        public static List<string> LogTypes
        {
            get
            {
                return new List<string>
                {
                "system",
                "download",
                "sniffer",
                "event",
                };
            }
        }

        public static List<string> LogDownloadTegs
        {
            get
            {
                return new List<string>
                {
                "court",
                "stan",
                "decision",
                };
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
