using System;

namespace HeroServer
{
    public class PrecheckPhone
    {
        public long Id { get; set; }
        public long CountryId { get; set; }
        public String Number { get; set; }
        public String Code { get; set; }
        public String CountryCode { get; set; }
        public String CallerName { get; set; }
        public String CarrierCountryCode { get; set; }
        public String CarrierNetworkCode { get; set; }
        public String CarrierName { get; set; }
        public String CarrierType { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public int Status { get; set; }

        public PrecheckPhone()
        {
        }

        public PrecheckPhone(long id, long countryId, String number, String code, String countryCode, String callerName, String carrierCountryCode,
                             String carrierNetworkCode, String carrierName, String carrierType, DateTime createDateTime,
                             DateTime updateDateTime, int status)
        {
            Id = id;
            CountryId = countryId;
            Code = code;
            Number = number;
            CountryCode = countryCode;
            CallerName = callerName;
            CarrierCountryCode = carrierCountryCode;
            CarrierNetworkCode = carrierNetworkCode;
            CarrierName = carrierName;
            CarrierType = carrierType;
            CreateDateTime = createDateTime;
            UpdateDateTime = updateDateTime;
            Status = status;
        }
    }
}
