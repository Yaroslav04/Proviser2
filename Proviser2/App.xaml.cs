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
                        "LogDataBase.db3", "NotificationDataBase.db3"
                    });
                }
                return dataBase;
            }
        }

        #region Withes
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

        #endregion

        #region Events
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

        #endregion

        #region Log
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

        #endregion

        #region Notification

        public static List<string> NotificationType
        {
            get
            {
                return new List<string>
                {
                "courtChannel",
                "prisonChanel",
                "nameChanel",
                "systemChanel",
                };
            }
        }

        #endregion

        #region Agregator

        public static bool IsStanDownload = false;
        public static bool IsCourtDownload = false;
        public static bool IsDecisionDownload = false;

        #endregion

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
