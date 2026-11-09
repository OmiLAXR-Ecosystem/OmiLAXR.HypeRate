using OmiLAXR.Actors.HeartRate;
using OmiLAXR.HypeRate.SDK;
using UnityEngine;
using HR = OmiLAXR.HypeRate.SDK.HypeRate;

namespace OmiLAXR.HypeRate
{
    public class HypeRateProvider : HeartRateProvider
    {
        public override int GetHeartRate() => heartRate;

        [ReadOnly]
        public int heartRate = 0;
        
        [Header("Get it here https://www.hyperate.io/api.")]
        public string apiKey = "<Request your API key>"; 
        [Header("Get it from your HypeRate App.")]
        public string sessionId = "internal-testing";
        public string hyperateURL = "wss://app.hyperate.io/socket/websocket";

        /// <summary>
        /// Socket delivering heart rates. Defaults to the HypeRate websocket; assign before Start to replace it, e.g. in tests.
        /// </summary>
        public IHeartRateSocket Socket { get; set; }

        private IHeartRateSocket _hypeRateSocket;

        private async void Start()
        {
            _hypeRateSocket = Socket ?? HR.GetInstance();

            _hypeRateSocket.HeartRateUpdated += hr =>
            {
                heartRate = hr;
            }; 
            
            await _hypeRateSocket.ConnectToServer(apiKey, hyperateURL);
            await _hypeRateSocket.JoinHeartbeatChannel(sessionId);
            
        }

        private async void OnApplicationQuit()
        {
            if (_hypeRateSocket == null)
                return;
            await _hypeRateSocket.CloseConnection();
        }
    }
}