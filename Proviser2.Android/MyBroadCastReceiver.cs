using Android.App;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace Proviser2.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class MyBroadCastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                var foreGroundServiceIntent = new Intent(Application.Context, typeof(ForegroundServices));
               Application.Context.StartForegroundService(intent);
                context.StartForegroundService(foreGroundServiceIntent);
            }
        }
    }
}
