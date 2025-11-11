using System;

namespace HeroServer
{
    public class VisionLiveness3dResult : LeapResult
    {
        public bool Success { get; set; }
        public bool WasProcessed { get; set; }
        public String LivenessStatus { get; set; }
        public String ScanResultBlob { get; set; }

        public VisionLiveness3dResult()
        {
        }

        public VisionLiveness3dResult(bool success, bool wasProcessed, String livenessStatus, String scanResultBlob)
        {
            Success = success;
            WasProcessed = wasProcessed;
            LivenessStatus = livenessStatus;
            ScanResultBlob = scanResultBlob;
        }
    }
}
