using System;

namespace HeroServer
{
    public class Onboarding
    {
        public int Id { get; set; } = -1;
        public int AppUserId { get; set; } = -1;
        public long DpiFront { get; set; } = 0;
        public long DpiBack { get; set; } = 0;
        public long Renap { get; set; } = 0;
        public long Portrait { get; set; } = 16383;
        public long Address { get; set; } = 0;
        public int IdentityId { get; set; } = -1;
        public int RenapIdentityId { get; set; } = -1;
        public int AddressId { get; set; } = -1;
        public String Comment { get; set; }
        public int BoardUserId { get; set; } = -1;
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; } = 1;


        public Onboarding()
        {
        }

        public Onboarding(int clientId, int identityId, int addressId)
        {
            AppUserId = clientId;
            IdentityId = identityId;
            AddressId = addressId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
        }

        public Onboarding(int appUserId, int identityId, int addressId, int boardUserId)
        {
            AppUserId = appUserId;
            IdentityId = identityId;
            AddressId = addressId;
            BoardUserId = boardUserId;
            CreateDateTime = DateTime.Now;
            UpdateDateTime = DateTime.Now;
        }

        public Onboarding(int id, int appUserId, long dpiFront, long dpiBack, long renap, long portrait, long address,
                          int identityId, int renapIdentityId, int addressId, String comment, int boardUserId,
                          DateTime createDateTime, DateTime updateDateTime, int status)
        {
            Id = id;
            AppUserId = appUserId;
            DpiFront = dpiFront;
            DpiBack = dpiBack;
            Renap = renap;
            Portrait = portrait;
            Address = address;
            IdentityId = identityId;
            RenapIdentityId = renapIdentityId;
            AddressId = addressId;
            Comment = comment;
            BoardUserId = boardUserId;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }

        public Onboarding(Onboarding onboarding)
        {
            Id = onboarding.Id;
            AppUserId = onboarding.AppUserId;
            DpiFront = onboarding.DpiFront;
            DpiBack = onboarding.DpiBack;
            Renap = onboarding.Renap;
            Portrait = onboarding.Portrait;
            Address = onboarding.Address;
            IdentityId = onboarding.IdentityId;
            RenapIdentityId = onboarding.RenapIdentityId;
            AddressId = onboarding.AddressId;
            Comment = onboarding.Comment;
            BoardUserId = onboarding.BoardUserId;
            CreateDateTime = onboarding.CreateDateTime;
            UpdateDateTime = onboarding.UpdateDateTime;
            Status = onboarding.Status;
        }

        // Get Checks

        private static long GetChecks(long data)
        {
            return data & ~(0x7FFL << 52);
        }

        public long GetDpiFrontChecks()
        {
            return GetChecks(DpiFront);
        }

        public long GetDpiBackChecks()
        {
            return GetChecks(DpiBack);
        }

        public long GetRenapChecks()
        {
            return GetChecks(Renap);
        }

        public long GetPortraitChecks()
        {
            return GetChecks(Portrait);
        }

        public long GetAddressChecks()
        {
            return GetChecks(Address);
        }

        // Get Check

        private static bool GetBoolCheck(long data, int fieldIdx)
        {
            return (data & (1L << fieldIdx)) != 0;
        }

        private static int GetIntCheck(long data, int fieldIdx, int length)
        {
            return (int)((data >> (fieldIdx * length)) & ((1L << length) - 1));
        }

        public bool GetDpiFrontCheck(int fieldIdx)
        {
            return GetBoolCheck(DpiFront, fieldIdx);
        }

        public bool GetDpiBackCheck(int fieldIdx)
        {
            return GetBoolCheck(DpiBack, fieldIdx);
        }

        public int GetRenapCheck(int fieldIdx)
        {
            return GetIntCheck(Renap, fieldIdx, 2);
        }

        public int GetPortraitCheck(int fieldIdx)
        {
            return GetIntCheck(Portrait, fieldIdx, 7);
        }

