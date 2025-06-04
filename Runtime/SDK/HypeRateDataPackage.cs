using System;

namespace OmiLAXR.HypeRate.SDK
{
    
    [Serializable]
    internal class HypeRateDataPackage
    {
        public string @event; // @ escapes the "event" keyword
        public HypeRateDataPackagePayload payload;
    }
}