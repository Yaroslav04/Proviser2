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
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Task.Run(async () =>
            {
                while (IsForegroundServiceRunning)
                {
                    Thread.Sleep(900000);//15 min
                    await Task.Run(async () =>
                    {
                        await NotificationAgregator.Run();
                    });

                    await ShowNotification();
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

        public void SendNotification(string _channel, int _id, string _title, string _message)
        {
            string channelID = _channel;
            var SnotificationManager = (NotificationManager)GetSystemService(NotificationService);

            var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.High);
            SnotificationManager.CreateNotificationChannel(notfificationChannel);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channelID)
            .SetContentTitle(_title)
            .SetContentText(_message)
            .SetSmallIcon(Resource.Drawable.abc_ic_arrow_drop_right_black_24dp);

            // Build the notification:
            Notification Snotification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            //const int notificationId = 0;
            notificationManager.Notify(_id, Snotification);
        }

        async Task ShowNotification()
        {
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday & DateTime.Now.DayOfWeek != DayOfWeek.Saturday)
            {
                if (DateTime.Now.Hour >= 9 & DateTime.Now.Hour <= 18)
                {
                    var notifications = await App.DataBase.Notification.GetListAsync();
                    if (notifications.Count > 0)
                    {
                        foreach (var notification in notifications)
                        {
                            if (notification.IsExecute == true & notification.IsNotificate == true)
                            {

                                notification.IsNotificate = false;
                                await App.DataBase.Notification.UpdateAsync(notification);

                                SendNotification(notification.Type, notification.N,
                                    GetTitleFromNotificationType(notification.Type), notification.Description);
                            }
                        }
                    }
                }
            }

            string GetTitleFromNotificationType(string _value)
            {
                if (_value == App.NotificationType[0])
                {
                    return "Судове засідання";
                }

                if (_value == App.NotificationType[1])
                {
                    return "Тримання під вартою";
                }

                return "";
            }
        }
    }
}
