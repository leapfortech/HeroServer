using System;

namespace HeroServer
{
    public class CardRegister
    {
        public long AppUserId { get; set; }
        public String InstrumentIdentifierId { get; set; }
        public int TypeId { get; set; }
        public String Number { get; set; }
        public int Digits { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public String Holder { get; set; }
        public float UtcOffset { get; set; } = -4f;

        public CardRegister()
        {
        }

        public CardRegister(long appUserId, String instrumentIdentifierId, int typeId, String number, int digits, int expirationMonth, int expirationYear, String holder, float utcOffset)
        {
            AppUserId = appUserId;
            InstrumentIdentifierId = instrumentIdentifierId;
            TypeId = typeId;
            Number = number;
            Digits = digits;
            ExpirationMonth = expirationMonth;
            ExpirationYear = expirationYear;
            Holder = holder;
            UtcOffset = utcOffset;
        }
    }
}