        public bool GetAddressCheck(int fieldIdx)
        {
            return GetBoolCheck(Address, fieldIdx);
        }

        // Set Check

        private static long SetBoolCheck(long data, int fieldIdx, bool value)
        {
            return value ? (data | (1L << fieldIdx)) : (data & ~(1L << fieldIdx));
        }

        private static long SetIntCheck(long data, int fieldIdx, int value, int length)
        {
            return (data & ~(((1L << length) - 1) << (fieldIdx * length))) | ((long)value << (fieldIdx * length));
        }

        public bool SetDpiFrontCheck(int fieldIdx, bool value)
        {
            if (GetDpiFrontCheck(fieldIdx) == value)
                return false;
            DpiFront = SetBoolCheck(DpiFront, fieldIdx, value);
            return true;
        }

        public bool SetDpiBackCheck(int fieldIdx, bool value)
        {
            if (GetDpiBackCheck(fieldIdx) == value)
                return false;
            DpiBack = SetBoolCheck(DpiBack, fieldIdx, value);
            return true;
        }

        public bool SetRenapCheck(int fieldIdx, int value)
        {
            if (GetRenapCheck(fieldIdx) == value)
                return false;
            Renap = SetIntCheck(Renap, fieldIdx, value, 2);
            return true;
        }

        public bool SetPortraitCheck(int fieldIdx, int value)
        {
            if (GetPortraitCheck(fieldIdx) == value)
                return false;
            Portrait = SetIntCheck(Portrait, fieldIdx, value, 7);
            return true;
        }

        public bool SetAddressCheck(int fieldIdx, bool value)
        {
            if (GetAddressCheck(fieldIdx) == value)
                return false;
            Address = SetBoolCheck(Address, fieldIdx, value);
            return true;
        }

        // Get Result

        private static int GetResult(long data)
        {
            return (int)(data >> 52); // & 0x7FFL);
        }

        public int GetDpiFrontResult()
        {
            return GetResult(DpiFront);
        }

        public int GetDpiBackResult()
        {
            return GetResult(DpiBack);
        }

        public int GetRenapResult()
        {
            return GetResult(Renap);
        }

        public int GetPortraitResult()
        {
            return GetResult(Portrait);
        }

        public int GetAddressResult()
        {
            return GetResult(Address);
        }

        public static int GetResultType(int result)
        {
            return result >> 7;
        }

        public static int GetResultValue(int result)
        {
            return result & 0x7F;
        }

        // Set Result

        private static long SetResult(long data, int result)
        {
            return (data & ~(0x7FFL << 52)) | ((long)result << 52);
        }

        public void SetDpiFrontResult(int result)
        {
            DpiFront = SetResult(DpiFront, result);
        }

        public void SetDpiBackResult(int result)
        {
            DpiBack = SetResult(DpiBack, result);
        }

        public void SetRenapResult(int result)
        {
            Renap = SetResult(Renap, result);
        }

        public void SetPortraitResult(int result)
        {
            Portrait = SetResult(Portrait, result);
        }

        public void SetAddressResult(int result)
        {
            Address = SetResult(Address, result);
        }

        public static int SetResultType(int result, int type)
        {
            return (result & ~(0xF << 7)) | (type << 7);
        }

        public static int SetResultValue(int result, int value)
        {
            return (result & ~0x7F) | value;
        }

        // Status

        public int GetStatus(int result)
        {
            int status = GetResultType(result) + 1;
            if (status == 2) // Success
                return AllResultSuccess() ? 2 : 1;
            return status;
        }

        public bool AllResultSuccess()
        {
            if (GetResultType(GetDpiFrontResult()) != 1)
                return false;
            if (GetResultType(GetDpiBackResult()) != 1)
                return false;
            if (GetResultType(GetRenapResult()) != 1)
                return false;
            if (GetResultType(GetPortraitResult()) != 1)
                return false;
            if (GetResultType(GetAddressResult()) != 1)
                return false;
            return true;
        }
    }
}
