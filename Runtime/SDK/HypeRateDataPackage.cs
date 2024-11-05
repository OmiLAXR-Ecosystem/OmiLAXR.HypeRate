using System;

namespace HypeRate.SDK
{
    
    [Serializable]
    public class HypeRateDataPackage
    {
        public string @event; // @ escapes the "event" keyword
        public HypeRateDataPackagePayload payload;
    }
}