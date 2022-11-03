using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proviser2.Droid
{
    public interface IForegroundService
    {
        void StartMyForegroundService();
        void StopMyForegroundService();
        bool IsForeGroundServiceRunning();
        void SendNotification(string message);
    }
}
