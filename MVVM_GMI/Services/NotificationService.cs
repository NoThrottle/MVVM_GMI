using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_GMI.Services
{
    internal class NotificationService
    {
        private static NotificationService instance;
        private static readonly object lockObject = new object();

        private NotificationService()
        {
        }

        public static NotificationService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new NotificationService();
                        }
                    }
                }
                return instance;
            }
        }

        List<Notification> _notifications = new List<Notification>();

        public List<Notification> Notifications
        {
            get { return _notifications; }
        }


        List<PrivateMessage> _privateMessages = new List<PrivateMessage>();

        public List<PrivateMessage> PrivateMessages(Type t)
        {
            var y = new List<PrivateMessage>(_privateMessages.Where(type => type.Receiver == t).ToList());
            _privateMessages.RemoveAll(type => type.Receiver == t);
            return y;
        }


        public void AddNotification(Notification notification)
        {
            _notifications.Add(notification);
        }
         
        public void AddMessage(PrivateMessage privateMessage)
        {
            _privateMessages.Add(privateMessage);
        }


    }

    class Notification
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string Appearance { get; set; }

        public string Timestamp { get; set; }

        public bool Persistent { get; set; }

        public ClickAction clickAction { get; set; }
    }

    class ClickAction
    {
        public string NavigateToPage {  get; set; }

        public string NavigateToURL { get; set; }
    }

    class PrivateMessage
    {
        /// <summary>
        /// Contents to be read
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Name of the class to receive it
        /// </summary>
        public Type Receiver { get; set; }


    }
}
