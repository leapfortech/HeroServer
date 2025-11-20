using System;

namespace HeroServer
{
    public class Notification
    {
        public long Id { get; set; }
        public long WebSysUserId { get; set; }
        public String MessageId { get; set; }
        public String Title { get; set; }
        public String Body { get; set; }
        public String Action { get; set; }
        public String Information { get; set; }
        public String Parameter { get; set; }
        public int DisplayMode { get; set; }
        public DateTime DateTime { get; set; }
        public int NotificationStatusId { get; set; }


        public Notification()
        {
        }

        public Notification(long id, long webSysUserId, String messageId, String title, String body, String action,
                            String information, String parameter, int displayMode, DateTime dateTime, int notificationStatusId)
        {
            Id = id;
            WebSysUserId = webSysUserId;
            MessageId = messageId;
            Title = title;
            Body = body;
            Action = action;
            Information = information;
            Parameter = parameter;
            DisplayMode = displayMode;
            DateTime = dateTime;
            NotificationStatusId = notificationStatusId;
        }
    }
}
