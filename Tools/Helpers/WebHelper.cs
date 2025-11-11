using System;

namespace HeroServer
{
    public static class WebHelper
    {
        public static DateTime ConvertDate(String date)
        {
            try
            {
                return new DateTime(int.Parse(date[..4]), int.Parse(date[5..7]), int.Parse(date[8..10]));
            }
            catch
            {
                return new DateTime(1, 1, 1);
            }
        }

        public static DateTime ConvertDateTime(String date)
        {
            try
            {
                return new DateTime(int.Parse(date[..4]), int.Parse(date[5..7]), int.Parse(date[8..10]),
                                    int.Parse(date[11..13]), int.Parse(date[14..16]), int.Parse(date[17..19]));
            }
            catch
            {
                return new DateTime(1, 1, 1);
            }
        }

        public static DateTime? ConvertNullDate(String date)
        {
            try
            {
                return new DateTime(int.Parse(date[..4]), int.Parse(date[5..7]), int.Parse(date[8..10]));
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? ConvertNullDateTime(String date)
        {
            try
            {
                return new DateTime(int.Parse(date[..4]), int.Parse(date[5..7]), int.Parse(date[8..10]),
                                    int.Parse(date[11..13]), int.Parse(date[14..16]), int.Parse(date[17..19]));
            }
            catch
            {
                return null;
            }
        }

        public static String ConvertStringArray(String[] stringArray)
        {
            if (stringArray == null || stringArray.Length == 0)
                return "";

            String res = stringArray[0];
            for (int i = 1; i < stringArray.Length; i++)
                res += "|" + stringArray[i];
            return res;
        }

        public static int ConvertBoolean(Boolean trigger)
        {
            return trigger ? 1 : 0;
        }

        public static String TrimNotifMsgId(String message)
        {
            if (message == null)
                return null;

            int idx = message.LastIndexOf('/');
            if (idx == -1)
                return message;

            return message[(idx + 1)..];
        }
    }
}