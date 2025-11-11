using System;
using System.Collections.Generic;

namespace HeroServer
{
    public class LeapResponse
    {
        public int Code { get; set; }
        public String Message { get; set; }
        public LeapResult Result { get; set; }
        public bool Charged { get; set; }

        public LeapResponse()
        {

        }

        public LeapResponse(int code, String message, LeapResult result, bool charged = true)
        {
            Code = code;
            Message = message;
            Result = result;
            Charged = charged;
        }

        public LeapResponse(int code, String message, bool charged = true)
        {
            Code = code;
            Message = message;
            Result = null;
            Charged = charged;
        }

        public LeapResponse(int code, String message, String info, bool charged = true)
        {
            Code = code;
            Message = $"{message} ({info})";
            Result = null;
            Charged = charged;
        }

        public LeapResponse(int code, Dictionary<int, String> messages, bool charged = true)
        {
            Code = code;
            Message = messages.TryGetValue(code, out string message) ? message : $"Error #{code}";
            Result = null;
            Charged = charged;
        }

        public LeapResponse(int code, Dictionary<int, String> messages, String info, bool charged = true)
        {
            Code = code;
            Message = (messages.TryGetValue(code, out string message) ? message : $"Error #{code}") + $" ({info})";
            Result = null;
            Charged = charged;
        }
    }
}
