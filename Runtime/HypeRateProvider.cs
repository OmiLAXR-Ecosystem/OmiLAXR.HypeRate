using OmiLAXR.Actors.HeartRate;
using UnityEngine;
using HR = HypeRate.SDK.HypeRate;

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

        // Textbox to display your heart rate in
        private HR _hypeRateSocket;

        private async void Start()
        {
            _hypeRateSocket = HR.GetInstance();

            _hypeRateSocket.OnHeartRateUpdate += hr =>
            {
                heartRate = hr;
            }; 
            
            await _hypeRateSocket.ConnectToServer(apiKey, hyperateURL);
            await _hypeRateSocket.JoinHeartbeatChannel(sessionId);
            
        }

        private async void OnApplicationQuit()
        {
            await _hypeRateSocket.CloseConnection();
        }
    }
}