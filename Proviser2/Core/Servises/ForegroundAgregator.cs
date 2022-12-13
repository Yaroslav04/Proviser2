using Proviser2.Core.Model;
using Proviser2.Droid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SQLite.SQLite3;

namespace Proviser2.Core.Servises
{
    public static class ForegroundAgregator
    {
        public static async Task Run()
        {
            await Download();
            await Sniffer();
        }

        static async Task Download()
        {
            Random random = new Random();
            if (DateTime.Now.Hour <= 6 & DateTime.Now.TimeOfDay > TimeSpan.FromMinutes(random.Next(0, 60)))
            {
                if (!App.IsStanDownload)
                {
                    App.IsStanDownload = true;
                    try
                    {
                        if (await App.DataBase.Log.IsDownloadNeed("stan"))
                        {
                            await ImportStanWebHook.Import();
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        App.IsStanDownload = false;
                    }
                }

                if (!App.IsCourtDownload)
                {
                    App.IsCourtDownload = true;
                    try
                    {
                        if (await App.DataBase.Log.IsDownloadNeed("court"))
                        {
                            await ImportCourtsWebHook.Import();
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        App.IsCourtDownload = false;
                    }
                }
                

                if (!App.IsDecisionDownload)
                {
                    App.IsDecisionDownload = true;
                    try
                    {
                        foreach (var item in await App.DataBase.GetCasesAsync())
                        {
                            if (await App.DataBase.Log.IsDownloadDecisionNeed(item.Case))
                            {
                                await ImportDecisions.Import(item.Case);
                                await SnifferServise.CriminalNumberSniffer(item.Case);
                            }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        App.IsDecisionDownload = false;
                    }
                }
            }
        }
        static async Task Sniffer()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday & DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
            {
                if (DateTime.Now.Hour >= 9 & DateTime.Now.Hour <= 18)
                {
                    await Task.Run(() => SnifferServise.CourtSniffer());
                    await Task.Run(() => SnifferServise.PrisonSniffer());
                    await Task.Run(() => SnifferServise.HearingSniffer());
                    await Task.Run(() => SnifferServise.NameSniffer());
                }
            }
        }
    }
}
