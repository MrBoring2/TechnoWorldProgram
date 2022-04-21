using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Terminal.Services
{
    public class CustomNotificationManager
    {
        private static NotificationManager notificationManager;
        private static CustomNotificationManager instance;
        private CustomNotificationManager() { }

        public static void ShowNotification(NotificationContent content, string areaDisplayName)
        {
            if (notificationManager == null)
            {
                notificationManager = new NotificationManager();
            }

            notificationManager.Show(content, areaName: areaDisplayName);
        }

    }
}
