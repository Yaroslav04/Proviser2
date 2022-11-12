using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Proviser2.Core.Servises;
using Proviser2.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Android.Content.ClipData;
using AndroidApp = Android.App.Application;
using Debug = System.Diagnostics.Debug;

[assembly: Dependency(typeof(ForegroundServices))]
namespace Proviser2.Droid
{
    [Service]
    public class ForegroundServices : Service, IForegroundService
    {
        public static bool IsForegroundServiceRunning;
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(async () =>
            {
                while (IsForegroundServiceRunning)
                {
                    if (DateTime.Now.Hour >= 1 & DateTime.Now.Hour <= 5)
                    {
                        //stan
                        if (await App.DataBase.Log.IsDownloadNeed("stan"))
                        {
                            Debug.WriteLine("Start download stan");
                            await ImportStanWebHook.Import();
                        }
                        else
                        {
                            Debug.WriteLine("No need download stan");
                        }

                        //court
                        if (await App.DataBase.Log.IsDownloadNeed("court"))
                        {
                            Debug.WriteLine("Start download court");
                            await ImportCourtsWebHook.Import();
                        }
                        else
                        {
                            Debug.WriteLine("No need download court");
                        }

                        //decision
                        foreach(var item in await App.DataBase.GetCasesAsync())
                        {
                            if (await App.DataBase.Log.IsDownloadDecisionNeed(item.Case))
                            {
                                Debug.WriteLine($"Start download decision {item.Case}");
                                await ImportDecisions.Import(item.Case);
                            }
                            else
                            {
                                Debug.WriteLine($"No need download decision {item.Case}");
                            }
                        }                     
                    }

                    Thread.Sleep(60000);
                }          
            });

            string channelID = "Proviser2ForegroundChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
            notificationManager.CreateNotificationChannel(notfificationChannel);


            var notificationBuilder = new NotificationCompat.Builder(this, channelID)
                                         .SetContentTitle("Proviser2 Foreground Servise")
                                         .SetSmallIcon(Resource.Mipmap.icon_round)
                                         .SetContentText("Proviser2 Running")
                                         .SetPriority(1)
                                         .SetOngoing(true)
                                         .SetChannelId(channelID)
                                         .SetAutoCancel(true);

            StartForeground(1001, notificationBuilder.Build());
            return base.OnStartCommand(intent, flags, startId);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            IsForegroundServiceRunning = true;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            IsForegroundServiceRunning = false;
        }

        public void StartMyForegroundService()
        {
            var intent = new Intent(AndroidApp.Context, typeof(ForegroundServices));
            AndroidApp.Context.StartForegroundService(intent);
        }

        public void StopMyForegroundService()
        {
            var intent = new Intent(AndroidApp.Context, typeof(ForegroundServices));
            AndroidApp.Context.StopService(intent);
        }

        public bool IsForeGroundServiceRunning()
        {
            return IsForegroundServiceRunning;
        }

        public void SendNotification(string message)
        {
            string channelID = "Proviser2NotificationChannel";
            var SnotificationManager = (NotificationManager)GetSystemService(NotificationService);

            var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.High);
            SnotificationManager.CreateNotificationChannel(notfificationChannel);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channelID)
            .SetContentTitle("Proviser2")
            .SetContentText(message)
            .SetSmallIcon(Resource.Drawable.abc_ic_arrow_drop_right_black_24dp);

            // Build the notification:
            Notification Snotification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, Snotification);

        }
    }
}
