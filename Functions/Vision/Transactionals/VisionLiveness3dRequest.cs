using System;

namespace HeroServer
{
    public class VisionLiveness3dRequest
    {
        public String FaceScan { get; set; }
        public String AuditTrailImage { get; set; }
        public String LowQualityAuditTrailImage { get; set; }
        public String UserAgent { get; set; }

        public VisionLiveness3dRequest()
        {
        }
    }
}
